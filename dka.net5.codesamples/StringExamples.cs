using System;
using dka.net5.codesamples.library.Extentions;

namespace dka.net5.codesamples
{
    public static class StringExamples
    {
        public static void Run()
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
    }
}