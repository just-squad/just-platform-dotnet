using System;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace JustPlatform.Hosting.HealthCheck;

public class LivenessCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        // Простой пример — всегда жив
        Task.FromResult(HealthCheckResult.Healthy());
}

