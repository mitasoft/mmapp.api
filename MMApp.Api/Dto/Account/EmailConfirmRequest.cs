namespace MMApp.Api.Dtos.Account
{
    public class EmailConfirmRequest
    {
        public string Uid { get; set; } = string.Empty;
        public string Cid { get; set; } = string.Empty;
    }
}
