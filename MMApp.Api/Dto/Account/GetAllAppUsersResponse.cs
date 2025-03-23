using MMApp.Api.Identity.Models;

namespace MMApp.Api.Dtos.Account
{
    public class GetAllAppUsersResponse
    {
        public List<ApplicationUser> Users { get; internal set; }
    }
    
}
