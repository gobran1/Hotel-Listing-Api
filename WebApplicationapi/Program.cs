using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace WebApplicationapi
{
  public class Program
  {
    public static void Main(string[] args)
    {
      Log.Logger = new LoggerConfiguration()
        .WriteTo.File(path: "/home/tiger/RiderProjects/WebApplicationapi/logs/log-.txt",
          outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:1j}{NewLine}{Exception}",
          rollingInterval: RollingInterval.Day,
          restrictedToMinimumLevel: LogEventLevel.Information
      ).CreateLogger();
      try
      {
        Log.Information("Application is starting");
        CreateHostBuilder(args).Build().Run();
      }
      catch (Exception e)
      {
        Log.Fatal(e, "Application failed to start");
      }
      finally
      {
        Log.CloseAndFlush();
      }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
  }
}
