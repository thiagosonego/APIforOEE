using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OEE.Infrastructure.Dapper.Registrations;
using Serilog;
using System;
using System.Threading.Tasks;

namespace OEE.API
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;

        public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static async Task Main(string[] args)
        {
            var configuration = GetConfiguration();

            Log.Logger = CreateSerilogLogger(configuration);

            DapperRegistrations.RegisterMapper();

            var webHost = CreateHostBuilder(configuration, args).Build();

            await webHost.RunAsync();
        }

        public static IWebHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseConfiguration(configuration)
                .UseIISIntegration();

        private static ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("ApplicationContext", AppName)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        private static IConfiguration GetConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory);

            if (string.IsNullOrWhiteSpace(environment))
            {
                builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            }
            else
            {
                builder.AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true);
            }

            var configuration = builder.Build();

            return configuration;
        }
    }
}
