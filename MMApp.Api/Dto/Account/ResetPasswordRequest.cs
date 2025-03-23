namespace MMApp.Api.Dtos.Account
{
    public class ResetPasswordRequest
    {
        public string Uid { get; set; }
        public string Cid { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
