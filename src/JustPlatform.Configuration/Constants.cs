namespace JustPlatform.Configuration;

public abstract class Constants
{
    public abstract class Environment
    {
        public const string HttpPortEvnName = "ASPNETCORE_HTTP_PORT";
        public const string GrpcPortEvnName = "ASPNETCORE_GRPC_PORT";
        public const string DebugPortEvnName = "ASPNETCORE_DEBUG_PORT";
        public const string DebugHostEvnName = "ASPNETCORE_DEBUG_HOST";
    }
}