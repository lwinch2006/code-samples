using Microsoft.Extensions.Options;
using OAuthClient.Extensions;
using OAuthClient.Interfaces;
using OAuthClient.Models;

namespace OAuthClient;

public interface IOAuthClientFactory
{
    IOAuthClient CreateOAuthClient(string name = null);
}

public class OAuthClientFactory : IOAuthClientFactory
{
    private readonly IOptionsMonitor<OAuthClientConfiguration> _optionsMonitor;
    private readonly IHttpClientFactory _httpClientFactory;
    
    public OAuthClientFactory(
        IHttpClientFactory httpClientFactory,
        IOptionsMonitor<OAuthClientConfiguration> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
        _httpClientFactory = httpClientFactory;
    }

    public IOAuthClient CreateOAuthClient(string name = null)
    {
        var oauthClientConfiguration = _optionsMonitor.GetEx(name);

        var oauthClient = new OAuthClient(_httpClientFactory, oauthClientConfiguration);
        
        return oauthClient;
    }
}
