using Experiments.Helpers;
using FunFair.Test.Common;
using Xunit;

namespace Experiments;

public sealed class RelativePathTests : LoggingTestBase
{
    public RelativePathTests(ITestOutputHelper output)
        : base(output) { }

    [Theory]
    [InlineData("/index.html", "/index.html", "index.html")]
    [InlineData("/index.html", "/image.jpg", "image.jpg")]
    [InlineData("/index.html", "/folder/index.html", "folder/index.html")]
    [InlineData("/folder/index.html", "/folder/index.html", "index.html")]
    [InlineData("/folder/index.html", "/folder/image.jpg", "image.jpg")]
    [InlineData("/folder1/index.html", "/folder2/index.html", "../folder2/index.html")]
    [InlineData("/folder1/folder1a/index.html", "/folder2/index.html", "../../folder2/index.html")]
    [InlineData(
        "/folder1/index.html",
        "/folder2/folder2a/index.html",
        "../folder2/folder2a/index.html"
    )]
    public void ProducesSaneRelativePath(
        string documentPath,
        string referencedFilePath,
        string expectedRelativePath
    )
    {
        string relativePath = PathHelpers.GetRelativePath(
            documentFullPath: documentPath,
            referencedFileFullPath: referencedFilePath
        );

        this.Output.WriteLine($"Doc: {documentPath}");
        this.Output.WriteLine($"Ref: {referencedFilePath}");
        this.Output.WriteLine($"Rel: {relativePath}");
        this.Output.WriteLine($"Exp: {expectedRelativePath}");

        Assert.Equal(expected: expectedRelativePath, actual: relativePath);
    }
}
