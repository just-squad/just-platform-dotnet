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
    extension(IServiceCollection services)
    {
        public IServiceCollection AddJustPlatform(IConfiguration configuration,
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
                services.AddPlatformCors(options);
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

            services.AddControllers();
            // Регистрируем hosted service для debug-сервера
            services.AddHostedService<DebugEndpointsHostedService>();

            return services;
        }

        public IServiceCollection AddPlatformCors(PlatformOptions options)
        {
            services.AddCors(corsOptions =>
            {
                corsOptions.AddPolicy(PlatformCorsOptions.WellKnownPlatformCorsPolicies.AnyOrigins,
                    policyBuilder =>
                    {
                        policyBuilder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
                corsOptions.AddPolicy(PlatformCorsOptions.WellKnownPlatformCorsPolicies.DebugPortOrigins,
                    policyBuilder =>
                    {
                        var allowedOrigins = new List<string>()
                        {
                            // local
                            new UriBuilder()
                            {
                                Scheme = "http",
                                Host = "localhost",
                                Port = options.Ports.DebugPort
                            }.ToString(),
                            new UriBuilder()
                            {
                                Scheme = "https",
                                Host = "localhost",
                                Port = options.Ports.DebugPort
                            }.ToString(),
                            // debug host
                            new UriBuilder()
                            {
                                Scheme = "http",
                                Host = options.Ports.DebugHost,
                                Port = options.Ports.DebugPort
                            }.ToString(),
                            new UriBuilder()
                            {
                                Scheme = "http",
                                Host = options.Ports.DebugHost,
                                Port = options.Ports.DebugPort
                            }.ToString(),
                        };
                        if (!string.IsNullOrWhiteSpace(options.PublicUrl))
                        {
                            var uriBuilder = new UriBuilder(options.PublicUrl)
                            {
                                Scheme = "https",
                                Port = options.Ports.HttpPort
                            };
                            allowedOrigins.Add(uriBuilder.ToString());
                            uriBuilder.Scheme = "http";
                            allowedOrigins.Add(uriBuilder.ToString());
                        }

                        policyBuilder
                            .WithOrigins(allowedOrigins.ToArray())
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            return services;
        }
    }
}