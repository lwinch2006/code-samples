using System;
using System.Reflection;
using DbUp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DataAccess
{
    public interface IMigrator
    {
        void Run();
    }

    public class Migrator : IMigrator
    {
        private readonly IHostEnvironment _environment;
        private readonly ILogger<Migrator> _logger;
        private readonly string _connectionString; 
        
        public Migrator(IHostEnvironment environment, IConfiguration configuration, ILogger<Migrator> logger)
        {
            _environment = environment;
            _logger = logger;
            _connectionString = configuration["Data:ConnectionString"];            
        }         
        
        public void Run()
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