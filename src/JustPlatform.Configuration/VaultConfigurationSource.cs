using Microsoft.Extensions.Configuration;

namespace JustPlatform.Configuration;

public class VaultConfigurationSource : IConfigurationSource
{
    public string? VaultUrl { get; set; }
    public string? VaultToken { get; set; }

    public IConfigurationProvider Build(IConfigurationBuilder builder) => new VaultConfigurationProvider(this);
}
