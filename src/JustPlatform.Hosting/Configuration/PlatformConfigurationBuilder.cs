using Microsoft.Extensions.Configuration;

namespace JustPlatform.Hosting.Configuration;

public static class PlatformConfigurationBuilder
{
    public static IConfigurationBuilder AddJustPlatformConfigurationSources(
        this IConfigurationBuilder builder,
        string? environmentName = null)
    {
        // 1. appsettings.json
        builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        // 2. appsettings.{Environment}.json
        var env = environmentName ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        builder.AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true);

        // 4. Environment Variables
        builder.AddEnvironmentVariables();

        return builder;
    }
}
