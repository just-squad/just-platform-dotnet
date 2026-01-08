using JustPlatform.Configuration.Models;
using JustPlatform.Configuration.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JustPlatform.Configuration.Services;

public class RemoteConfigurationService : IRemoteConfigurationService, IDisposable
{
    private readonly MutableConfigurationProvider _provider;
    private readonly IConfiguration _localConfiguration; // <-- Добавляем локальную конфигурацию
    private readonly IOptionsMonitor<PlatformVaultOptions> _options;
    private readonly IVaultProvider _vaultProvider;
    private readonly ILogger<RemoteConfigurationService> _logger;
    private Timer? _timer;
    private bool _isUsingFallback = false;

    public RemoteConfigurationService(
        MutableConfigurationProvider provider,
        IConfiguration localConfiguration, // <-- Внедряем
        IOptionsMonitor<PlatformVaultOptions> options,
        IVaultProvider vaultProvider,
        ILogger<RemoteConfigurationService> logger)
    {
        _provider = provider;
        _localConfiguration = localConfiguration;
        _options = options;
        _vaultProvider = vaultProvider;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        await ReloadAsync(ct);
        var interval = TimeSpan.FromSeconds(_options.CurrentValue.RefreshIntervalSeconds);
        _timer = new Timer(async _ => await ReloadAsync(CancellationToken.None), null, interval, interval);
    }

    public async Task ReloadAsync(CancellationToken ct = default)
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
            IHasVaultData? secrets = opts.UseKvV2
                ? await GetVaultDataFromV2(opts.VaultPath, ct)
                : await GetVaultDataFromV1(opts.VaultPath, ct);

            if (secrets != null)
            {
                var newData = new Dictionary<string, string?>();

                // Для KV v2
                if (secrets is VaultV2SecretResponse v2Secrets)
                {
                    foreach (var secret in v2Secrets.Data.Data)
                    {
                        newData[$"Vault:{secret.Key}"] = secret.Value?.ToString();
                    }
                }
                // Для KV v1
                else if (secrets is VaultSecretResponse v1Secrets)
                {
                    foreach (var secret in v1Secrets.Data.Data)
                    {
                        newData[$"Vault:{secret.Key}"] = secret.Value?.ToString();
                    }
                }

                await _provider.UpdateDataAsync(newData);
                
                if (_isUsingFallback)
                {
                    _logger.LogInformation("Successfully reconnected to Vault. Fallback mode disabled.");
                    _isUsingFallback = false;
                }
                else
                {
                    _logger.LogInformation("Remote configuration reloaded from Vault.");
                }
            }
            else
            {
                await UseFallbackConfiguration();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reload remote configuration from Vault. Using fallback.");
            await UseFallbackConfiguration();
        }
    }

    private async Task UseFallbackConfiguration()
    {
        if (!_isUsingFallback)
        {
            _logger.LogWarning("Vault is unavailable. Switching to fallback configuration.");
            _isUsingFallback = true;
        }

        // Используем локальные значения как fallback
        var fallbackData = new Dictionary<string, string?>();

        // Копируем только те ключи, которые начинаются с "Vault:" из локальной конфигурации
        foreach (var kvp in _localConfiguration.AsEnumerable())
        {
            if (kvp.Key.StartsWith("Vault:", StringComparison.OrdinalIgnoreCase))
            {
                fallbackData[kvp.Key] = kvp.Value;
            }
        }

        await _provider.UpdateDataAsync(fallbackData);
    }

    private async Task<IHasVaultData?> GetVaultDataFromV1(string vaultPath, CancellationToken ct)
    {
        var response = await _vaultProvider.GetSecretV1Async(vaultPath, ct);
        return response?.Data;
    }

    private async Task<IHasVaultData?> GetVaultDataFromV2(string vaultPath, CancellationToken ct)
    {
        var response = await _vaultProvider.GetSecretV2Async(vaultPath, ct);
        return response?.Data;
    } 

    public void Dispose()
    {
        _timer?.Dispose();
        _provider?.Dispose();
    }
}