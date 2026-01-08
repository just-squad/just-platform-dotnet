using System;
using Microsoft.AspNetCore.Builder;

namespace JustPlatform.Hosting.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseJustPlatform(this IApplicationBuilder app) =>
        // Здесь можно добавить middleware, которые должны быть на основном порту (например, логирование запросов)
        app;
}
