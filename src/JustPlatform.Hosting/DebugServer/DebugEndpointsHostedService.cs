using System.Reflection;
using JustPlatform.Hosting.Configuration;
using JustPlatform.Hosting.HealthCheck;
using JustPlatform.Hosting.Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Swagger;

namespace JustPlatform.Hosting.DebugServer;

public class DebugEndpointsHostedService(PlatformOptions options, IServiceProvider mainAppServiceProvider)
    : IHostedService
{
    private WebApplication? _debugApp;

    public async Task StartAsync(CancellationToken ct)
    {
        if (options.Ports.HttpPort == options.Ports.DebugPort)
        {
            throw new InvalidOperationException("Main port and debug port cannot be the same.");
        }

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Production // или Development
        });

        builder.WebHost.UseUrls($"http://{options.Ports.DebugHost}:{options.Ports.DebugPort}");

        // Регистрируем сервисы, необходимые для debug-портов
        builder.Services.AddSingleton(options);

        if (options.EnableHealthChecks)
        {
            builder.Services.AddHealthChecks()
                .AddCheck<LivenessCheck>("liveness", HealthStatus.Unhealthy, ["live"])
                .AddCheck<ReadinessCheck>("readiness", HealthStatus.Unhealthy, ["ready"]);
        }

        if (options.EnableMetrics)
        {
            builder.Services.AddPlatformOpenTelemetry(options);
        }

        if (options.EnableSwagger)
        {
            builder.Services.AddCors(corsOptions =>
            {
                corsOptions.AddPolicy(options.Cors.PolicyName, policyBuilder =>
                {
                    policyBuilder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            // Получаем ISwaggerProvider из основного DI контейнера и регистрируем его как инстанс
            var swaggerProvider = mainAppServiceProvider.GetRequiredService<ISwaggerProvider>();
            builder.Services.AddSingleton(swaggerProvider);
        }

        var app = builder.Build();

        // Маппим эндпоинты
        if (app.Environment.IsDevelopment())
        {
            app.MapGet("/", () => "Debug server is running!");
        }

        // Add /info endpoint
        app.MapGet("/info", () =>
        {
            var assembly = Assembly.GetEntryAssembly();
            var appName = assembly?.GetName().Name ?? "Unknown";
            var appVersion = assembly?.GetName().Version?.ToString() ?? "Unknown";
            return Results.Ok(new { ApplicationName = appName, Version = appVersion });
        });

        if (options.EnableHealthChecks)
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

        if (options.EnableMetrics)
        {
            app.MapPrometheusScrapingEndpoint();
        }

        if (options.EnableSwagger)
        {
            app.UseCors(options.Cors.PolicyName);
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        _debugApp = app;

        await app.StartAsync(ct);
    }

    public async Task StopAsync(CancellationToken ct)
    {
        if (_debugApp != null)
        {
            await _debugApp.StopAsync(ct);
        }
    }
}