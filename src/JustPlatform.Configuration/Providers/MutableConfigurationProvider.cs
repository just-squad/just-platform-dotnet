using Microsoft.Extensions.Configuration;

namespace JustPlatform.Configuration.Providers;

public class MutableConfigurationProvider : ConfigurationProvider, IDisposable
{
    private readonly Timer _timer;
    private readonly TimeSpan _refreshInterval;

    public MutableConfigurationProvider(TimeSpan refreshInterval)
    {
        _refreshInterval = refreshInterval;
        _timer = new Timer(OnTimerCallback, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
    }

    public async Task UpdateDataAsync(IDictionary<string, string?> newData)
    {
        Data = new Dictionary<string, string?>(newData, StringComparer.OrdinalIgnoreCase);
        OnReload();
    }

    private async void OnTimerCallback(object? state)
    {
        // Здесь можно вызвать обновление, если нужно по таймеру
        // Пока заглушка
    }

    public void StartTimer() => _timer.Change(_refreshInterval, _refreshInterval);

    public void Dispose() => _timer.Dispose();
}
