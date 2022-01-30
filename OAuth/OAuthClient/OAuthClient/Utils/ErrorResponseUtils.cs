using OAuthClient.Models.Responses;

namespace OAuthClient.Utils;

public static class ErrorResponseUtils
{
    public static IOAuthClientResponse GetStateMismatchErrorResponse()
    {
        var response = new ErrorResponse
        {
            Error = "Invalid state error",
            ErrorDescription = "Returned state does not match the original one",
            ErrorUri = string.Empty
        };

        return response;
    }

    public static IOAuthClientResponse GetExchangeAuthCodeToAccessTokenErrorResponse()
    {
        var response = new ErrorResponse
        {
            Error = "Exchange authorization code to access token error",
            ErrorDescription = "Unhandled exception has happen during exchanging authorization code to access token",
            ErrorUri = string.Empty
        };

        return response;
    }
}