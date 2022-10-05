using System;
using System.Data.Common;
using System.Data.SqlClient;
using Dapper;
using Infrastructure.Utils.Constants;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Utils
{
    public interface IDbConnectionFactory
    {
        DbConnection GetDbConnection();
    }

    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;
        private readonly InfrastructureConstants.DbTypes _dbType;
    
        public DbConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration[InfrastructureConstants.DbConnectionConfigParamName];
            _dbType = Enum.Parse<InfrastructureConstants.DbTypes>(configuration[InfrastructureConstants.DbTypeConfigParamName]);
        }

        public DbConnection GetDbConnection()
        {
            switch (_dbType)
            {
                case InfrastructureConstants.DbTypes.Sqlite:
                    
                    SqlMapper.RemoveTypeMap(typeof(Guid));
                    SqlMapper.RemoveTypeMap(typeof(Guid?));
                    
                    SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());
                    SqlMapper.AddTypeHandler(new GuidHandler());
                    SqlMapper.AddTypeHandler(new TimeSpanHandler());
                    
                    return new SqliteConnection(_connectionString);
            
                case InfrastructureConstants.DbTypes.Mssql:
                    return new SqlConnection(_connectionString);
            
                default:
                    return null;
            }
        }
    }
}