using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace JustPlatform.Hosting.Configuration;

internal class JustPlatformExtensibilityOptions
{
    public Action<IApplicationBuilder>? ConfigurePipeline { get; set; }
    public Action<IEndpointRouteBuilder>? ConfigureEndpoints { get; set; }
}
