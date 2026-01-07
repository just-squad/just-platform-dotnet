namespace JustPlatform.Configuration.Services;

public interface IRemoteConfigurationService
{
    Task InitializeAsync(CancellationToken cancellationToken);
    Task ReloadAsync(CancellationToken cancellationToken);
}
