using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using MMApp.Api.Service;
using MMApp.Api.Dtos.Account;
using MMApp.Api.Identity.Models;

namespace MMApp.Api.Controllers.v1
{
    [Route("api/v1/account")]
    [ApiController]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class AccountController : ControllerBase
    {
        private readonly IAppAuthenticationService _authenticationService;
        private readonly INewUserService _newUserService;

        public AccountController(
            IAppAuthenticationService authenticationService,
            INewUserService newUserService)
        {
            _authenticationService = authenticationService;
            _newUserService = newUserService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegistrationResponse>> RegisterAsync(RegistrationRequest request)
        {
            return Ok(await _authenticationService.RegisterAsync(request));
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request)
        {
            return Ok(await _authenticationService.AuthenticateAsync(request));
        }

        [Authorize(Roles = "admin")]
        [HttpPost("registerNewUser")]
        public async Task<ActionResult<RegistrationResponse>> RegisterNewUserAsync(RegistrationRequest request)
        {
            return Ok(await _newUserService.RegisterNewUserAsync(request));
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var result = await _authenticationService.RefreshToken();

            if (result.IsSuccess)
                return Ok(result);

            return Unauthorized();
        }

        //[Authorize(Roles = "admin")]
        [HttpPost("GetAllAppUsers")]
        public IActionResult GetAllAppUsers(GetAllAppUsersRequest getAllAppUsersRequest)
        {
            var result = _authenticationService.GetAllAppUsers(getAllAppUsersRequest);
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("RemoveUser")]
        public async Task<IActionResult> RemoveUser(RemoveUserRequest removeUserRequest)
        {
            var result = await _authenticationService.RemoveUserAsync(removeUserRequest);
            return Ok(result);
        }

        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(EmailConfirmRequest emailConfirmRequest)
        {
            var confirmResult = await _newUserService.ConfirmEmail(emailConfirmRequest);
            return Ok(confirmResult);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateAppUser(UpdateAppUserRequest updateAppUserRequest)
        {
            UpdateAppUserResponse result = await _authenticationService.UpdateAppUserAsync(updateAppUserRequest);
            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordNotificationRequest resetPasswordNotificationRequest)
        {
            await _newUserService.ResetPasswordNotificationAsync(resetPasswordNotificationRequest);
            return Ok();
        }

        [HttpPost("validate-reset-password")]
        public async Task<IActionResult> ValidateResetPassword(Dtos.Account.ResetPasswordRequest resetPasswordRequest)
        {
            var result = await _newUserService.ResetPasswordAsync(resetPasswordRequest);
            return Ok(result);
        }
    }
}
