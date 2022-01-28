using OAuthClient.Models.Responses;

namespace OAuthClient.Utils;

public static class OAuthClientErrorResponseUtils
{
    public static IOAuthClientResponse GetStateMismatchErrorResponse()
    {
        var response = new OAuthClientErrorResponse
        {
            Error = "",
            ErrorDescription = "",
            ErrorUri = string.Empty
        };

        return response;
    }
}