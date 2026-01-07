using System;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace JustPlatform.Hosting.HealthCheck;

public class ReadinessCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        // Пример — проверяем готовность к работе (например, подключение к БД)
        // Пока просто Healthy, но можно расширить
        Task.FromResult(HealthCheckResult.Healthy());
}
