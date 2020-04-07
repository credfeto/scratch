using System;
using Xunit;
using Xunit.Abstractions;

namespace Experiments
{
    public class ReleaseNoteFormatting
    {
        private readonly ITestOutputHelper _output;

        public ReleaseNoteFormatting( ITestOutputHelper output)
        {
            _output = output;
        }
        
        const string SIMPLE = @"### Added
- Some Stuff
- Some Other Stuff
### Changed
- FF-1324 - some text
";
            
        
        [Fact]
        public void Convert()
        {
            _output.WriteLine(SIMPLE);
        }
    }
}