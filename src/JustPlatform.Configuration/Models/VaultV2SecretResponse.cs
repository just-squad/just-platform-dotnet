namespace JustPlatform.Configuration.Models;

public class VaultV2SecretResponse
{
    public required VaultV2Data Data { get; set; }
}

public class VaultV2Data
{
    public required VaultV2Metadata Metadata { get; set; }
    public required Dictionary<string, object> Data { get; set; }
}

public class VaultV2Metadata
{
    public string? CreatedTime { get; set; }
    public string? DeletionTime { get; set; }
    public bool Destroyed { get; set; }
    public string? Version { get; set; }
}
