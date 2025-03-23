using Microsoft.AspNetCore.Identity;
using System.Text;
using MMApp.Api.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MMApp.Api.Service;
using MMApp.Api.Identity.Context;
using MMApp.Api.Identity.Models;

namespace MMApp.Api.Identity;

public static class IdentityServiceExtensions
{
    public static void AddIdentityServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        ArgumentException.ThrowIfNullOrEmpty(configuration["JwtSettings:Key"]);

        services.AddOptions<JwtSettings>()
           .BindConfiguration("JwtSettings")
           .ValidateDataAnnotations()
           .ValidateOnStart();

        services.AddDbContext<MyAppIdentityContext>(
            options => options.UseSqlServer(
                configuration.GetConnectionString("MyDbContextIdentity")));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<MyAppIdentityContext>()
            .AddDefaultTokenProviders();

        services.AddTransient<IAppAuthenticationService, AppAuthenticationService>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!))
                };

                //o.Events = new JwtBearerEvents()
                //{
                //    OnAuthenticationFailed = c =>
                //        {
                //            c.NoResult();
                //            c.Response.StatusCode = StatusCodes.Status403Forbidden;
                //            c.Response.ContentType = "text/plain";

                //            var result = ApiResult.Error<int>(
                //                new List<FluentValidation.Results.ValidationFailure>
                //                {
                //                new FluentValidation.Results.ValidationFailure
                //                {
                //                    ErrorMessage = c.Exception.ToString(),
                //                    PropertyName = "*"
                //                }
                //                }
                //            );

                //            return c.Response.WriteAsync(JsonConvert.SerializeObject(result));
                //        },
                //    OnChallenge = context =>
                //    {
                //        context.HandleResponse();

                //        if (!context.Response.HasStarted)
                //        {
                //            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                //            context.Response.ContentType = "application/json";
                //        }

                //        var result = ApiResult.Error<int>(
                //          new List<FluentValidation.Results.ValidationFailure>
                //          {
                //                    new FluentValidation.Results.ValidationFailure
                //                    {
                //                        ErrorMessage = "Not authorized",
                //                        PropertyName = "*"
                //                    }
                //          }
                //        );

                //        return context.Response.WriteAsync(JsonConvert.SerializeObject(result));
                //    },
                //    OnForbidden = context =>
                //    {
                //        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                //        context.Response.ContentType = "application/json";

                //        var result = ApiResult.Error<int>(
                //            new List<FluentValidation.Results.ValidationFailure>
                //            {
                //                            new FluentValidation.Results.ValidationFailure
                //                            {
                //                                ErrorMessage = "Not authorized",
                //                                PropertyName = "*"
                //                            }
                //            }
                //  );

                //        return context.Response.WriteAsync(JsonConvert.SerializeObject(result));
                //    }
                //};

            });
    }
}
