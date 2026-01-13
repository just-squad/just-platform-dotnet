namespace JustPlatform.Configuration.Models;

public interface IHasVaultData
{
    Dictionary<string, object> Data { get; set; }
}
