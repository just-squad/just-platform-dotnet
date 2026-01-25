using JustPlatform.Hosting.Configuration;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace JustPlatform.Hosting.Swagger;

/// <summary>
/// A Swagger document filter that adds the main application's server URL to the OpenAPI document.
/// This ensures that the Swagger UI sends API requests to the correct application port,
/// even when the UI is served from a different debug port.
/// </summary>
public class ServersDocumentFilter(PlatformOptions options) : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var url = $"http://localhost:{options.Ports.HttpPort}";

        if (!string.IsNullOrEmpty(options.PublicUrl))
        {
            url = $"{options.PublicUrl}";
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = $"http://{url}";
            }
        }

        swaggerDoc.Servers = new List<OpenApiServer>
        {
            new() { Url = url }
        };
    }
}