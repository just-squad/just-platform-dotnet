using System;
using System.Net.Http.Json;
using JustPlatform.Configuration.Models;
using Microsoft.Extensions.Logging;

namespace JustPlatform.Configuration.Providers;

public class VaultHttpProvider : IVaultProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<VaultHttpProvider> _logger;

    public VaultHttpProvider(HttpClient httpClient, ILogger<VaultHttpProvider> logger)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Acme.Platform.Configuration");
        _logger = logger;
    }

    public async Task<VaultSecretResponse?> GetSecretV1Async(string path, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/v1/{path}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Vault v1 request failed with status {StatusCode}", response.StatusCode);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<VaultSecretResponse>(cancellationToken: cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching v1 secret from Vault at path {Path}", path);
            return null;
        }
    }

    public async Task<VaultV2SecretResponse?> GetSecretV2Async(string path, CancellationToken cancellationToken = default)
    {
        try
        {
            // Для KV v2 путь должен быть: /v1/{mount}/data/{path}
            var mount = ExtractMountFromPath(path);
            var secretPath = ExtractSecretPathFromPath(path);
            var fullUrl = $"/v1/{mount}/data/{secretPath}";

            var response = await _httpClient.GetAsync(fullUrl, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Vault v2 request failed with status {StatusCode}", response.StatusCode);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<VaultV2SecretResponse>(cancellationToken: cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching v2 secret from Vault at path {Path}", path);
            return null;
        }
    }

    private static string ExtractMountFromPath(string path)
    {
        var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0] : "secret";
    }

    private static string ExtractSecretPathFromPath(string path)
    {
        var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length <= 1 
            ? "" 
            : string.Join("/", parts.Skip(1));
    }
}
