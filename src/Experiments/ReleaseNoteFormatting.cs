using System;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace Experiments
{
    public class ReleaseNoteFormatting
    {
        public ReleaseNoteFormatting(ITestOutputHelper output)
        {
            _output = output;
        }

        private readonly ITestOutputHelper _output;

        private const string SIMPLE = @"### Added
- Some Stuff
- Some Other Stuff
### Changed
- FF-1324 - some text
";

        private static string Bold(string value)
        {
            return "**" + value + "**";
        }

        private static string Underline(string value)
        {
            return "__" + value + "__";
        }


        [Fact]
        public void Convert()
        {
            _output.WriteLine(SIMPLE);

            _output.WriteLine("****************************************************");
            var builder = new StringBuilder();
            var text = SIMPLE.Split(Environment.NewLine);
            foreach (var line in text)
            {
                if (line.StartsWith("### "))
                {
                    var replacement = Bold(Underline(line.Substring(4).Trim()));
                    builder.AppendLine(replacement);
                    continue;
                }

                builder.AppendLine(Regex.Replace(line, "(ff\\-\\d+)", "_$1:_", RegexOptions.IgnoreCase).Trim());
            }


            _output.WriteLine(builder.ToString());
        }
    }
}