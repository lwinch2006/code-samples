using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var urls = _configuration.GetSection("host:urls")
                        .AsEnumerable()
                        .Select(t => t.Value)
                        .Where(t => t != null)
                        .ToArray();
                        
                    webBuilder.UseEnvironment(_environmentName);
                    webBuilder.UseConfiguration(_configuration);
                    webBuilder.UseSerilog();
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(urls);
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
            var elasticSearchUrl = _configuration["ElasticSearch:Url"];
            var indexFormat = _configuration["ElasticSearch:Index"];
            
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticSearchUrl))
                {
                    AutoRegisterTemplate = true,
                    IndexFormat = indexFormat
                })
                .CreateLogger();            
        }
    }
}