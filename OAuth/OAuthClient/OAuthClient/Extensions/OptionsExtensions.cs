using Microsoft.Extensions.Options;

namespace OAuthClient.Extensions;

public static class OptionsExtensions
{
    public static T GetEx<T>(this IOptionsMonitor<T> optionsMonitor, string name)
    {
        var result = optionsMonitor.Get(name?.ToLower());
        return result;
    }
}