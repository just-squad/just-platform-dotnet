using JustPlatform.Configuration.Models;

namespace JustPlatform.Configuration.Providers;

public interface IVaultProvider
{
    Task<VaultSecretResponse?> GetSecretV1Async(string path, CancellationToken cancellationToken);
    Task<VaultV2SecretResponse?> GetSecretV2Async(string path, CancellationToken cancellationToken);
}
