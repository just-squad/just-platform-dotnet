namespace JustPlatform.Configuration.Models;

public class VaultSecretResponse
{
    public required VaultData Data { get; set; }
}

public class VaultData
{
    public required Dictionary<string, object> Data { get; set; }
    public string? Metadata { get; set; }
}
