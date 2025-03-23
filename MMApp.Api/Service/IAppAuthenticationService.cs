using MMApp.Api.Dtos.Account;
using MMApp.Api.Identity.Models;

namespace MMApp.Api.Service;

public interface IAppAuthenticationService
{
    Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request);
    GetAllAppUsersResponse GetAllAppUsers(GetAllAppUsersRequest getAllAppUsersRequest);
    Task<TokenRefreshResult> RefreshToken();
    Task<RegistrationResponse> RegisterAsync(RegistrationRequest request);
    Task<RemoveUserResponse> RemoveUserAsync(RemoveUserRequest removeUserRequest);
    Task<UpdateAppUserResponse> UpdateAppUserAsync(UpdateAppUserRequest updateAppUserRequest);

}
