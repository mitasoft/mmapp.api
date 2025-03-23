namespace MMApp.Api.Dtos.Account;

public class UpdateAppUserRequest
{
    public string Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string[] Roles { get; set; } = [];
}
