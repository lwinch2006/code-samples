namespace OAuthClient.Utils;

public static class StateUtils
{
    public static string Generate()
    {
        var rnd = new Random();
        var bytes = new byte[16];
        rnd.NextBytes(bytes);
        var state = BitConverter.ToString(bytes).Replace("-", string.Empty).ToLower();
        return state;
    }
}