using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MMApp.Api.Configuration;
using MMApp.Api.Dtos.Account;
using MMApp.Api.Identity.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MMApp.Api.Service;

public class AppAuthenticationService : IAppAuthenticationService
{

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    //private readonly ModusContext _modusContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly RoleManager<IdentityRole> _roleManager;
    // readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    //private readonly IEmailTemplateService _emailTemplateService;
    private readonly JwtSettings _jwtSettings;

    public AppAuthenticationService(
        UserManager<ApplicationUser> userManager,
        IOptions<JwtSettings> jwtSettings,
        SignInManager<ApplicationUser> signInManager,
        //ModusContext modusContext,
        IHttpContextAccessor httpContextAccessor,
        RoleManager<IdentityRole> roleManager,
        //IEmailService emailService,
        IConfiguration configuration
        //IEmailTemplateService emailTemplateService
        )
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
        _signInManager = signInManager;
        //_modusContext = modusContext;
        _httpContextAccessor = httpContextAccessor;
        _roleManager = roleManager;
        //_emailService = emailService;
        _configuration = configuration;
        //_emailTemplateService = emailTemplateService;
    }

    public async Task<RegistrationResponse> RegisterAsync(RegistrationRequest request)
    {
        var existingUser = await _userManager.FindByNameAsync(request.Email);

        if (existingUser != null)
            return new RegistrationResponse { IsSuccess = false, Error = "User already exists" };

        var user = new ApplicationUser
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.Email,
            EmailConfirmed = true,
            Mobile = request.Mobile,
            RefreshTokenExpireDate = DateTime.UtcNow.AddDays(2),
            RefreshTokenCreatedDate = DateTime.UtcNow,
            RefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(512))
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        var roleResult = await _userManager.AddToRoleAsync(user, "user");

        //CreateCookieForRefreshToken(refreshToken);

        if (result.Succeeded)
            return new RegistrationResponse() { IsSuccess = true, UserId = user.Id };
        else
            return new RegistrationResponse() { IsSuccess = false, Error = string.Join(",", result.Errors.Select(x => x.Description)) };

    }

    public GetAllAppUsersResponse GetAllAppUsers(GetAllAppUsersRequest getAllAppUsersRequest)
    {
        var users = from user in _userManager.Users
                        //join userProfile in _modusContext.UserProfiles on user.Id equals userProfile.UserId
                        //orderby user.LastName, user.FirstName
                    select user;




        return new GetAllAppUsersResponse
        {
            Users = users.ToList(),
        };
    }

    public async Task<RemoveUserResponse> RemoveUserAsync(RemoveUserRequest removeUserRequest)
    {
        //var applicationUser = _userManager.Users.FirstOrDefault(x => x.Id == removeUserRequest.UserId);

        //if (applicationUser != null)
        //{
        //    await _userManager.DeleteAsync(applicationUser);

        //    var appProfile = _modusContext.UserProfiles.FirstOrDefault(x => x.UserId == applicationUser.Id);
        //    if (appProfile != null)
        //    {
        //        _modusContext.UserProfiles.Remove(appProfile);
        //        _modusContext.SaveChanges();
        //    }

        //    _modusContext.SaveChanges(true);

        //}

        return new RemoveUserResponse();
    }

    public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return AuthenticationResponse.UserNotFound;

        var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

        if (!isEmailConfirmed)
            return AuthenticationResponse.EmailNotConfirmed;

        var result = await _signInManager.PasswordSignInAsync(
            user.Email!,
            request.Password,
            isPersistent: false,
            lockoutOnFailure: false);

        if (!result.Succeeded)
            return AuthenticationResponse.UnableToLogin;

        JwtSecurityToken jwtSecurityToken = await GenerateToken(user);

        AuthenticationResponse response = new AuthenticationResponse
        {
            Id = user.Id,
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            Email = user.Email!,
            UserName = user.UserName!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsSuccess = true
        };

        user.RefreshTokenExpireDate = DateTime.UtcNow.AddDays(2);
        user.RefreshTokenCreatedDate = DateTime.UtcNow;
        user.RefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(512));
        await _userManager.UpdateAsync(user);

        return response;
    }


    /// <summary>
    /// Create cookie for Refreh Token
    /// Cookie type: HttpOnly
    /// Not accesbile from Javascript
    /// </summary>
    /// <param name="refreshToken"></param>
    //private void CreateCookieForRefreshToken(RefreshToken refreshToken)
    //{
    //    var cookieOptions = new CookieOptions
    //    {
    //        HttpOnly = true,
    //        Expires = refreshToken.Expires,
    //        SameSite = SameSiteMode.None,
    //        Secure = true,
    //        IsEssential = true
    //    };

    //    _httpContextAccessor.HttpContext!.Response.Cookies.Append(
    //        "refreshToken",
    //        refreshToken.Token,
    //        cookieOptions);
    //}

    
    
    private async Task<JwtSecurityToken> GenerateToken(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();

        for (int i = 0; i < roles.Count; i++)
        {
            roleClaims.Add(new Claim("roles", roles[i]));
        }

        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim("uid", user.Id),
                new Claim("modusRoles", string.Join(",", roles))
            }
        .Union(userClaims)
        .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var expirePerEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
            ? DateTime.UtcNow.AddSeconds(_jwtSettings.DurationInSeconds)
            : DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expirePerEnv, //DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
            signingCredentials: signingCredentials);
        return jwtSecurityToken;
    }

    public async Task<TokenRefreshResult> RefreshToken()
    {
        //var refreshToken = _httpContextAccessor.HttpContext!.Request.Cookies["refreshToken"];

        //Trace.WriteLine(string.Format("Refresh Token read from cookie: {0, -100}", refreshToken));


        //var userProfile = _modusContext.UserProfiles
        //    .Include(x => x.AspNetUser)
        //    .Where(u => u.RefreshToken == refreshToken)
        //    .FirstOrDefault();

        //if (userProfile == null || userProfile != null && userProfile.TokenExpires <= DateTime.UtcNow)
        //{
        //    return new TokenRefreshResult { IsUnauthorized = true };
        //}

        //ApplicationUser appUser = await _userManager.FindByIdAsync(userProfile!.UserId);

        //if (appUser == null)
        //{
        //    return new TokenRefreshResult { IsUnauthorized = true };
        //}

        //// create a new token for the user
        //var newToken = await GenerateToken(appUser);

        //// create a new refresh token for the user
        //var newRefreshToken = GenerateRefreshToken();
        //Trace.WriteLine(string.Format("New Refresh Token write to cookie: {0, -100}", newRefreshToken.Token));

        //userProfile.RefreshToken = newRefreshToken.Token;
        //userProfile.TokenCreated = newRefreshToken.Created;
        //userProfile.TokenExpires = newRefreshToken.Expires;

        //await _modusContext.SaveChangesAsync();

        //CreateCookieForRefreshToken(newRefreshToken);

        //// return the new token (not the refresh token)
        //return new TokenRefreshResult
        //{
        //    FirstName = userProfile.FirstName,
        //    LastName = userProfile.LastName,
        //    Token = new JwtSecurityTokenHandler().WriteToken(newToken),
        //    IsSuccess = true
        //};

        return new TokenRefreshResult { };
    }



    public async Task<UpdateAppUserResponse> UpdateAppUserAsync(UpdateAppUserRequest updateAppUserRequest)
    {
        //var appUser = await _userManager.FindByIdAsync(updateAppUserRequest.Id);

        //if (appUser != null)
        //{
        //    var existingRoles = await _userManager.GetRolesAsync(appUser);
        //    await _userManager.RemoveFromRolesAsync(appUser, existingRoles);
        //    await _userManager.AddToRolesAsync(appUser, updateAppUserRequest.Roles);

        //    appUser.FirstName = updateAppUserRequest.FirstName;
        //    appUser.LastName = updateAppUserRequest.LastName;
        //    //appUser.Email = updateAppUserRequest.Email;

        //    var updateResult = await _userManager.UpdateAsync(appUser);

        //    if (updateResult.Succeeded)
        //    {
        //        var userProfile = _modusContext.UserProfiles.FirstOrDefault(up => up.UserId == appUser.Id);
        //        if (userProfile != null)
        //        {
        //            userProfile.FirstName = updateAppUserRequest.FirstName;
        //            userProfile.LastName = updateAppUserRequest.LastName;
        //            //userProfile.Email = updateAppUserRequest.Email;
        //            userProfile.Mobile = updateAppUserRequest.Mobile;

        //            _modusContext.SaveChanges();
        //        }
        //    }
        //}

        return new UpdateAppUserResponse();
    }


}
