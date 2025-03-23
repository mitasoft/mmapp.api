using MMApp.Api.Dtos.Account;
using MMApp.Api.Identity.Models;

namespace MMApp.Api.Service
{
    public interface INewUserService
    {
        Task<RegistrationResponse> RegisterNewUserAsync(RegistrationRequest request);
        Task<EmailConfirmResponse> ConfirmEmail(EmailConfirmRequest emailConfirmRequest);
        Task<ResetPasswordResponse> ResetPasswordAsync(MMApp.Api.Dtos.Account.ResetPasswordRequest validateForgotPasswordRequest);
        Task<ResetPasswordNotificationResponse> ResetPasswordNotificationAsync(ResetPasswordNotificationRequest resetPasswordNotificationRequest);
    }
}
