namespace MMApp.Api.Identity.Models;

public class AuthenticationResponse
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;


    public string Error { get; set; }
    public bool IsSuccess { get; set; }
    public int ErrorCode { get; set; }

    public static readonly AuthenticationResponse UserNotFound = new()
    {
        IsSuccess = false,
        Error = "User not found",
        ErrorCode = 1
    };

    public static readonly AuthenticationResponse EmailNotConfirmed = new()
    {
        IsSuccess = false,
        Error = "User didn't confirmed the email.",
        ErrorCode = 2
    };

    public static readonly AuthenticationResponse UnableToLogin = new()
    {
        IsSuccess = false,
        Error = "Unable to login",
        ErrorCode = 3
    };

}
