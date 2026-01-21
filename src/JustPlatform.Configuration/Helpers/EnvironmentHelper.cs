namespace JustPlatform.Configuration.Helpers;

public static class EnvironmentHelper
{
    /// <summary>
    /// Получение значение http порта для сервиса из переменных окружения.
    /// </summary>
    /// <returns>Значение http порта или null</returns>
    public static string? GetHttpPortEvnVariable() =>
        Environment.GetEnvironmentVariable(Constants.Environment.HttpPortEvnName);

    /// <summary>
    /// Получение значение grpc порта для сервиса из переменных окружения.
    /// </summary>
    /// <returns>Значение grpc порта или null</returns>
    public static string? GetGrpcPortEvnVariable() =>
        Environment.GetEnvironmentVariable(Constants.Environment.GrpcPortEvnName);

    /// <summary>
    /// Получение значение debug порта для сервиса из переменных окружения.
    /// </summary>
    /// <returns>Значение debug порта или null</returns>
    public static string? GetDebugPortEvnVariable() =>
        Environment.GetEnvironmentVariable(Constants.Environment.DebugPortEvnName);
}