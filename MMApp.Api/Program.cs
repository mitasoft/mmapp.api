
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MMApp.Api.Configuration;
using MMApp.Api.Identity.Context;
using MMApp.Api.Identity.Models;
using MMApp.Api.Service;
using System.Reflection;

namespace MMApp.Api;

public static class Program
{
    static void AddConfigFiles(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddJsonFile("appsettings.json", optional: false);
        builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: false);
    }

    static void AddIdentity(this WebApplicationBuilder builder)
    {
        ArgumentException.ThrowIfNullOrEmpty(builder.Configuration["JwtSettings:Key"]);

        builder.Services.AddOptions<JwtSettings>()
            .BindConfiguration("JwtSettings")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddDbContext<MyAppIdentityContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("MyDbContextIdentity")));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<MyAppIdentityContext>()
            .AddDefaultTokenProviders();
    }


    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddConfigFiles();
        builder.AddIdentity();


        builder.Services.AddTransient<IAppAuthenticationService, AppAuthenticationService>();
        builder.Services.AddTransient<IEncodersService, EncodersService>();
        builder.Services.AddTransient<INewUserService, NewUserService>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
