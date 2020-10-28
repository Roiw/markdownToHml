# Simple MarkdownToHtml Parser
A simplified and easily extensible Markup to HTML parser. It offers a small set of rules.  This project is meant to be easy to extend and maintain at cost of performance.

That being said I heavily use regular expressions and string concatenation. A faster approach could be by parsing character by character and keeping context information. 

## What is currently not supported:

- Nested lists.
- Paragraphs nested inside of lists.
- Multiline list items.

Additionally, in order to make the output HTML more readable I add empty lines in between tags, except for the case of the \<li\>, \<strong\> and \<em\> tags.

## Expanding the project

This algorithm was designed to be easy to understand and upgrade. The parse process consists of three parts: Preprocess; Rule Parsing; Postprocess
Each rule inherits from an abstract class Rule and must implement a Parse method. The user can easily select which rules he would like to parse and add them to the rules array. Note that some rules have priority over others so ordering is important. 

This approach also facilitates expanding and maintaining the project. As it is easy to split tasks between developers (each developer working on a rule) and it's very easy to create unit tests for each rule.

## Running the project

In order to run the project:
- dotnet new console
- dotnet run

This will run the test cases defined on unitTests

Another possibility is to instantiate a MarkdownToHtml class and call ParseMarkdown providing an input and output file.
