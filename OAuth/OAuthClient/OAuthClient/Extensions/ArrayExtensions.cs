namespace OAuthClient.Extensions;

public static class ArrayExtensions
{
    public static string ToStringEx(this IEnumerable<string> source)
    {
        if (source == null)
        {
            return string.Empty;
        }
        
        return string.Join(' ', source);
    }
}