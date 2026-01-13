using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace JustPlatform.Hosting.HealthCheck;

public class LivenessCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct) =>
        // Простой пример — всегда жив
        Task.FromResult(HealthCheckResult.Healthy());
}

