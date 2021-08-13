namespace Dka.Net5.IdentityWithDapper.Infrastructure.Utils.Constants
{
    public class InfrastructureConstants
    {
        public const string DbConnectionConfigParamName = "Data:ConnectionString";
        public const string DbTypeConfigParamName = "Data:DbType";
        
        public enum DbTypes
        {
            Mssql = 1,
            Sqlite = 2
        }        
    }
}