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

namespace JustPlatform.Hosting.Extensions;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddJustPlatform(this WebApplicationBuilder builder,
        Action<PlatformOptions>? configureOptions = null)
    {
        builder.Services.AddJustPlatform(
            builder.Environment,
            builder.Configuration,
            configureOptions
        );


        return builder;
    }

    public static IServiceCollection AddJustPlatform(this IServiceCollection services,
            IWebHostEnvironment webHostEnvironment,
            IConfigurationBuilder configurationManager,
            Action<PlatformOptions>? configureOptions = null)
    {
        var options = new PlatformOptions();
        // 1. Строим конфигурацию
        var cfgBuilder = configurationManager.AddJustPlatformConfigurationSources(webHostEnvironment.EnvironmentName);
        var cfg = cfgBuilder.Build();
        var optionsFromCfg = cfg.GetSection(PlatformOptions.SectionName).Get<PlatformOptions>();
        if(optionsFromCfg is not null)
        {
            options = optionsFromCfg;
        }
        
        // 2. Если пользователь задал переменные явно, то нужно переписать значения
        configureOptions?.Invoke(options);

        
        // 3. Vault/OpenBao (опционально, если включено)
        if (options.VaultOptions.IsEnabled)
        {
            services.AddJustPlatformVaultConfiguration(configurationManager.Build());
        }

        // 2. Применяем переопределения из PlatformOptions
        if (options.OverrideConfiguration is not null)
        {
            var notNullConfigurations = options.OverrideConfiguration.Where(kvp => kvp.Value != null).ToDictionary();
            configurationManager.AddInMemoryCollection(notNullConfigurations);
        }

        if (options.EnableHealthChecks)
        {
            services.AddHealthChecks()
                    .AddCheck<LivenessCheck>("liveness", HealthStatus.Degraded, new[] { "live" })
                    .AddCheck<ReadinessCheck>("readiness", HealthStatus.Degraded, new[] { "ready" });
        }

        if (options.EnableSerilog)
        {
            Logging.SerilogConfiguration.ConfigureSerilog();
            services.AddSerilog(Log.Logger, dispose: true);
        }

        if (options.EnableMetrics)
        {
            services.AddOpenTelemetry();
        }

        // Регистрируем hosted service для debug-сервера
        services.AddHostedService<DebugEndpointsHostedService>();

        return services;
    }
}
