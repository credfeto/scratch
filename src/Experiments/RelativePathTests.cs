using System.Collections.Generic;
using System.IO;
using System.Linq;
using FunFair.Test.Common;
using Xunit;
using Xunit.Abstractions;

namespace Experiments;

public sealed class RelativePathTests : LoggingTestBase
{
    public RelativePathTests(ITestOutputHelper output)
        : base(output)
    {
    }

    private static string GetRelativePath(string documentFullPath, string referencedFileFullPath)
    {
        IReadOnlyList<string> documentPathParts = documentFullPath.Split(Path.DirectorySeparatorChar);
        IReadOnlyList<string> referencedFilePathParts = referencedFileFullPath.Split(Path.DirectorySeparatorChar);

        int commonLength = documentPathParts.TakeWhile((t, i) => t == referencedFilePathParts[i])
                                            .Count();

        if (commonLength == referencedFilePathParts.Count && documentPathParts.Count == commonLength)
        {
            return documentPathParts[commonLength - 1];
        }

        int upDir = documentPathParts.Count - commonLength - 1;

        return string.Join(Path.DirectorySeparatorChar.ToString(),
                           Enumerable.Repeat(element: "..", count: upDir)
                                     .Concat(referencedFilePathParts.Skip(commonLength)));
    }

    [Theory]
    [InlineData("/index.html", "/index.html", "index.html")]
    [InlineData("/index.html", "/image.jpg", "image.jpg")]
    [InlineData("/index.html", "/folder/index.html", "folder/index.html")]
    [InlineData("/folder/index.html", "/folder/index.html", "index.html")]
    [InlineData("/folder/index.html", "/folder/image.jpg", "image.jpg")]
    [InlineData("/folder1/index.html", "/folder2/index.html", "../folder2/index.html")]
    [InlineData("/folder1/folder1a/index.html", "/folder2/index.html", "../../folder2/index.html")]
    [InlineData("/folder1/index.html", "/folder2/folder2a/index.html", "../folder2/folder2a/index.html")]
    public void ProducesSaneRelativePath(string documentPath, string referencedFilePath, string expectedRelativePath)
    {
        string relativePath = GetRelativePath(documentFullPath: documentPath, referencedFileFullPath: referencedFilePath);

        this.Output.WriteLine($"Doc: {documentPath}");
        this.Output.WriteLine($"Ref: {referencedFilePath}");
        this.Output.WriteLine($"Rel: {relativePath}");
        this.Output.WriteLine($"Exp: {expectedRelativePath}");

        Assert.Equal(expected: expectedRelativePath, actual: relativePath);
    }
}