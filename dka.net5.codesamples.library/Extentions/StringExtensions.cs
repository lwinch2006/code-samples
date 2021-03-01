using System;
using System.Linq;

namespace dka.net5.codesamples.library.Extentions
{
    public static class StringExtensions
    {
        public static int LevenshteinDistance(string source, string target)
        {
            var sourceLength = source.Length;
            var targetLength = target.Length;
            var distance = new int[sourceLength + 1, targetLength + 1];

            // Step 1
            if (sourceLength == 0)
            {
                return targetLength;
            }

            if (targetLength == 0)
            {
                return sourceLength;
            }

            // Step 2
            for (var i = 0; i <= sourceLength; distance[i, 0] = i++)
            {
            }

            for (var j = 0; j <= targetLength; distance[0, j] = j++)
            {
            }

            // Step 3
            for (var i = 1; i <= sourceLength; i++)
            {
                //Step 4
                for (var j = 1; j <= targetLength; j++)
                {
                    // Step 5
                    var cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                    // Step 6
                    distance[i, j] = Math.Min(
                        Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                        distance[i - 1, j - 1] + cost);
                }
            }

            // Step 7
            return distance[sourceLength, targetLength];
        }        
        
        public static float ComputeSimilarity(this string source, string target)
        {
            if (string.Equals(source, target, StringComparison.OrdinalIgnoreCase))
            {
                return 100.0f;
            }

            if (source == null || target == null)
            {
                return 0.0f;
            }

            var distance = LevenshteinDistance(source, target);
            var maxLength = Math.Max(source.Length, target.Length);
            var similarityPercentage = (float)(maxLength - distance) / maxLength * 100.0f;
            return similarityPercentage;
        }
        
        public static string ReplaceEx(this string originalString, string pattern, string replacement)
        {
            if (originalString == null ||
                pattern == null ||
                replacement == null)
            {
                return originalString;
            }

            var count = 0;
            var position0 = 0;
            var position1 = 0;

            var upperString = originalString.ToUpper();
            var upperPattern = pattern.ToUpper();

            var inc = (originalString.Length / pattern.Length) * (replacement.Length - pattern.Length);

            var chars = new char[originalString.Length + Math.Max(0, inc)];

            while ((position1 = upperString.IndexOf(upperPattern, position0)) != -1)
            {
                for (var i = position0; i < position1; ++i)
                {
                    chars[count++] = originalString[i];
                }

                for (var i = 0; i < replacement.Length; ++i)
                {
                    chars[count++] = replacement[i];
                }

                position0 = position1 + pattern.Length;
            }

            if (position0 == 0)
            {
                return originalString;
            }

            for (var i = position0; i < originalString.Length; ++i)
            {
                chars[count++] = originalString[i];
            }

            return new string(chars, 0, count);
        }        
        
        public static string ReplaceEx(this string originalString, string[] patterns, string replacement)
        {
            if (originalString == null ||
                patterns == null ||
                replacement == null)
            {
                return originalString;
            }

            for (var i = 0; i < patterns.Length; i++)
            {
                originalString = originalString.ReplaceEx(patterns[i], replacement);
            }

            return originalString;
        }
        
        public static string ExtractAllDigits(this string originalString)
        {
            if (originalString == null)
            {
                return null;
            }

            var arrayOfDigits = originalString.Where(char.IsDigit).ToArray();
            var result = string.Join(string.Empty, arrayOfDigits);

            return result;
        }        
    }
}