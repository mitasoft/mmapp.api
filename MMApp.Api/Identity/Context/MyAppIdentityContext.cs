using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MMApp.Api.Identity.Models;

namespace MMApp.Api.Identity.Context;

public class MyAppIdentityContext : IdentityDbContext<ApplicationUser>
{
    public MyAppIdentityContext()
    {

    }
    public MyAppIdentityContext(DbContextOptions<MyAppIdentityContext> options) : base(options)
    {

    }

}
