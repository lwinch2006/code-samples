using System;
using System.Reflection;
using DbUp;
using DbUp.Builder;
using Infrastructure.Utils.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure
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
        private readonly InfrastructureConstants.DbTypes _dbType;
        
        public Infrastructure(IHostEnvironment environment, IConfiguration configuration, ILogger<Infrastructure> logger)
        {
            _environment = environment;
            _logger = logger;
            _connectionString = configuration[InfrastructureConstants.DbConnectionConfigParamName];
            _dbType = Enum.Parse<InfrastructureConstants.DbTypes>(configuration[InfrastructureConstants.DbTypeConfigParamName]);
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
            UpgradeEngineBuilder upgradeBuilder;
            
            switch (_dbType)
            {
                case InfrastructureConstants.DbTypes.Sqlite:
                    upgradeBuilder = DeployChanges.To.SQLiteDatabase(_connectionString);
                    break;
                
                case InfrastructureConstants.DbTypes.Mssql:
                default:
                    EnsureDatabase.For.SqlDatabase(_connectionString);
                    upgradeBuilder = DeployChanges.To.SqlDatabase(_connectionString);
                    break;                    
            }
            
            var upgrader = upgradeBuilder
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
            UpgradeEngineBuilder upgradeBuilder;
            
            switch (_dbType)
            {
                case InfrastructureConstants.DbTypes.Sqlite:
                    upgradeBuilder = DeployChanges.To.SQLiteDatabase(_connectionString);
                    break;
                
                case InfrastructureConstants.DbTypes.Mssql:
                default:
                    EnsureDatabase.For.SqlDatabase(_connectionString);
                    upgradeBuilder = DeployChanges.To.SqlDatabase(_connectionString);
                    break;                    
            }            
            
            var upgrader = upgradeBuilder
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