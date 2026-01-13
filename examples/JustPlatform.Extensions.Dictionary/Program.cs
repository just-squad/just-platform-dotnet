using JustPlatform.Hosting.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddJustPlatform(
    configureOptions: options =>
    {
        options.EnableSwagger = true;
        options.Ports.HttpPort = 8080;
        options.Ports.DebugPort = 8081;
        options.Ports.GrpcPort = 8082;
    },
    addServices: services =>
    {
        services.AddTransient<MyDummyService>();
    },
    configureEndpoints: endpoints =>
    {
        endpoints.MapGet("/hello", async context =>
        {
            await context.Response.WriteAsync("Hello, World!");
        });
    }
);

var app = builder.Build();

app.UseJustPlatform();

app.Run();

// Dummy service for demonstration
public class MyDummyService { }