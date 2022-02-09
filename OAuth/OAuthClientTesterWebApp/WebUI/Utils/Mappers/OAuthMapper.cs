using System.ComponentModel;
using System.Reflection;
using Application.Models.OAuth;
using OAuthClient.Models.Responses;
using WebUI.ViewModels;
using WebUI.ViewModels.OAuth;
using WebUI.ViewModels.OAuthTester;

namespace WebUI.Utils.Mappers;

public static class OAuthMapper
{
    public static OAuthTesterViewModel GetNewOAuthTesterViewModel(string configurationName = null)
    {
        var destionation = new OAuthTesterViewModel
        {
            ConfigurationName = configurationName ?? OAuthConfigurationNames.ClientCredentials,
            AvailableConfigurationNames = GetAvailableConfigurationNames()
        };

        return destionation;
    }       
    
    public static ErrorResponse Map(ErrorResponseViewModel source)
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

    public static AccessTokenResponseViewModel Map(AccessTokenResponse source)
    {
        if (source == null)
        {
            return null;
        }

        var destination = new AccessTokenResponseViewModel
        {
            AccessToken = source.AccessToken,
            TokenType = source.TokenType,
            ExpiresIn = source.ExpiresIn,
            RefreshToken = source.RefreshToken,
            Scope = source.Scope
        };

        return destination;
    }

    public static IOAuthClientResponse Map<T>(T source)
    {
        var response = (IOAuthClientResponse) (source switch
        {
            AuthorizationCodeResponseViewModel authorizationCodeResponseViewModel => Map(authorizationCodeResponseViewModel),
            ImplicitFlowResponseViewModel implicitFlowResponseViewModel => Map(implicitFlowResponseViewModel),
        });

        return response;
    }
 
    
    private static IDictionary<string, string> GetAvailableConfigurationNames()
    {
        var result = new Dictionary<string, string>();
        var fields = typeof(OAuthConfigurationNames).GetFields()
            .Where(f => f.Name != nameof(OAuthConfigurationNames.Github));

        foreach (var field in fields)
        {
            var key = (string)field.GetValue(field);
            var value = field.GetCustomAttribute<DescriptionAttribute>()?.Description ?? key;
            result.Add(key, value);
        }

        return result;
    }    
}