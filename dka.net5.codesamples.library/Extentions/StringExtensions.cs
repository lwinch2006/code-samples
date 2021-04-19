using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
        
        public static string Compact(this string source)
        {
            var result = source.ToLower().Replace(" ", string.Empty);
            return result;
        }

        public static bool CheckSimilarity(this string source, string target, float threshold)
        {
            var sourceCompact = source.Compact();
            var targetCompact = target.Compact();

            return sourceCompact.ComputeSimilarity(targetCompact) >= threshold;
        }
        
        public static bool IsWordBreaker(this char source)
        {
            if (source == 43 // '+' not counted as word separator
                || source == 45 // '-' not counted as word separator
                || source == 47 // '/' not counted as word separator
                || source == 92) // '\' not counted as word separator
            {
                return false;
            }

            // See ASCII table for description.
            var result = (source >= 32 && source <= 47)
                         || (source >= 58 && source <= 64)
                         || (source >= 91 && source <= 96)
                         || (source >= 123 && source <= 126);

            return result;
        }
        
        public static string RemoveWords(this string stringToClean, IEnumerable<string> wordsToRemove)
        {
            var wordSeparators = new[] {' ', ',', '.', '?', '!'};
            var result = string.Join(" ", stringToClean.Split(wordSeparators, StringSplitOptions.RemoveEmptyEntries).Where(w => !wordsToRemove.Contains(w)));
            return result;
        }        
        
        public static (string string1, string string2) RemoveSameWordsWithRegex(string string1, string string2, IEnumerable<string> words)
        {
            foreach (var word in words)
            {
                var pattern = $@"\b{word}\b";

                var newString1 = Regex.Replace(string1, pattern, string.Empty);
                var newString2 = Regex.Replace(string2, pattern, string.Empty);
                
                if (newString1.Length != string1.Length && newString2.Length != string2.Length)
                {
                    string1 = newString1;
                    string2 = newString2;
                }                
            }

            return (string1, string2);
        }        
        
        public static (string string1, string string2) RemoveSameWords(string string1, string string2, IEnumerable<string> words)
        {
            foreach (var word in words)
            {
                var newString1 = string1.RemoveWord(word);
                var newString2 = string2.RemoveWord(word);

                if (newString1.Length != string1.Length && newString2.Length != string2.Length)
                {
                    string1 = newString1;
                    string2 = newString2;
                }
            }

            return (string1, string2);
        }
        
        public static string RemoveWord(this string stringToClean, string wordToRemove)
        {
            var wordSeparators = new[] {' ', ',', '.', '?', '!'};
            var result = string.Join(" ", stringToClean.Split(wordSeparators, StringSplitOptions.RemoveEmptyEntries).Where(w => w != wordToRemove));
            return result;
        }   
        
        public static (string string1, string string2) RemoveSameWordsEx(string string1, string string2, IEnumerable<string> words)
        {
            foreach (var word in words)
            {
                var newString1 = string1.ReplaceWordEx(word, string.Empty);                
                var newString2 = string2.ReplaceWordEx(word, string.Empty);

                if (newString1.Length != string1.Length && newString2.Length != string2.Length)
                {
                    string1 = newString1;
                    string2 = newString2;
                }
            }

            return (string1, string2);
        }

        public static string ReplaceWordEx(this string originalString, string word, string replacement)
        {
            if (originalString == null ||
                word == null ||
                replacement == null)
            {
                return originalString;
            }

            var upperString = originalString.ToUpper();
            var upperPattern = word.ToUpper();
            
            var count = 0;
            var position0 = 0;
            int position1;

            var inc = originalString.Length / word.Length * (replacement.Length - word.Length);
            var chars = new char[originalString.Length + Math.Max(0, inc)];

            var tempPosition0 = position0; 
            
            while ((position1 = upperString.IndexOf(upperPattern, tempPosition0, StringComparison.Ordinal)) != -1)
            {
                var position1Start = position1 - 1;
                
                if (position1Start >= 0 && !originalString[position1Start].IsWordBreaker())
                {
                    tempPosition0 = position1 + word.Length;
                    continue;
                }

                var position1End = position1 + upperPattern.Length;
                
                if (position1End < originalString.Length && !originalString[position1End].IsWordBreaker())
                {
                    tempPosition0 = position1 + word.Length;
                    continue;
                }

                for (var i = position0; i < position1; ++i)
                {
                    chars[count++] = originalString[i];
                }

                for (var i = 0; i < replacement.Length; ++i)
                {
                    chars[count++] = replacement[i];
                }

                tempPosition0 = position1 + word.Length;
                position0 = tempPosition0;
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
    }
}