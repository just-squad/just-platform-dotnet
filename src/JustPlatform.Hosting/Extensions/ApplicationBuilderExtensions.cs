using JustPlatform.Hosting.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JustPlatform.Hosting.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseJustPlatform(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<PlatformOptions>();
        var extensibilityOptions = app.ApplicationServices.GetRequiredService<JustPlatformExtensibilityOptions>();
        var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

        if (options.EnableSwagger)
        {
            app.UseCors(options.Cors.PolicyName);
        }

        app.UseRouting();
        
        // Allow user to add custom middleware
        extensibilityOptions.ConfigurePipeline?.Invoke(app);
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            
            // Allow user to add custom endpoints
            extensibilityOptions.ConfigureEndpoints?.Invoke(endpoints);
        });

        return app;
    }
}
