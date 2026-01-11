using System;
using JustPlatform.Hosting.Configuration;
using JustPlatform.Hosting.DebugServer;
using JustPlatform.Hosting.HealthCheck;
using JustPlatform.Configuration.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using JustPlatform.Hosting.Metrics;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Routing;

namespace JustPlatform.Hosting.Extensions;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddJustPlatform(this WebApplicationBuilder builder,
        Action<PlatformOptions>? configureOptions = null,
        Action<IServiceCollection>? addServices = null,
        Action<IApplicationBuilder>? configurePipeline = null,
        Action<IEndpointRouteBuilder>? configureEndpoints = null
        )
    {
        // 1. Resolve options
        var options = new PlatformOptions();
        var optionsFromCfg = builder.Configuration.GetSection(PlatformOptions.SectionName).Get<PlatformOptions>();
        if (optionsFromCfg is not null)
        {
            options = optionsFromCfg;
        }
        configureOptions?.Invoke(options);

        // 2. Configure Kestrel
        builder.WebHost.ConfigureKestrel(kestrelOptions =>
        {
            kestrelOptions.ListenAnyIP(options.Ports.HttpPort, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
            });

            kestrelOptions.ListenAnyIP(options.Ports.GrpcPort, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });
        });

        // 3. Add platform services
        builder.Services.AddJustPlatform(builder.Configuration, options);

        // 4. Add custom user services
        addServices?.Invoke(builder.Services);

        // 5. Register extensibility delegates for middleware
        var extensibilityOptions = new JustPlatformExtensibilityOptions
        {
            ConfigurePipeline = configurePipeline,
            ConfigureEndpoints = configureEndpoints
        };
        builder.Services.AddSingleton(extensibilityOptions);


        return builder;
    }

    public static IServiceCollection AddJustPlatform(this IServiceCollection services, IConfiguration configuration, PlatformOptions options)
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
                    .AddCheck<LivenessCheck>("liveness", HealthStatus.Degraded, new[] { "live" })
                    .AddCheck<ReadinessCheck>("readiness", HealthStatus.Degraded, new[] { "ready" });
        }

        if (options.EnableSwagger)
        {
            services.AddControllers();
            services.AddSwaggerGen();
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
