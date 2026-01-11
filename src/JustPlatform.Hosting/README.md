# JustPlatform.Hosting

A foundational library for building robust ASP.NET Core services, providing out-of-the-box configuration for common patterns like health checks, metrics, logging, and more. This library is designed to bootstrap your application with sensible defaults and a clean, extensible API.

## Features

- **Centralized Configuration**: A single `AddJustPlatform()` call to set up your application's core services.
- **Port Management**: Automatic configuration of HTTP, gRPC, and Debug ports.
- **Health Checks**: Pre-configured Liveness and Readiness health check endpoints (`/live`, `/ready`) exposed on a separate debug port.
- **Metrics**: Integrated OpenTelemetry for metrics with a Prometheus exporter endpoint (`/metrics`) on the debug port.
- **Logging**: Quick setup for Serilog.
- **Swagger/OpenAPI**: Automatic Swagger UI setup for development environments.
- **Extensibility**: Easily add your own services and middleware via `Action` delegates.

## Getting Started

To get started, install the NuGet package and use the `AddJustPlatform()` and `UseJustPlatform()` extension methods in your `Program.cs`.

```csharp
// Program.cs
using JustPlatform.Hosting.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

var builder = WebApplication.CreateBuilder(args);

// Add and configure platform services
builder.AddJustPlatform(
    // Configure platform options
    configureOptions: options =>
    {
        options.EnableSwagger = true;
        options.Ports.HttpPort = 8080;
        options.Ports.DebugPort = 8081;
        options.Ports.GrpcPort = 8082;
    },
    // Add your own custom services
    addServices: services =>
    {
        services.AddTransient<MyDummyService>();
    },
    // Add your own custom endpoints
    configureEndpoints: endpoints =>
    {
        endpoints.MapGet("/hello", async context =>
        {
            await context.Response.WriteAsync("Hello, World!");
        });
    }
);

var app = builder.Build();

// Use all the configured platform middleware
app.UseJustPlatform();

app.Run();

// Dummy service for demonstration
public class MyDummyService { }
```

## Configuration

Configuration is handled via the `PlatformOptions` class, which can be configured through `appsettings.json` or directly in code.

**appsettings.json:**
```json
{
  "PlatformOptions": {
    "EnableHealthChecks": true,
    "EnableMetrics": true,
    "EnableSerilog": true,
    "EnableSwagger": true,
    "Ports": {
      "HttpPort": 8080,
      "GrpcPort": 8082,
      "DebugPort": 8081
    }
  }
}
```

## Extensibility

`JustPlatform.Hosting` is designed to be extensible. The `AddJustPlatform` method provides hooks to register your own services and middleware.

- `addServices`: An `Action<IServiceCollection>` to register your application's services.
- `configurePipeline`: An `Action<IApplicationBuilder>` to add custom middleware to the request pipeline.
- `configureEndpoints`: An `Action<IEndpointRouteBuilder>` to add custom endpoints, in addition to the standard controllers.
