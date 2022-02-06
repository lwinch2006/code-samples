namespace OAuthClient.Models.Constants;

public enum OAuthFlowTypes
{
    AuthorizationCode = 1,
    AuthorizationCodeWithPKCE = 2,
    Implicit = 3,
    Password = 4,
    ClientCredentials = 5,
    Device = 6
}