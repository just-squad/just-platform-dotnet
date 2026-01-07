using JustPlatform.Configuration.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JustPlatform.Configuration.Services;

public class RemoteConfigurationService : IRemoteConfigurationService, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly IOptionsMonitor<PlatformVaultOptions> _options;
    private readonly IVaultProvider _vaultProvider;
    private readonly ILogger<RemoteConfigurationService> _logger;
    private Timer? _timer;

    public RemoteConfigurationService(
        IConfiguration configuration,
        IOptionsMonitor<PlatformVaultOptions> options,
        IVaultProvider vaultClient,
        ILogger<RemoteConfigurationService> logger)
    {
        _configuration = configuration;
        _options = options;
        _vaultProvider = vaultClient;
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
        if (!opts.IsEnabled || string.IsNullOrEmpty(opts.VaultUrl) || string.IsNullOrEmpty(opts.VaultToken))
        {
            _logger.LogDebug("Vault configuration is disabled or credentials are missing.");
            return;
        }

        try
        {
            var secrets = await _vaultProvider.GetSecretV1Async(opts.VaultPath, cancellationToken);

            if (secrets?.Data?.Data != null)
            {
                foreach (var secret in secrets.Data.Data)
                {
                    var key = secret.Key;
                    var value = secret.Value?.ToString();
                    (_configuration as IConfigurationRoot)?.GetReloadToken()?.RegisterChangeCallback(_ => { }, null);
                    // В .NET 8 можно использовать MutableConfiguration
                    // Пока просто логируем
                    _logger.LogInformation("Loaded secret: {Key} = {Value}", key, value);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reload remote configuration from Vault.");
        }
    }

    public void Dispose() => _timer?.Dispose();
}
