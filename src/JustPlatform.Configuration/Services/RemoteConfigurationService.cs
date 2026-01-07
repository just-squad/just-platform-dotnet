using JustPlatform.Configuration.Models;
using JustPlatform.Configuration.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JustPlatform.Configuration.Services;

public class RemoteConfigurationService : IRemoteConfigurationService, IDisposable
{
    private readonly MutableConfigurationProvider _provider;
    private readonly IOptionsMonitor<PlatformVaultOptions> _options;
    private readonly IVaultProvider _vaultProvider;
    private readonly ILogger<RemoteConfigurationService> _logger;
    private Timer? _timer;

    public RemoteConfigurationService(
        MutableConfigurationProvider provider,
        IOptionsMonitor<PlatformVaultOptions> options,
        IVaultProvider vaultProvider,
        ILogger<RemoteConfigurationService> logger)
    {
        _provider = provider;
        _options = options;
        _vaultProvider = vaultProvider;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await ReloadAsync(cancellationToken);
        var interval = TimeSpan.FromSeconds(_options.CurrentValue.RefreshIntervalSeconds);
        _timer = new Timer(async _ => await ReloadAsync(CancellationToken.None), null, interval, interval);
    }

    public async Task ReloadAsync(CancellationToken cancellationToken = default)
    {
        var opts = _options.CurrentValue;
        if (!opts.IsEnabled || string.IsNullOrWhiteSpace(opts.VaultUrl) || string.IsNullOrWhiteSpace(opts.VaultToken))
        {
            _logger.LogDebug("Vault configuration is disabled or credentials are missing.");
            return;
        }

        if (string.IsNullOrWhiteSpace(opts.VaultPath))
        {
            _logger.LogDebug("Vault path is missing when Vault connection is enabled. Skip process.");
            return;
        }

        try
        {
            var newData = new Dictionary<string, string?>();
            if (opts.UseKvV2)
            {
                var secretsV2 = await _vaultProvider.GetSecretV2Async(opts.VaultPath, cancellationToken);
                if (secretsV2 is not null)
                {
                    foreach (var secret in secretsV2.Data.Data)
                    {
                        newData[$"Vault:{secret.Key}"] = secret.Value?.ToString();
                    }
                }
            }
            else
            {
                var secretsV1 = await _vaultProvider.GetSecretV1Async(opts.VaultPath, cancellationToken);
                if (secretsV1 is not null)
                {
                    foreach (var secret in secretsV1.Data.Data)
                    {
                        newData[$"Vault:{secret.Key}"] = secret.Value?.ToString();
                    }
                }
            }
            if (newData.Count > 0)
            {
                await _provider.UpdateDataAsync(newData);
                _logger.LogInformation("Remote configuration reloaded from Vault.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reload remote configuration from Vault.");
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
        _provider?.Dispose();
    }
}