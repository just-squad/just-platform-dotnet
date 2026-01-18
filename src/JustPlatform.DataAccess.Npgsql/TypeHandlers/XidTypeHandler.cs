using System.Data;
using Dapper;
using JustPlatform.DataAccess.Npgsql.Models;
using Npgsql;
using NpgsqlTypes;

namespace JustPlatform.DataAccess.Npgsql.TypeHandlers;

internal sealed class XidTypeHandler : SqlMapper.TypeHandler<Xid>
{
    public override void SetValue(IDbDataParameter parameter, Xid value)
    {
        parameter.Value = value.Value;
        if (parameter is NpgsqlParameter npgsqlParameter)
        {
            npgsqlParameter.NpgsqlDbType = NpgsqlDbType.Xid;
        }
    }

    public override Xid Parse(object value) =>
        value == DBNull.Value
            ? default
            : new Xid((uint)value);
}