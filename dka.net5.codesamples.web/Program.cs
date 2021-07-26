using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace dka.net5.codesamples.web
{
    public class Program
    {
        private static string _environmentName;
        private static IConfiguration _configuration; 
        
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            host.Run();        }

        private static IHost BuildWebHost(string[] args)
        {
            BuildParameters();
            BuildConfiguration();

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
    }
}