using System;
using JustPlatform.Hosting.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JustPlatform.Hosting.DebugServer;

public class DebugEndpointsHostedService : IHostedService
{
    private readonly PlatformOptions _options;
    private WebApplication? _debugApp;

    public DebugEndpointsHostedService(PlatformOptions options)
    {
        _options = options;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_options.Ports.HttpPort == _options.Ports.DebugPort)
        {
            throw new InvalidOperationException("Main port and debug port cannot be the same.");
        }

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Production // или Development
        });

        builder.WebHost.UseUrls($"http://localhost:{_options.Ports.DebugPort}");

        builder.Services.AddSingleton(_options);

        var app = builder.Build();

        if (_options.EnableHealthChecks)
        {
            app.MapHealthChecks("/healthz", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("live")
            });
            app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready")
            });
        }

        if (_options.EnableMetrics)
        {
            app.MapPrometheusScrapingEndpoint(); // для /metrics
        }

        if (_options.EnableSwagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        _debugApp = app;

        await app.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_debugApp != null)
        {
            await _debugApp.StopAsync(cancellationToken);
        }
    }
}
