using System.Text.Json.Serialization;

namespace OAuthClient.Models.Responses;

public class ErrorResponse : IOAuthClientResponse
{
    [JsonPropertyName("error")]
    public string Error { get; init; }
    
    [JsonPropertyName("error_description")]
    public string ErrorDescription { get; init; }
    
    [JsonPropertyName("error_uri")]
    public string ErrorUri { get; init; }    
}