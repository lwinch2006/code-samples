namespace OAuthClient.Extensions;

public static class ArrayExtensions
{
    public static string ToString(this Array source)
    {
        return string.Join(' ', source);
    }
}