using System.ComponentModel.DataAnnotations;

namespace MMApp.Api.Identity.Models;

/// <summary>
/// User registration model
/// </summary>
public class RegistrationRequest
{
    /// <summary>
    /// </summary>
    /// <example>Mihai</example>
    [Required]
    public string FirstName { get; set; } = string.Empty;

    /// <example>Moldovan</example>
    [Required]
    public string LastName { get; set; } = string.Empty;

    /// <example>user@user.com</example>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <example>User123!@#</example>
    [Required]
    public string Password { get; set; } = string.Empty;

    /// <example>0755111111</example>
    [Required]
    public string Mobile { get; set; } = string.Empty;
}
