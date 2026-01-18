using System.Data;
using Dapper;
using Npgsql;

namespace JustPlatform.DataAccess.Npgsql.TypeHandlers;

internal sealed class DateTimeTypeHandler : SqlMapper.TypeHandler<DateTime>
{
    public override void SetValue(
        IDbDataParameter parameter,
        DateTime value)
    {
        if (parameter is NpgsqlParameter npgsqlParameter)
        {
            npgsqlParameter.NpgsqlValue = value;
        }
        else
        {
            parameter.Value = value;
        }
    }

    public override DateTime Parse(
        object value
    ) => value switch
    {
        DateTime time => DateTime.SpecifyKind(time, DateTimeKind.Utc),
        DateTimeOffset offset => offset.Date.ToUniversalTime(),
        _ => throw new InvalidOperationException("Must be DateTime or DateTimeOffset object to be mapped.")
    };
}