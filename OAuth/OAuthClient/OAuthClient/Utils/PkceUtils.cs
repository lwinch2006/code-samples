using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace OAuthClient.Utils;

public static class PkceUtils
{
    public static string GenerateCodeVerifier(int length = 64)
    {
        const string validCharsAsString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-._~";
        return GetCryptographicallyRandomStringAlt1(validCharsAsString, length);
    }

    public static string GenerateCodeChallenge(string codeVerifier, bool isPlain = false)
    {
        if (isPlain)
        {
            return codeVerifier;
        }

        using var sha256 = SHA256.Create();
        var codeVerifierHashed = sha256.ComputeHash(Encoding.ASCII.GetBytes(codeVerifier));
        var result = Base64UrlTextEncoder.Encode(codeVerifierHashed);
        return result;
    }

    private static string GetCryptographicallyRandomStringAlt1(string validCharsAsString, int length)
    {
        var validChars = validCharsAsString.ToCharArray();
        var buffer = new byte[sizeof(uint)];
        var result = new StringBuilder();
        using var rng = RandomNumberGenerator.Create();

        while (length-- > 0)
        {
            rng.GetBytes(buffer);
            var num = BitConverter.ToUInt32(buffer, 0);
            result.Append(validChars[(int)(num % (uint)validChars.Length)]);
        }

        return result.ToString();
    }

    private static string GetCryptographicallyRandomStringAlt2(string validCharsAsString, int length)
    {
        var validChars = validCharsAsString.ToCharArray();
        var result = new StringBuilder();
        using var rng = RandomNumberGenerator.Create();

        while (result.Length < length)
        {
            var oneByte = new byte[1];
            rng.GetBytes(oneByte);
            var character = (char)oneByte[0];
            if (!validChars.Contains(character))
            {
                continue;
            }

            result.Append(character);
        }

        return result.ToString();
    }
}