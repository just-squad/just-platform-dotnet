namespace JustPlatform.Configuration.Helpers;

public static class EnvironmentHelper
{
    /// <summary>
    /// Получение значение http порта для сервиса из переменных окружения.
    /// </summary>
    /// <returns>Значение http порта или null</returns>
    public static int? GetHttpPortEvnVariable()
    {
        var port = Environment.GetEnvironmentVariable(Constants.Environment.HttpPortEvnName);
        return int.TryParse(port, out var portInt)
            ? portInt
            : null;
    }

    /// <summary>
    /// Получение значение grpc порта для сервиса из переменных окружения.
    /// </summary>
    /// <returns>Значение grpc порта или null</returns>
    public static int? GetGrpcPortEvnVariable()
    {
        var port = Environment.GetEnvironmentVariable(Constants.Environment.GrpcPortEvnName);
        return int.TryParse(port, out var portInt)
            ? portInt
            : null;
    }

    /// <summary>
    /// Получение значение debug порта для сервиса из переменных окружения.
    /// </summary>
    /// <returns>Значение debug порта или null</returns>
    public static int? GetDebugPortEvnVariable()
    {
        var port = Environment.GetEnvironmentVariable(Constants.Environment.DebugPortEvnName);
        return int.TryParse(port, out var portInt)
            ? portInt
            : null;
    }

    /// <summary>
    /// Получение значение debug хоста для сервиса из переменных окружения.
    /// </summary>
    /// <returns>Значение debug хоста или null</returns>
    public static string? GetDebugHostEvnVariable() =>
        Environment.GetEnvironmentVariable(Constants.Environment.DebugHostEvnName);

    /// <summary>
    /// Получение значение публичного хоста для сервиса из переменных окружения.
    /// </summary>
    /// <returns>Значение публичного хоста</returns>
    public static string? GetPublicUrlEvnVariable() =>
        Environment.GetEnvironmentVariable(Constants.Environment.PublicUrlEvnName);
}