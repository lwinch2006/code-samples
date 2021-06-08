using System;
using System.IO;
using System.Text.RegularExpressions;
using dka.net5.codesamples.library.Extentions;

namespace dka.net5.codesamples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Algorithms");
            
            // Strings.
            Init();
            StringExamples.Run();
            
            // Regular expressions.
            RegularExpressionExamples.Run();
        }

        static void ProduceWordSamplesFile()
        {
            using (var sw = new StreamWriter("word-samples-new.txt"))
            {
                foreach (var line in File.ReadLines("word-samples.txt"))
                {
                    var newline = $"\"{line.Trim()}\",";
                    sw.WriteLine(newline);
                }
                
                sw.Close();
            }
        }

        static void Init()
        {
            // Warming up algorythms.
            var t = Regex.Replace("Test bathroom", "\\bbathroom\\b", string.Empty);
            t = "Test bathroom".RemoveWord("bathroom");
            t = "Test bathroom".ReplaceWordEx("bathroom", string.Empty);
            t = "Test bathroom".ReplaceEx("bathroom", string.Empty);
        }
    }
}