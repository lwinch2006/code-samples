namespace OAuthClient.Extensions;

public static class ArrayExtensions
{
    public static string ToStringEx(this string[] source)
    {
        return string.Join(' ', source);
    }
}