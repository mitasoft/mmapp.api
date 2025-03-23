namespace MMApp.Api.Service
{
    public interface IEncodersService
    {
        string Encode(string input);
        string Decode(string input);
    }
}
