using JustPlatform.Hosting.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace JustPlatform.Hosting.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseJustPlatform(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<PlatformOptions>();
        var extensibilityOptions = app.ApplicationServices.GetRequiredService<JustPlatformExtensibilityOptions>();

        if (options.EnableSwagger)
        {
            app.UseCors(options.Cors.PolicyName);
        }

        app.UseRouting();
        extensibilityOptions.ConfigurePipeline?.Invoke(app);
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            extensibilityOptions.ConfigureEndpoints?.Invoke(endpoints);
        });

        return app;
    }
}