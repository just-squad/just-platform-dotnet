using Dapper;
using JustPlatform.DataAccess.Npgsql.Models;
using JustPlatform.DataAccess.Npgsql.TypeHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace JustPlatform.DataAccess.Npgsql.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterDapperMapperTypes(this IServiceCollection services)
    {
        SqlMapper.RemoveTypeMap(typeof(DateTime));
        SqlMapper.RemoveTypeMap(typeof(DateTime?));

        SqlMapper.AddTypeHandler(typeof(DateTime?), new DateTimeTypeHandler());
        SqlMapper.AddTypeHandler(typeof(Xid), new XidTypeHandler());

        return services;
    }
}