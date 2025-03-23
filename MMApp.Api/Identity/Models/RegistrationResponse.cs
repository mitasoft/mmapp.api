namespace MMApp.Api.Identity.Models;

public class RegistrationResponse
{
    public string UserId { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public string Error { get; set; }
}
