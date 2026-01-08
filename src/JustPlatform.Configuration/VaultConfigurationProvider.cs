using Microsoft.Extensions.Configuration;

namespace JustPlatform.Configuration;

public class VaultConfigurationProvider : ConfigurationProvider
{
    private readonly VaultConfigurationSource _source;

    public VaultConfigurationProvider(VaultConfigurationSource source)
    {
        _source = source;
    }

    public override void Load()
    {
        // Загружаем данные из Vault и обновляем Data
        // Data["Key"] = "Value";
    }
}
