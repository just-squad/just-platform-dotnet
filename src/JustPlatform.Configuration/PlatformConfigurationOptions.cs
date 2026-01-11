namespace JustPlatform.Configuration;

/// <summary>
/// Configuration for connecting to a Vault server or another compatible server.
/// </summary>
/// <remarks>
/// Section name in configurations "PlatformOptions:Vault"
/// </remarks>
public class PlatformVaultOptions
{
    public const string SectionName = "PlatformOptions:Vault";

    /// <summary>
    /// Is Vault enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = false;

    /// <summary>
    /// Vault connection URL.
    /// </summary>
    public string? VaultUrl { get; set; }

    /// <summary>
    /// Vault connection token.
    /// </summary>
    public string? VaultToken { get; set; }

    /// <summary>
    /// Vault project path.
    /// </summary>
    public string? VaultPath { get; set; } = string.Empty;

    /// <summary>
    /// Whether to use protocol v2.
    /// </summary>
    public bool UseKvV2 { get; set; } = true;

    /// <summary>
    /// Configuration refresh interval in seconds.
    /// </summary>
    public int RefreshIntervalSeconds { get; set; } = 30;
}
