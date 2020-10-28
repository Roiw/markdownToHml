using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace markdownToHml
{
    class Program
    {
        static void Main(string[] args)
        {    
            TestMarkdown();
        }

        private static void TestMarkdown()
        {

            bool noError = true;
            noError = RunMarkdownTest(@"unitTests\general.txt", "General") && noError;
            noError = RunMarkdownTest(@"unitTests\orderedList.txt", "Ordered Lists") && noError;
            noError = RunMarkdownTest(@"unitTests\unorderedList.txt", "Unordered Lists") && noError;
            noError = RunMarkdownTest(@"unitTests\italic.txt", "Italic") && noError;
            noError = RunMarkdownTest(@"unitTests\bold.txt", "Bold") && noError;
            noError = RunMarkdownTest(@"unitTests\header.txt", "Header") && noError;
            noError = RunMarkdownTest(@"unitTests\paragraph.txt", "Paragraph") && noError;
            
            if (noError)
                Console.WriteLine("All tests completed successfully.");
            else
                Console.WriteLine("Tests completed with errors.");

        }

        private static bool RunMarkdownTest(string testFile, string testName)
        {
            MarkdownToHtml parser = new MarkdownToHtml();

            Console.WriteLine("------------------------------ Running " + testName);
            Console.WriteLine("");
            var olTests = Regex.Split(System.IO.File.ReadAllText(testFile), @"############ Test \d{1,} ############").Where( s => s != String.Empty);
            int testNumber = 1;
            bool foundError = true;
            foreach (string tst in olTests)
            {
                string[] spltTest = Regex.Split(tst, @"--------------- Ans ------------" + Environment.NewLine);
                if (String.Equals(parser.ParseMarkdown(spltTest[0]), spltTest[1].TrimEnd('\r', '\n')))
                    Console.WriteLine("Ordered List Test " + testNumber + ": Passed!");
                else
                {
                    Console.WriteLine("Ordered List Test " + testNumber + ": Failed");
                    foundError = false;
                }
                testNumber++;
            }
            Console.WriteLine("");
            return foundError;
        }
    }
}
