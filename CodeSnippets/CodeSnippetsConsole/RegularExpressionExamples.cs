using System;
using System.Text.RegularExpressions;
using CodeSnippetsLibrary;

namespace CodeSnippetsConsole
{
    public static class RegularExpressionExamples
    {
        public static void Run()
        {
            Test1();
        }

        private static void Test1()
        {
            const string str1 = "sample1 hello world";
            const string str2 = "sample2 hello world";
            const string str3 = "sample1 sample2 hello world";
            const string str4 = "sample1sample2 hello world";
            const string str5 = "hello world";
            
            Console.WriteLine();
            Console.WriteLine("Regular expressions - string does not containing substring");
            Console.WriteLine("1) Given:");
            Console.WriteLine("Pattern: {0}", RegularExpressions.Patterns.StringNotCointainingSubstring);
            Console.WriteLine("String 1: \"{0}\"", str1);
            Console.WriteLine("String 2: \"{0}\"", str2);
            Console.WriteLine("String 3: \"{0}\"", str3);
            Console.WriteLine("String 4: \"{0}\"", str4);
            Console.WriteLine("String 5: \"{0}\"", str5);
            Console.WriteLine();

            var isMatch1 = Regex.IsMatch($"\"{str1}\"", RegularExpressions.Patterns.StringNotCointainingSubstring).ToString();
            var isMatch2 = Regex.IsMatch($"\"{str2}\"", RegularExpressions.Patterns.StringNotCointainingSubstring).ToString();
            var isMatch3 = Regex.IsMatch($"\"{str3}\"", RegularExpressions.Patterns.StringNotCointainingSubstring).ToString();
            var isMatch4 = Regex.IsMatch($"\"{str4}\"", RegularExpressions.Patterns.StringNotCointainingSubstring).ToString();
            var isMatch5 = Regex.IsMatch($"\"{str5}\"", RegularExpressions.Patterns.StringNotCointainingSubstring).ToString();
            
            Console.WriteLine("Is string 1 matches pattern: {0}", isMatch1);
            Console.WriteLine("Is string 2 matches pattern: {0}", isMatch2);
            Console.WriteLine("Is string 3 matches pattern: {0}", isMatch3);
            Console.WriteLine("Is string 4 matches pattern: {0}", isMatch4);
            Console.WriteLine("Is string 5 matches pattern: {0}", isMatch5);
            
            Console.WriteLine();
        }
    }
}