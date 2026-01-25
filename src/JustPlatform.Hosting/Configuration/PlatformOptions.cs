using JustPlatform.Configuration;

namespace JustPlatform.Hosting.Configuration;

/// <summary>
/// Set of just platform options
/// </summary>
/// <remarks>
/// Section name in configurations "PlatformOptions"
/// </remarks>
public class PlatformOptions
{
    public const string SectionName = "PlatformOptions";

    public bool EnableHealthChecks { get; set; } = true;
    public bool EnableMetrics { get; set; } = true;
    public bool EnableSerilog { get; set; } = true;
    public bool EnableSwagger { get; set; } = false;
    public string? PublicUrl { get; set; }

    /// <summary>
    /// Connection ports
    /// </summary>
    public PlatformPortsOptions Ports { get; set; } = new();

    /// <summary>
    /// Vault connection
    /// </summary>
    public PlatformVaultOptions Vault { get; set; } = new();

    /// <summary>
    /// Cors
    /// </summary>
    public PlatformCorsOptions Cors { get; set; } = new();

    /// <summary>
    /// Override configurations
    /// </summary>
    public Dictionary<string, string?>? OverrideConfiguration { get; set; } = new();
}

public class PlatformCorsOptions
{
    /// <summary>
    /// Название для политики CORS
    /// </summary>
    /// <remarks>Возможные варианты</remarks>
    public string PolicyName { get; set; } = WellKnownPlatformCorsPolicies.AnyOrigins;

    public static class WellKnownPlatformCorsPolicies
    {
        public const string AnyOrigins = "AnyOrigins";
        public const string DebugPortOrigins = "DebugPortOrigins";
    }
}

public class PlatformPortsOptions
{
    public int HttpPort { get; set; } = 80;
    public int GrpcPort { get; set; } = 82;
    public int DebugPort { get; set; } = 84;
    public string DebugHost { get; set; } = "localhost";
}
