using System.Text.Json.Nodes;
using JustPlatform.Hosting.Configuration;
using JustPlatform.Hosting.DebugServer;
using JustPlatform.Hosting.HealthCheck;
using JustPlatform.Configuration.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Microsoft.Extensions.Configuration;
using JustPlatform.Hosting.Metrics;
using JustPlatform.Hosting.Swagger;
using Microsoft.OpenApi;

namespace JustPlatform.Hosting.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJustPlatform(this IServiceCollection services,
        IConfiguration configuration,
        PlatformOptions options)
    {
        // Register options for middleware to use
        services.AddSingleton(options);

        // Vault/OpenBao (опционально, если включено)
        if (options.Vault.IsEnabled)
        {
            services.AddJustPlatformVaultConfiguration(configuration);
        }

        if (options.EnableHealthChecks)
        {
            services.AddHealthChecks()
                .AddCheck<LivenessCheck>("liveness", HealthStatus.Degraded, ["live"])
                .AddCheck<ReadinessCheck>("readiness", HealthStatus.Degraded, ["ready"]);
        }

        if (options.EnableSwagger)
        {
            services.AddCors(corsOptions =>
            {
                corsOptions.AddPolicy(options.Cors.PolicyName, policyBuilder =>
                {
                    policyBuilder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.DocumentFilter<ServersDocumentFilter>();
                c.CustomSchemaIds(x => x.FullName?.Replace("+", ".", StringComparison.Ordinal));

                c.MapType<TimeSpan>(() => new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Example = JsonValue.Create("0.00:00:00"),
                    Format = "dd:HH:mm:ss"
                });
                c.MapType<TimeSpan?>(() => new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Example = JsonValue.Create("0.00:00:00"),
                    Format = "dd:HH:mm:ss",
                });
            });
        }

        if (options.EnableSerilog)
        {
            Logging.SerilogConfiguration.ConfigureSerilog();
            services.AddSerilog(Log.Logger, dispose: true);
        }

        if (options.EnableMetrics)
        {
            services.AddPlatformOpenTelemetry(options);
        }

        // Регистрируем hosted service для debug-сервера
        services.AddHostedService<DebugEndpointsHostedService>();

        return services;
    }
}