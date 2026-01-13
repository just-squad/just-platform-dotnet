using JustPlatform.Hosting.Configuration;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace JustPlatform.Hosting.Swagger;

/// <summary>
/// A Swagger document filter that adds the main application's server URL to the OpenAPI document.
/// This ensures that the Swagger UI sends API requests to the correct application port,
/// even when the UI is served from a different debug port.
/// </summary>
public class ServersDocumentFilter(PlatformOptions _options) : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context) =>
        swaggerDoc.Servers = new List<OpenApiServer>
        {
            // Assuming the main server is running on localhost.
            // This could be made more robust with configuration from IServer.
            new() { Url = $"http://localhost:{_options.Ports.HttpPort}" }
        };
}