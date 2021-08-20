using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using AspNetCoreRateLimit;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using WebApplicationapi.Data;
using WebApplicationapi.Models;

namespace WebApplicationapi
{
  public static class ServiceExtensions
  {
    public static void ConfigureIdentity(this IServiceCollection services)
    {
      var builder = services.AddIdentityCore<ApiUser>(options => options.User.RequireUniqueEmail = true);

      builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
      builder.AddEntityFrameworkStores<DatabaseContext>()
        .AddDefaultTokenProviders();
    }

    public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
    {
      var jwtSetting = configuration.GetSection("JWT");

      services.AddAuthentication(options =>
        {
          options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
          options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSetting.GetSection("issuer").Value,
            IssuerSigningKey = new SymmetricSecurityKey(
              Encoding.UTF8.GetBytes(jwtSetting.GetSection("key").Value)
            ),
          };
        });
    }

    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
      app.UseExceptionHandler(builder =>
      {
        builder.Run(async context =>
        {
          context.Response.StatusCode = StatusCodes.Status500InternalServerError;
          context.Response.ContentType = "application/json";

          var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

          if (exceptionHandlerFeature != null)
          {
            Log.Error($"there is an error in {exceptionHandlerFeature.Error}");

            await context.Response.WriteAsync(new Error
              {
                StatusCode = context.Response.StatusCode,
                Message = "Internal server error,please try again later."
              }.ToString()
            );
          }
        });
      });
    }

    public static void ConfigureCaching(this IServiceCollection services)
    {
      services.AddResponseCaching();
      services.AddHttpCacheHeaders(options =>
      {
        options.MaxAge = 120;
        options.CacheLocation = CacheLocation.Private;
      }, options => { options.MustRevalidate = true; });
    }

    public static void ConfigureRateLimiter(this IServiceCollection services)
    {
      var rateLimitRules = new List<RateLimitRule>
      {
        new RateLimitRule
        {
          Endpoint = "*",
          Limit = 1,
          Period = "5s"
        }
      };

      services.Configure<IpRateLimitOptions>(options => { options.GeneralRules = rateLimitRules; });
      services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
      services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
      services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
      services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

    }
  }
}
