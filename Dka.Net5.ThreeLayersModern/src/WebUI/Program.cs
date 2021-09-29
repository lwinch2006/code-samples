using System;
using System.IO;
using System.Linq;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using WebUI.Utils.Extensions;

namespace WebUI
{
    public class Program
    {
        private static string _environmentName;
        private static IConfiguration _configuration;        
        
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            var logger = host.Services.GetService<ILogger<Program>>();
            
            logger.LogInformation("Host started {DateTime} UTC", DateTime.UtcNow);
            host.Run();
            logger.LogInformation("Host finished {DateTime} UTC", DateTime.UtcNow);
        }

        private static IHost BuildWebHost(string[] args)
        {
            BuildParameters();
            BuildConfiguration();
            BuildLogger();

            var webHost = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((builderContext, configBuilder) =>
                {
                    if (builderContext.HostingEnvironment.IsProduction())
                    {
                        var config = configBuilder.Build();
                        var secretClient = new SecretClient(
                            new Uri($"https://{config["KeyVault:Name"]}.vault.azure.net/"),
                            new DefaultAzureCredential());

                        configBuilder.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseEnvironment(_environmentName);
                    webBuilder.UseConfiguration(_configuration);
                    webBuilder.UseSerilog();
                    webBuilder.UseStartup<Startup>();

                    if (_environmentName.IsDevelopment())
                    {
                        var urls = _configuration.GetSection("host:urls")
                            .AsEnumerable()
                            .Select(t => t.Value)
                            .Where(t => t != null)
                            .ToArray();                        
                        
                        webBuilder.UseUrls(urls);
                    }
                })
                .Build();

            return webHost;
        }        

        private static void BuildParameters()
        {
            _environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        }        
        
        private static void BuildConfiguration()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{_environmentName}.json", optional: true, reloadOnChange: true)
                .Build();
        }

        private static void BuildLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
                .Enrich.WithProperty("Environment", _environmentName)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .CreateLogger();            
        }
    }
}