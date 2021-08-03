using System;
using System.Reflection;
using DbUp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dka.Net5.IdentityWithDapper.Infrastructure
{
    public interface IInfrastructure
    {
        void Setup();
    }
    
    public class Infrastructure : IInfrastructure
    {
        private readonly IHostEnvironment _environment;
        private readonly ILogger<Infrastructure> _logger;
        private readonly string _connectionString;        
        
        public Infrastructure(IHostEnvironment environment, IConfiguration configuration, ILogger<Infrastructure> logger)
        {
            _environment = environment;
            _logger = logger;
            _connectionString = configuration["Data:ConnectionString"];            
        }        
        
        public void Setup()
        {
            RunDbMigrations();
            
            if (_environment.IsDevelopment())
            {
                // Seed scripts for dev env.
                RunDbMigrationsInDevelopmentEnvironment();
            }            
        }
        
        private void RunDbMigrations()
        {
            EnsureDatabase.For.SqlDatabase(_connectionString);
            
            var upgrader =
                DeployChanges.To
                    .SqlDatabase(_connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), scriptName => !scriptName.Contains("seed-dev-data", StringComparison.OrdinalIgnoreCase))
                    .LogToConsole()
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                throw new Exception("Database migrations run failure", result.Error);
            }

            _logger.LogInformation("Database migrations run success");
        }
        
        private void RunDbMigrationsInDevelopmentEnvironment()
        {
            EnsureDatabase.For.SqlDatabase(_connectionString);
            
            var upgrader =
                DeployChanges.To
                    .SqlDatabase(_connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), scriptName => scriptName.Contains("seed-dev-data", StringComparison.OrdinalIgnoreCase))
                    .LogToConsole()
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                throw new Exception("Database migrations (Development environment) run failure", result.Error);
            }

            _logger.LogInformation("Database migrations (Development environment) run success");            
        }             
    }
}