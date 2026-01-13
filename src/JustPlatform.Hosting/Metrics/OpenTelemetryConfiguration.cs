using System.Reflection;
using JustPlatform.Hosting.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace JustPlatform.Hosting.Metrics;

public static class OpenTelemetryConfiguration
{
    public static IServiceCollection AddPlatformOpenTelemetry(
        this IServiceCollection services,
        PlatformOptions options)
    {
        services.AddOpenTelemetry()
            .WithMetrics(builder =>
            {
                builder
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(Assembly.GetEntryAssembly()?.GetName().Name ?? "UnknownService"))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddPrometheusExporter();
            });

        return services;
    }
}