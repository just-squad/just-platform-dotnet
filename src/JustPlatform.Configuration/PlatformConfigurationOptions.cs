namespace JustPlatform.Configuration;

public class PlatformVaultOptions
{
    public const string SectionName = "Platform:Configuration:Vault";

    public bool IsEnabled { get; init; } = false;
    public string? VaultUrl { get; init; }
    public string? VaultToken { get; init; }
    public string? VaultPath { get; init; } = string.Empty;
    public int RefreshIntervalSeconds { get; init; } = 30;
}
