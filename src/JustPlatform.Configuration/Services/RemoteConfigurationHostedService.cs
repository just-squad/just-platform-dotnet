using Microsoft.Extensions.Hosting;

namespace JustPlatform.Configuration.Services;

public class RemoteConfigurationHostedService : BackgroundService
{
    private readonly IRemoteConfigurationService _service;

    public RemoteConfigurationHostedService(IRemoteConfigurationService service)
    {
        _service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _service.InitializeAsync(stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Заглушка
        }
    }
}
