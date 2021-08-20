using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using WebApplicationapi.Configurations;
using WebApplicationapi.Data;
using WebApplicationapi.IRepository;
using WebApplicationapi.Repository;
using WebApplicationapi.Services;

namespace WebApplicationapi
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<DatabaseContext>(options =>
        options.UseSqlServer(Configuration.GetConnectionString("sqlConnection"))
      );

      //setup Rate Limit config
      services.AddMemoryCache();
      services.ConfigureRateLimiter();
      services.AddHttpContextAccessor();

      //setup Auth config
      services.AddAuthentication();
      services.ConfigureIdentity();
      services.ConfigureJWT(Configuration);

      //setup cache config
      services.ConfigureCaching();

      //setup Cors config
      services.AddCors(options =>
      {
        options.AddPolicy("AllowAll", builder =>
          builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
        );
      });

      //Add Mapper implementation class
      services.AddAutoMapper(typeof(MapperInitializer));

      //register authManager
      services.AddScoped<IAuthManager, AuthManager>();

      //register an unit of work for entity framework repos
      services.AddTransient<IUnitOfWork, UnitOfWork>();

      services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "HotelListing", Version = "v1"}); });

      services.AddControllers(
        config =>
        {
          config.CacheProfiles.Add("120SecDuration", new CacheProfile
          {
            Duration = 120,
          });
        }
      ).AddNewtonsoftJson(options =>
        {
          options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        }
      );
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseSwagger();
      app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplicationapi v1"));

      app.ConfigureExceptionHandler();

      app.UseHttpsRedirection();

      app.UseCors("AllowAll");

      app.UseResponseCaching();
      app.UseHttpCacheHeaders();

      app.UseIpRateLimiting();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
  }
}
