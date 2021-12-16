using System.Text;
using System.Text.Json;
using Dka.Net.ApimBackupRestore.Models.Apim;
using Microsoft.Extensions.Configuration;

namespace Dka.Net.ApimBackupRestore.Utils;

public static class ApimUtils
{
    private const string SubscriptionId = "SubscriptionId";
    private const string ResourceGroupName = "ResourceGroupName";
    private const string ServiceName = "ServiceName";
    private const string ApiVersion = "ApiVersion";
    
    private const string StorageAccount = "StorageAccount";
    private const string ContainerName = "ContainerName";
    
    private const string BackupUrlPattern =
        "https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ApiManagement/service/{serviceName}/backup?api-version={api-version}";

    private const string RestoreUrlPattern =
        "https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.ApiManagement/service/{serviceName}/restore?api-version={api-version}";
    
    public static async Task Backup(IConfiguration configuration, HttpClient httpClient)
    {
        var accessToken = await AuthorizationUtils.GetBearerToken(configuration);
        
        var backupUrl = BackupUrlPattern
            .Replace("{subscriptionId}", configuration[SubscriptionId])
            .Replace("{resourceGroupName}", configuration[ResourceGroupName])
            .Replace("{serviceName}", configuration[ServiceName])
            .Replace("{api-version}", configuration[ApiVersion]);

        var backupName = $"{configuration[ServiceName]}/backup";
        
        var contentPayload = $@"{{
            ""storageAccount"": ""{configuration[StorageAccount]}"",
            ""containerName"": ""{configuration[ContainerName]}"",
            ""backupName"": ""{backupName}"",
            ""accessType"": ""SystemAssignedManagedIdentity""
        }}";
        
        var content = new StringContent(contentPayload, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, backupUrl)
        {
            Content = content
        };
        
        request.Headers.Add("Authorization", $"Bearer {accessToken}");
        
        var response = await httpClient.SendAsync(request);
        
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var errorModel = JsonSerializer.Deserialize<ApimErrorResponse>(
                responseContent, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Console.WriteLine("Error ({0}) with message ({1})", errorModel.Error.Code, errorModel.Error.Message);
            Console.WriteLine();
            throw;
        }
        
        Console.WriteLine("Location to check operation status: {0}", response.Headers.Location);
    }
    
    public static async Task Restore(IConfiguration configuration, HttpClient httpClient)
    {
        var accessToken = await AuthorizationUtils.GetBearerToken(configuration);
        
        var restoreUrl = RestoreUrlPattern
            .Replace("{subscriptionId}", configuration[SubscriptionId])
            .Replace("{resourceGroupName}", configuration[ResourceGroupName])
            .Replace("{serviceName}", configuration[ServiceName])
            .Replace("{api-version}", configuration[ApiVersion]);
        
        var restoreName = $"{configuration[ServiceName]}/backup";
        
        var contentPayload = $@"{{
            ""storageAccount"": ""{configuration[StorageAccount]}"",
            ""containerName"": ""{configuration[ContainerName]}"",
            ""backupName"": ""{restoreName}"",
            ""accessType"": ""SystemAssignedManagedIdentity""
        }}";
        
        var content = new StringContent(contentPayload, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, restoreUrl)
        {
            Content = content
        };
        
        request.Headers.Add("Authorization", $"Bearer {accessToken}");
        
        var response = await httpClient.SendAsync(request);

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var errorModel = JsonSerializer.Deserialize<ApimErrorResponse>(
                responseContent, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Console.WriteLine("Error ({0}) with message ({1})", errorModel.Error.Code, errorModel.Error.Message);
            Console.WriteLine();
            throw;
        }        
        
        Console.WriteLine("Location to check operation status: {0}", response.Headers.Location);
    }
}