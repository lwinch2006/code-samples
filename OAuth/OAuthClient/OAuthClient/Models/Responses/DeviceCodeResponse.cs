using System.Text.Json.Serialization;

namespace OAuthClient.Models.Responses;

public class DeviceCodeResponse : IOAuthClientResponse
{
    [JsonPropertyName("device_code")]
    public string DeviceCode { get; init; }
    
    [JsonPropertyName("user_code")]
    public string UserCode { get; init; }
    
    [JsonPropertyName("verification_uri")]
    public string VerficationUri { get; init; }
    
    [JsonPropertyName("interval")]
    public int Interval { get; set; }
    
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
}