using JustPlatform.Configuration.Providers;
using JustPlatform.Configuration.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JustPlatform.Configuration.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJustPlatformVaultConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        // You can create a temporary logger before the app is built
        using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.AddConsole());
        ILogger logger = loggerFactory.CreateLogger("AddJustPlatformVaultConfiguration");

        var platformVaultOptionsValue = configuration.GetSection(PlatformVaultOptions.SectionName)
                                .Get<PlatformVaultOptions>() ?? new PlatformVaultOptions();
        if (platformVaultOptionsValue is null)
        {
            logger.LogInformation("Configuration for Vault connection is not provided. Vault is disabled.");
            return services;
        }
        if (platformVaultOptionsValue.IsEnabled == false)
        {
            logger.LogInformation("Vault is disabled by configurations.");
            return services;
        }
        if (string.IsNullOrWhiteSpace(platformVaultOptionsValue.VaultUrl))
        {
            logger.LogError("Vault connection is enabled in configurations. But VaultUrl not provided. Vault NOT REGISTERED");
            return services;
        }
        if (string.IsNullOrWhiteSpace(platformVaultOptionsValue.VaultToken))
        {
            logger.LogError("Vault connection is enabled in configurations. But VaultToken not provided. Vault NOT REGISTERED");
            return services;
        }
        if (string.IsNullOrWhiteSpace(platformVaultOptionsValue.VaultPath))
        {
            logger.LogError("Vault connection is enabled in configurations. But VaultPath fot service not provided. Vault NOT REGISTERED");
            return services;
        }

        services.Configure<PlatformVaultOptions>(configuration.GetSection(PlatformVaultOptions.SectionName));

        // Добавляем mutable provider
        var refreshInterval = TimeSpan.FromSeconds(platformVaultOptionsValue.RefreshIntervalSeconds);
        var provider = new MutableConfigurationProvider(refreshInterval);
        services.AddSingleton<IConfigurationProvider>(provider);
        services.AddSingleton(provider);

        services.AddHttpClient<IVaultProvider, VaultHttpProvider>((provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<PlatformVaultOptions>>().Value;
            if (!string.IsNullOrEmpty(options.VaultUrl))
            {
                client.BaseAddress = new Uri(options.VaultUrl);
            }

            if (!string.IsNullOrEmpty(options.VaultToken))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", options.VaultToken);
            }
        });

        // Внедряем локальную конфигурацию для fallback
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<IRemoteConfigurationService, RemoteConfigurationService>();
        services.AddHostedService<RemoteConfigurationHostedService>();

        return services;
    }
}
