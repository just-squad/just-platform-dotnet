using JustPlatform.Configuration;

namespace JustPlatform.Hosting.Configuration;

public class PlatformOptions
{
    public const string SectionName = "PlatformOptions";

    public bool EnableHealthChecks { get; init; } = true;
    public bool EnableMetrics { get; init; } = true;
    public bool EnableSerilog { get; init; } = true;
    public bool EnableSwagger { get; init; } = false;


    // Порты
    public PlatformPortsOptions Ports { get; init; } = new();

    // Vault connection
    public PlatformVaultOptions VaultOptions { get; init; } = new();

    // Переопределение конфигураций
    public Dictionary<string, string?>? OverrideConfiguration { get; set; } = new();
}

public class PlatformPortsOptions
{
    public int HttpPort { get; init; } = 80;
    public int GrpcPort { get; init; } = 82;
    public int DebugPort { get; init; } = 84;
}
