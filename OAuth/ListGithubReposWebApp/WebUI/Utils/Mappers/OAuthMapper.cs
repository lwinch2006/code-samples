using OAuthClient.Models.Responses;
using WebUI.ViewModels;
using WebUI.ViewModels.OAuth;

namespace WebUI.Utils.Mappers;

public static class OAuthMapper
{
    public static ErrorResponse Map(ErrorCallbackViewModel source)
    {
        if (source == null)
        {
            return null;
        }

        var destination = new ErrorResponse
        {
            Error = source.Error,
            ErrorDescription = source.ErrorDescription,
            ErrorUri = source.ErrorUri
        };

        return destination;
    }

    public static AuthorizationCodeResponse Map(AuthorizationCodeResponseViewModel source)
    {
        if (source == null)
        {
            return null;
        }

        var destination = new AuthorizationCodeResponse
        {
            Code = source.Code,
            State = source.State
        };

        return destination;
    }

    public static ImplicitFlowResponse Map(ImplicitFlowResponseViewModel source)
    {
        if (source == null)
        {
            return null;
        }

        var destination = new ImplicitFlowResponse
        {
            AccessToken = source.AccessToken,
            State = source.State,
            TokenType = source.TokenType,
            ExpiresIn = source.ExpiresIn,
            Scope = source.Scope
        };

        return destination;
    }
}