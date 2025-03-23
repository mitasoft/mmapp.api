using Microsoft.AspNetCore.Identity;

namespace MMApp.Api.Identity.Models;

public class ApplicationUser : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Mobile { get; set; }
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public required string RefreshToken { get; set; }
    public required DateTime RefreshTokenExpireDate { get; set; }
    public required DateTime RefreshTokenCreatedDate { get; set; }
}
