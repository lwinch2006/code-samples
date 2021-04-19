using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using dka.net5.codesamples.library.Extentions;

namespace dka.net5.codesamples
{
    public static class StringExamples
    {
        private static readonly Random Rnd = new Random();
        
        public static void Run()
        {
            Test1();
            Test2();
            Test3();
        }

        private static void Test1()
        {
            Console.WriteLine();
            Console.WriteLine("String similarity");
            Console.WriteLine("1) Given:");
            Console.WriteLine("String 1: arrow");
            Console.WriteLine("String 2: arow");
            Console.WriteLine();
            
            const string str1 = "arrow";
            const string str2 = "arow";

            var distinace = StringExtensions.LevenshteinDistance(str2, str1);
            var similarity = str2.ComputeSimilarity(str1);

            Console.WriteLine("Distance: {0} edits needed to convert \"{1}\" to \"{2}\"", distinace, str2, str1);
            Console.WriteLine("Similarity: \"{1}\" {0}% similar to \"{2}\"", similarity, str2, str1);
            Console.WriteLine();
        }

        private static void Test2()
        {
            Console.WriteLine();
            Console.WriteLine("Testing performance of removing same words from strings");
            Console.WriteLine("1) Given:");
            Console.WriteLine("String 1: mc donalds bathroom magazine throat");
            Console.WriteLine("String 2: mcdonalds throat security bathroom"); 
            Console.WriteLine();
            
            const string str1 = "mc donalds bathroom magazine throat";
            const string str2 = "mcdonalds throat security bathroom";

            var t1 = 0.0;
            var t2 = 0.0;
            var t3 = 0.0;
            const int n = 1;
            var result1 = (string1:string.Empty, string2:string.Empty);
            var result2 = (string1:string.Empty, string2:string.Empty);
            var result3 = (string1:string.Empty, string2:string.Empty);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            for (var i = 0; i < n; i++)
            {
                var t0 = stopWatch.Elapsed;
                result1 = StringExtensions.RemoveSameWordsWithRegex(str1, str2, Constants.WordsToRemove);
                t1 += (stopWatch.Elapsed - t0).TotalMilliseconds;
                
                t0 = stopWatch.Elapsed;
                result2 = StringExtensions.RemoveSameWords(str1, str2, Constants.WordsToRemove);
                t2 += (stopWatch.Elapsed - t0).TotalMilliseconds;

                t0 = stopWatch.Elapsed;
                result3 = StringExtensions.RemoveSameWordsEx(str1, str2, Constants.WordsToRemove);
                t3 += (stopWatch.Elapsed - t0).TotalMilliseconds;
            }

            stopWatch.Stop();
            
            t1 /= n;
            t2 /= n;
            t3 /= n;
            
            var p2 = (t2 - t1) / t1 * 100.0f;
            var p3 = (t3 - t1) / t1 * 100.0f;
            
            Console.WriteLine("Removing words with RegEx took {0:#0.000}ms and result \"{1}\" and \"{2}\"", t1, result1.string1, result1.string2);
            Console.WriteLine("Removing words with split and join took {0:#0.000}ms ({4}{3:#0.00}%) and result \"{1}\" and \"{2}\"", t2, result2.string1, result2.string2, p2, p2 > 0 ? "+" : "");
            Console.WriteLine("Removing words with ReplaceWordsEx took {0:#0.000}ms ({4}{3:#0.00}%) and result \"{1}\" and \"{2}\"", t3, result3.string1, result3.string2, p3, p3 > 0 ? "+" : "");
            Console.WriteLine();
        }

        private static void Test3()
        {
            Console.WriteLine();
            Console.WriteLine("Testing performance of RegEx and ReplaceEx");
            Console.WriteLine("1) Given:");
            Console.WriteLine("String 1: mc donalds bathroom magazine throat");
            Console.WriteLine("String 2: mcdonalds throat security bathroom");
            Console.WriteLine();
            
            const string str1 = "mc donalds bathroom magazine throat";

            var t1 = 0.0;
            var t2 = 0.0;
            var t3 = 0.0;
            var t4 = 0.0;
            const int n = 1;
            var result1 = string.Empty;
            var result2 = string.Empty;
            var result3 = string.Empty;
            var result4 = string.Empty;

            var word = "bathroom";
            
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            for (var i = 0; i < n; i++)
            {
                var t0 = stopWatch.Elapsed;
                result1 = Regex.Replace(str1, $"\\b{word}\\b", string.Empty);
                t1 += (stopWatch.Elapsed - t0).TotalMilliseconds;

                t0 = stopWatch.Elapsed;
                result2 = str1.RemoveWord(word);
                t2 += (stopWatch.Elapsed - t0).TotalMilliseconds;
                
                t0 = stopWatch.Elapsed;
                result3 = str1.ReplaceWordEx(word, string.Empty);
                t3 += (stopWatch.Elapsed - t0).TotalMilliseconds;
                
                t0 = stopWatch.Elapsed;
                result4 = str1.ReplaceEx(word, string.Empty);
                t4 += (stopWatch.Elapsed - t0).TotalMilliseconds;     
            }
            
            stopWatch.Stop();

            t1 /= n;
            t2 /= n;
            t3 /= n;
            t4 /= n;
            
            var p2 = (t2 - t1) / t1 * 100.0f;
            var p3 = (t3 - t1) / t1 * 100.0f;
            var p4 = (t4 - t1) / t1 * 100.0f;

            Console.WriteLine("Chosen word is \"{0}\"", word);
            Console.WriteLine("Replacing words with RegEx took {0:#0.000}ms and result \"{1}\"", t1, result1);
            Console.WriteLine("Replacing words with split and join took {0:#0.000}ms ({3}{2:#0.00}%) and result \"{1}\"", t2, result2, p2, p2 > 0 ? "+" : "");            
            Console.WriteLine("Replacing words with ReplaceWordEx took {0:#0.000}ms ({3}{2:#0.00}%) and result \"{1}\"", t3, result3, p3, p3 > 0 ? "+" : "");
            Console.WriteLine("Replacing words with ReplaceEx took {0:#0.000}ms ({3}{2:#0.00}%) and result \"{1}\"", t4, result4, p4, p4 > 0 ? "+" : "");            
            Console.WriteLine();
        }
    }
}