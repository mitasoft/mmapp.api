namespace MMApp.Api.Identity.Models
{
    public class TokenRefreshResult
    {
        public bool IsUnauthorized { get; set; }
        public string Token { get; internal set; }
        public bool IsSuccess { get; internal set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
