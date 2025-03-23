using Microsoft.AspNetCore.Identity;
using MMApp.Api.Dtos.Account;
using MMApp.Api.Identity.Models;
using System.Data;
using System.Security.Cryptography;

namespace MMApp.Api.Service
{
    public class NewUserService : INewUserService
    {
        public const string EMAIL_SUBJECT_CONFIRM_EMAIL = "Email confirmation";
        public const string EMAIL_SUBJECT_RESET_PASSWORD = "Password reset";

        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly ModusContext _modusContext;
       
        //private readonly IEmailService _emailService;
        //private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEncodersService _encodersService;

        public NewUserService(
            UserManager<ApplicationUser> userManager,
            //ModusContext modusContext,
            //IEmailService emailService,
            //IEmailTemplateService emailTemplateService,
            IEncodersService encodersService
            )
        {
            _userManager = userManager;
            // = modusContext;
            //_emailService = emailService;
            //_emailTemplateService = emailTemplateService;
            _encodersService = encodersService;
        }


        /// <summary>
        /// STEP 1. Register a NEW USER without PASSWORD
        /// STEP 2. Email CONFIRMATION is sent to user email 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RegistrationResponse> RegisterNewUserAsync(RegistrationRequest request)
        {
            var existingUser = await _userManager.FindByNameAsync(request.Email);
            if (existingUser != null)
            {
                var error = $"User Name '{request.Email}' already exists.";
                return new RegistrationResponse { IsSuccess = false, Error = error };
            }

            var existingEmail = await _userManager.FindByEmailAsync(request.Email);
            if (existingEmail != null)
            {
                var error = $"Email address '{request.Email}' already used.";
                return new RegistrationResponse { IsSuccess = false, Error = error };
            }

            var newUser = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Email,
                EmailConfirmed = false,
                Mobile = request.Mobile,
                RefreshTokenExpireDate = DateTime.UtcNow.AddDays(2),
                RefreshTokenCreatedDate = DateTime.UtcNow,
                RefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(512))
            };


            //_modusContext.UserProfiles.Add(new UserProfile
            //{
            //    UserId = newUser.Id,
            //    FirstName = newUser.FirstName,
            //    LastName = newUser.LastName,
            //    Email = newUser.Email,
            //    Mobile = request.Mobile,
            //    IsActive = true,
            //    RefreshToken = "",
            //    TokenCreated = DateTime.UtcNow,
            //    TokenExpires = DateTime.UtcNow
            //});

            //await _modusContext.SaveChangesAsync();

            var result = await _userManager.CreateAsync(newUser);
            await _userManager.AddToRoleAsync(newUser, "user");
            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

            //var confirmationTokenEncoded = _encodersService.Encode(confirmationToken);
            //var userIdEncoded = _encodersService.Encode(newUser.Id);

            //var emailBody = _emailTemplateService.GetConfirmEmailBody(confirmationTokenEncoded, userIdEncoded);

            //await _emailService.SendEmailAsync(
            //    newUser.Email,
            //    newUser.FullName,
            //    EMAIL_SUBJECT_CONFIRM_EMAIL,
            //    emailBody);

            if (result.Succeeded)
            {
                return new RegistrationResponse()
                {
                    UserId = newUser.Id,
                    IsSuccess = true
                };
            }
            else
            {
                return new RegistrationResponse()
                {
                    IsSuccess = false,
                    Error = string.Join(",", result.Errors.Select(x => x.Description))
                };
            }
        }


        /// <summary>
        /// STEP 3. User confirm via EMAIL. Email is confirmed.
        /// </summary>
        /// <param name="emailConfirmRequest"></param>
        /// <returns></returns>
        public async Task<EmailConfirmResponse> ConfirmEmail(EmailConfirmRequest emailConfirmRequest)
        {
            try
            {
                //emailConfirmRequest.Uid.Should().NotBeNullOrEmpty();
                //emailConfirmRequest.Cid.Should().NotBeNullOrEmpty();

                var userId = _encodersService.Decode(emailConfirmRequest.Uid);

                var applicationUser = await _userManager.FindByIdAsync(userId);
                if (applicationUser != null)
                {
                    var confirmationCode = _encodersService.Decode(emailConfirmRequest.Cid);

                    var confirmResult = await _userManager.ConfirmEmailAsync(applicationUser, confirmationCode);
                    if (!confirmResult.Succeeded)
                    {
                        return new EmailConfirmResponse { Success = false };
                    }
                }

                return new EmailConfirmResponse { Success = true };
            }
            catch (Exception)
            {
                return new EmailConfirmResponse { Success = false };
            }
        }


        public async Task<ResetPasswordNotificationResponse> ResetPasswordNotificationAsync(ResetPasswordNotificationRequest resetPasswordNotificationRequest)
        {
            //resetPasswordNotificationRequest.Email.Should().NotBeNullOrEmpty();

            var user = await _userManager.FindByEmailAsync(resetPasswordNotificationRequest.Email);

            if (user != null)
            {
                var confirmationUid = await _userManager.GeneratePasswordResetTokenAsync(user);

                //var emailBody = _emailTemplateService.GetResetEmailBody(
                //    _encodersService.Encode(confirmationUid),
                //    _encodersService.Encode(user.Id));

                //await _emailService.SendEmailAsync(
                //    user.Email!,
                //    user.FullName,
                //    EMAIL_SUBJECT_RESET_PASSWORD,
                //    emailBody);

                return new ResetPasswordNotificationResponse { Success = true };
            }

            return new ResetPasswordNotificationResponse { Success = false };
        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest validateForgotPasswordRequest)
        {
            //validateForgotPasswordRequest.Uid.Should().NotBeNullOrEmpty();
            //validateForgotPasswordRequest.Cid.Should().NotBeNullOrEmpty();
            //validateForgotPasswordRequest.Password.Should().NotBeNullOrEmpty();
            //validateForgotPasswordRequest.ConfirmPassword.Should().NotBeNullOrEmpty();
            //validateForgotPasswordRequest.Password.Should().MatchEquivalentOf(validateForgotPasswordRequest.ConfirmPassword);

            var confirmationId = _encodersService.Decode(validateForgotPasswordRequest.Cid);
            var userId = _encodersService.Decode(validateForgotPasswordRequest.Uid);

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, confirmationId, validateForgotPasswordRequest.Password);

                return result.Succeeded ? new ResetPasswordResponse { Success = true } : new ResetPasswordResponse { Success = false };
            }

            return new ResetPasswordResponse
            {
                Success = false
            };

        }


    }
}
