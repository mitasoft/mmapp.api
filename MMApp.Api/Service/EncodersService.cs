using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace MMApp.Api.Service;

public class EncodersService : IEncodersService
{
    public string Decode(string input)
    {
        var decoded = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(input));
        return decoded;
    }

    public string Encode(string input)
    {
        var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(input));
        return encoded;
    }
}
