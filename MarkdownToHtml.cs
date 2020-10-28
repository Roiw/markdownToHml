using System;
using System.Text.RegularExpressions;

namespace markdownToHml
{
    public class MarkdownToHtml
    {    
        /// <summary>
        /// Converts an Markdown file to Html
        /// </summary>
        /// <param name="inputMarkdownFilePath">The location of the markdown file.</param>
        /// <param name="outputMarkdownFilePath">The location to output the HTML file.</param>
        public void ParseMarkdown(string inputMarkdownFilePath, string outputMarkdownFilePath)
        {
            string source = System.IO.File.ReadAllText(inputMarkdownFilePath);
            source = "\n" + source + "\n";
            System.IO.File.WriteAllText(outputMarkdownFilePath, Parse(source));
        }

        /// <summary>
        /// Converts an Markdown string to Html
        /// </summary>
        /// <param name="source">A string with markdown sintaxe</param>
        /// <returns>A string with HTML sintaxe</returns>
        public string ParseMarkdown(string source)
        {
           return Parse(source);
        }

        /// <summary>
        /// Use a set of rules to parse Markdown to HTML.
        /// </summary>
        /// <param name="source">A string in Markdown format.</param>
        /// <returns>A string in html format.</returns>
        private string Parse(string source)
        {
            // Call Preprocess..
            source = Preprocess(source);

            Rule[] rules = new Rule[] 
            {
                new BoldRule(),
                new ItalicRule(),
                new HeaderRule(),
                new ParagraphRule(),
                new OrderedListRule(),
                new UnorderedListRule(),
            };

            foreach (Rule rule in rules)
            {
               source = rule.Process(source);
            }

            // Call post-process..
            return Postprocess(source);
        }

        /// <summary>
        /// Some operations to help process the result.
        /// </summary>
        /// <param name="input">The raw input.</param>
        /// <returns>Processed input.</returns>
        private string Preprocess(string input)
        {
            // Case we are using windows line ending, we normalize.
            string output = Regex.Replace(input, "\r\n", "\n"); 
            output = Regex.Replace(output, @"\n\s+\n", "\n\n"); 
            output = Regex.Replace(output, @"\&(?!\w+?;)", @"&amp;"); 
            output = output.Replace("<", @"&lt;");
            return output;
        }

        /// <summary>
        /// Some operations to restore the output to it's correct format.
        /// </summary>
        /// <param name="input">The parsed HTML input.</param>
        /// <returns>Post processed input.</returns>
        private string Postprocess(string input)
        {
            // Normalize spaces between tags from multiple to 1 line.
            string output = Regex.Replace(input, @"(?<=(\</.*?\>))\n{3,}(?=(\<.*?\>))", "\n\n"); 
            // Add your system line ending.
            output = Regex.Replace(output, "\n", Environment.NewLine);
            output = output.Replace(@"<ol>", Environment.NewLine + "<ol>");
            output = output.Replace(@"<ul>", Environment.NewLine + "<ul>");
            output = output.Replace(@"\&", "&");
            return output.Trim();
        }

        /// <summary>
        /// Represents a rule for parsing Unordered Lists.
        /// </summary>
        private class UnorderedListRule : Rule
        {
            private string CreateHtml(Match elem)
            {
                string ns = Regex.Replace(elem.Value, @"\n(\*|\+|-)", "");
                return "<ul>\n<li>" + ns.Trim() + "</li>\n</ul>";
            }

            public override string Process(string input)
            {
                //((\n\n)|(\n[\*|\+|-])
                string offStart = @"<p>([^(</p>)])*";
                input = Regex.Replace(input, @"(?<!"+ offStart + @")\n(\*|\+|-)(.*)", CreateHtml);
                return Regex.Replace(input, @"\s?</ul>(\s|\n)*?<ul>", "");
            }
        }

        /// <summary>
        /// Represents a rule for parsing Ordered Lists.
        /// </summary>
        private class OrderedListRule : Rule
        {
            private string CreateHtml(Match elem)
            {
                string ns = Regex.Replace(elem.Value, @"\n[0-9]+\.", "");
                return "<ol>\n<li>" + ns.Trim() + "</li>\n</ol>";
            }

            public override string Process(string input)
            {
                string offStart = @"<p>([^(</p>)])*";
                input = Regex.Replace(input,  @"(?<!"+ offStart + @")\n[0-9]+\.\s(.*)", CreateHtml);
                return Regex.Replace(input, @"\s?</ol>(\s|\n)*?<ol>", "");
            }
        }

        /// <summary>
        /// Represents a rule for parsing Italic (Emphasis) elemets.
        /// </summary>
        private class ItalicRule : Rule
        {
            private string CreateHtml(Match elem)
            {
                string captured = elem.Value;
                return "<em>" + captured.Substring(1, captured.Length - 2 ) + "</em>";
            }

            public override string Process(string input)
            {
                return Regex.Replace(input, @"(\*|_)(.+?)\1", CreateHtml);
            }
        }

        /// <summary>
        /// Represents a rule for parsing Bold (Strong) elemets.
        /// </summary>
        private class BoldRule : Rule
        {
            private string CreateHtml(Match elem)
            {
                string captured = elem.Value;
                return "<strong>" + captured.Substring(2, captured.Length - 4 ) + "</strong>";
            }

            public override string Process(string input)
            {
                return Regex.Replace(input, @"(\*{2}|_{2})(.+?)\1", CreateHtml);
            }
        }

        /// <summary>
        /// Represents a rule for parsing Paragraphs.
        /// </summary>
        private class ParagraphRule : Rule
        {
            private string CreateHtml(Match elem)
            {
                string matched = elem.Value;

                // If the match starts with one of the tags, means that it cannot be a paragraph
                if (Regex.Match(matched, @"^\n?\s?</?(h|ol|ul|li)").Success) // Check for conflicts, there are some cases where we cannot have a paragraph.
                    return matched;

                // If there is any of the tags below inside the paragraph we remove them.
                matched = Regex.Replace(matched, @"</?(h|ol|ul|li)>\n?", "");

                return "<p>"+ matched.Trim() + "</p>";
            }

            public override string Process(string input)
            {
                return Regex.Replace(input, @"(?<=((\n{2}|(^\n))))[^\s#\-\+\*(\d\.\s)](.|\n)*?(?=(\n{2}|$))", CreateHtml);
            }
        }

        /// <summary>
        /// Represents a rule for parsing Headers.
        /// </summary>
        private class HeaderRule : Rule
        {
            public string CreateHtml(Match elem)
            {
                Match front = Regex.Match(elem.Value, @"^\n?#+");
                Match back = Regex.Match(elem.Value, "#+?\n?$");
                string header = Regex.Match(front.Value, "#+").Length.ToString();
                string content = elem.Value;
                content = !back.Success? content.Substring(front.Length) : content.Substring(front.Length, back.Index - front.Length);
                return "\n<h"+ header + ">"+ content.Trim() + "</h"+ header + ">\n";
            }

            public override string Process(string input)
            {
                return Regex.Replace(input, @"\n(#+).*", CreateHtml);
            }
        }

        /// <summary>
        /// Abstract class that represents a rule.
        /// </summary>
        private abstract class Rule 
        {   
            public abstract string Process(String input);
        }
    }
}