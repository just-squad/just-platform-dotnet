using Serilog;

namespace JustPlatform.Hosting.Logging;

public class SerilogConfiguration
{
    public static void ConfigureSerilog() => Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateBootstrapLogger(); // или CreateLogger() в зависимости от того, когда вызывается
}
