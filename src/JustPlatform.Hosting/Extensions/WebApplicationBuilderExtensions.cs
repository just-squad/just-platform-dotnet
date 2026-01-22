using JustPlatform.Configuration.Helpers;
using JustPlatform.Hosting.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JustPlatform.Hosting.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddJustPlatform(this WebApplicationBuilder builder,
        Action<PlatformOptions>? configureOptions = null,
        Action<IServiceCollection>? addServices = null,
        Action<IApplicationBuilder>? configurePipeline = null,
        Action<IEndpointRouteBuilder>? configureEndpoints = null
    )
    {
        // 1. Resolve options
        var options = new PlatformOptions();
        var optionsFromCfg = builder.Configuration
            .GetSection(PlatformOptions.SectionName)
            .Get<PlatformOptions>();

        if (optionsFromCfg is not null)
        {
            options = optionsFromCfg;
        }
        options = ApplyPortsFromEnvironment(options);
        configureOptions?.Invoke(options);

        // 2. Configure Kestrel
        builder.WebHost.ConfigureKestrel(kestrelOptions =>
        {
            kestrelOptions.ListenAnyIP(options.Ports.HttpPort,
                listenOptions => { listenOptions.Protocols = HttpProtocols.Http1AndHttp2; });

            kestrelOptions.ListenAnyIP(options.Ports.GrpcPort,
                listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
        });

        // 3. Add platform services
        builder.Services.AddJustPlatform(builder.Configuration, options);

        // 4. Add custom user services
        addServices?.Invoke(builder.Services);

        // 5. Register extensibility delegates for middleware
        var extensibilityOptions = new JustPlatformExtensibilityOptions
        {
            ConfigurePipeline = configurePipeline,
            ConfigureEndpoints = configureEndpoints
        };
        builder.Services.AddSingleton(extensibilityOptions);

        return builder;

        PlatformOptions ApplyPortsFromEnvironment(PlatformOptions source)
        {
            var httpPort = EnvironmentHelper.GetHttpPortEvnVariable();
            if (httpPort is not null)
            {
                if(int.TryParse(httpPort, out var httpPortInt))
                {
                    source.Ports.HttpPort = httpPortInt;
                }
            }
            var grpcPort = EnvironmentHelper.GetGrpcPortEvnVariable();
            if (grpcPort is not null)
            {
                if(int.TryParse(grpcPort, out var grpcPortInt))
                {
                    source.Ports.GrpcPort = grpcPortInt;
                }
            }
            var debugPort = EnvironmentHelper.GetDebugPortEvnVariable();
            if (debugPort is not null)
            {
                if(int.TryParse(debugPort, out var debugPortInt))
                {
                    source.Ports.DebugPort = debugPortInt;
                }
            }
            return source;
        }
    }
}