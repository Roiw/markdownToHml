/*
 * Copyright (c) 2020 Lucas Martins de Souza (https://github.com/Roiw).
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
 * associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
