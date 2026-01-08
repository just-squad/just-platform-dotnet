namespace JustPlatform.Configuration.Services;

public interface IRemoteConfigurationService
{
    Task InitializeAsync(CancellationToken ct);
    Task ReloadAsync(CancellationToken ct);
}
