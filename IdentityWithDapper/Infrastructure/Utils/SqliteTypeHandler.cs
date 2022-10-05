using System;
using System.Data;
using Dapper;

namespace IdentityWithDapper.Infrastructure.Utils
{
    public abstract class SqliteTypeHandler<T> : SqlMapper.TypeHandler<T>
    {
        // Parameters are converted by Microsoft.Data.Sqlite
        public override void SetValue(IDbDataParameter parameter, T value)
            => parameter.Value = value.ToString();
    }

    public class DateTimeOffsetHandler : SqliteTypeHandler<DateTimeOffset>
    {
        public override DateTimeOffset Parse(object value)
        {
            return DateTimeOffset.Parse((string)value); 
        }
    }

    public class GuidHandler : SqliteTypeHandler<Guid>
    {
        public override Guid Parse(object value)
        {
            return Guid.Parse((string)value); 
        }
    }

    public class TimeSpanHandler : SqliteTypeHandler<TimeSpan>
    {
        public override TimeSpan Parse(object value)
        {
            return TimeSpan.Parse((string)value); 
        }
    }
}