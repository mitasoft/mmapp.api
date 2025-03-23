namespace MMApp.Api.Identity.Models;

public class AuthenticationRequest
{
    /// <summary>
    /// </summary>
    /// <example>user4@user.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    /// <example>User123!@#</example>
    public string Password { get; set; } = string.Empty;
}
