namespace OAuthClient;

public interface IOAuthFlowsFactory
{
    public IOAuthFlows CreateOAuthFlows(string name);
}

public class OAuthFlowsFactory : IOAuthFlowsFactory
{
    private readonly IOAuthClientFactory _oAuthClientFactory;
    
    public OAuthFlowsFactory(IOAuthClientFactory oAuthClientFactory)
    {
        _oAuthClientFactory = oAuthClientFactory;
    }
    
    public IOAuthFlows CreateOAuthFlows(string name)
    {
        var oauthClient = _oAuthClientFactory.CreateOAuthClient(name);
        var oauthFlows = new OAuthFlows(oauthClient);
        return oauthFlows;
    }
}