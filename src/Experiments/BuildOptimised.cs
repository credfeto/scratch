using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FunFair.Test.Common;
using Xunit;
using Xunit.Abstractions;

namespace Experiments;

public sealed class BuildOptimised : LoggingTestBase
{
    private const string SOURCE = "/home/markr/work/scratch/";
    private const string DESTINATION = "/home/markr/work/scratch-dest/";
    private readonly IHashedFileDetector _hashedFileDetector;

    public BuildOptimised(ITestOutputHelper outpout)
        : base(outpout)
    {
        this._hashedFileDetector = new HashedFileDetector();
    }

    [Fact]
    public async Task BuildAsync()
    {
        await Task.CompletedTask;

        if (Directory.Exists(DESTINATION))
        {
            Directory.Delete(path: DESTINATION, recursive: true);
        }

        Directory.CreateDirectory(DESTINATION);

        IReadOnlyList<StrippedFile> files = Directory.GetFiles(path: SOURCE, searchPattern: "*", searchOption: SearchOption.AllDirectories)
                                                     .Select(this.GetStrippedFile)
                                                     .OrderBy(s => s.Path)
                                                     .ToArray();

        this.Output.WriteLine("Content:");

        foreach (StrippedFile file in files)
        {
            this.Output.WriteLine($"*: {file.RootRelativePath}");
        }

        IReadOnlyList<StrippedFile> renamableFile = files.Where(f => f.IsRenamable)
                                                         .ToArray();

        IReadOnlyList<StrippedFile> fixedNameFiles = files.Where(f => !f.IsRenamable)
                                                          .ToArray();

        this.Output.WriteLine("Renamable:");

        foreach (StrippedFile file in renamableFile)
        {
            this.Output.WriteLine($"*: {file.RootRelativePath}");
        }

        this.Output.WriteLine("Fixed Names:");

        foreach (StrippedFile file in fixedNameFiles)
        {
            this.Output.WriteLine($"*: {file.RootRelativePath}");
        }

        IReadOnlyList<StrippedFile> binaryFiles = files.Where(f => !f.IsText && f.IsRenamable)
                                                       .ToArray();

        Dictionary<string, string> fileHashes = await this.HashBinaryFilesAsync(binaryFiles);

        IReadOnlyList<StrippedFile> allTextFiles = files.Where(f => f.IsText)
                                                        .ToArray();
        Dictionary<string, string> textFiles = await LoadTextFilesAsync(allTextFiles);

        IReadOnlyList<StrippedFile> renamableTextFiles = allTextFiles.Where(f => f.IsRenamable)
                                                                     .ToArray();

        this.ReplaceBinaryFilesInTextFiles(renamableTextFiles: renamableTextFiles, textFiles: textFiles, binaryFiles: binaryFiles, fileHashes: fileHashes);

        this.ReplaceTextFilesInRenamableTextFiles(renamableTextFiles: renamableTextFiles, fileHashes: fileHashes, textFiles: textFiles);
    }

    private void ReplaceTextFilesInRenamableTextFiles(IReadOnlyList<StrippedFile> renamableTextFiles, Dictionary<string, string> fileHashes, Dictionary<string, string> textFiles)
    {
        int pass = 0;
        bool changes;

        do
        {
            ++pass;
            this.Output.WriteLine($"{nameof(this.ReplaceTextFilesInRenamableTextFiles)}: Pass {pass}");
            changes = false;

            foreach (StrippedFile file in renamableTextFiles)
            {
                bool hasReferenecesToOtherFiles = false;

                if (fileHashes.ContainsKey(file.Path))
                {
                    // already
                    continue;
                }

                string content = textFiles[file.Path];

                foreach (StrippedFile referencedFile in renamableTextFiles)
                {
                    if (referencedFile.Path == file.Path)
                    {
                        continue;
                    }

                    string relative = GetRelativePath(documentPath: file.Path, referencedFilePath: referencedFile.Path);

                    if (content.Contains(value: relative, comparisonType: StringComparison.Ordinal))
                    {
                        hasReferenecesToOtherFiles = true;

                        break;
                    }
                }

                if (!hasReferenecesToOtherFiles)
                {
                    string hash = HashFileContent(Encoding.UTF8.GetBytes(content));
                    string hashRelative = this.MakeRelativeHashFileName(file: file, hash: hash);

                    this.Output.WriteLine($"*: {file.RootRelativePath} : {hashRelative}");

                    fileHashes.Add(key: file.Path, value: hashRelative);

                    MakeReplacment(renamableTextFiles: renamableTextFiles, fileHashes: fileHashes, textFiles: textFiles, file: file, content: content, hashRelative: hashRelative);

                    changes = true;
                }
            }
        } while (changes);
    }

    private static void MakeReplacment(IReadOnlyList<StrippedFile> renamableTextFiles,
                                       Dictionary<string, string> fileHashes,
                                       Dictionary<string, string> textFiles,
                                       StrippedFile file,
                                       string content,
                                       string hashRelative)
    {
        foreach (StrippedFile referencing in renamableTextFiles)
        {
            if (fileHashes.ContainsKey(referencing.Path))
            {
                continue;
            }

            string relativeInReferencing = GetRelativePath(documentPath: referencing.Path, referencedFilePath: file.Path);
            string referencingContent = textFiles[referencing.Path];

            if (content.Contains(value: relativeInReferencing, comparisonType: StringComparison.Ordinal))
            {
                string newFileName = relativeInReferencing.Substring(startIndex: 0, relativeInReferencing.Length - file.FileName.Length) + hashRelative;
                referencingContent = referencingContent.Replace(oldValue: relativeInReferencing, newValue: newFileName, comparisonType: StringComparison.Ordinal);

                textFiles[referencing.Path] = referencingContent;
            }
        }
    }

    private async Task<Dictionary<string, string>> HashBinaryFilesAsync(IReadOnlyList<StrippedFile> binaryFiles)
    {
        this.Output.WriteLine("Hashed Binary:");

        Dictionary<string, string> fileHashes = new(StringComparer.Ordinal);

        foreach (StrippedFile file1 in binaryFiles)
        {
            string hash = await HashFileAsync(file1.Path);

            string hashRelative = this.MakeRelativeHashFileName(file: file1, hash: hash);

            this.Output.WriteLine($"*: {file1.RootRelativePath} : {hashRelative}");

            fileHashes.Add(key: file1.Path, value: hashRelative);
        }

        return fileHashes;
    }

    private void ReplaceBinaryFilesInTextFiles(IReadOnlyList<StrippedFile> renamableTextFiles,
                                               Dictionary<string, string> textFiles,
                                               IReadOnlyList<StrippedFile> binaryFiles,
                                               Dictionary<string, string> fileHashes)
    {
        int pass = 0;
        bool changes;

        do
        {
            ++pass;
            this.Output.WriteLine($"{nameof(this.ReplaceBinaryFilesInTextFiles)}: Pass {pass}");
            changes = false;

            foreach (StrippedFile file in renamableTextFiles)
            {
                this.Output.WriteLine($"Checking {file.Path}...");
                string content = textFiles[file.Path];

                bool hasChange = false;

                foreach (StrippedFile binaryFile in binaryFiles)
                {
                    string hashedBinary = fileHashes[binaryFile.Path];

                    string search = "/" + binaryFile.RootRelativePath;

                    if (content.Contains(value: search, comparisonType: StringComparison.Ordinal))
                    {
                        content = content.Replace(oldValue: search, "/" + hashedBinary, comparisonType: StringComparison.Ordinal);
                        hasChange = true;
                    }

                    string relative = GetRelativePath(documentPath: file.Path, referencedFilePath: binaryFile.Path);

                    if (content.Contains(value: relative, comparisonType: StringComparison.Ordinal))
                    {
                        string newFileName = relative.Substring(startIndex: 0, relative.Length - binaryFile.FileName.Length) + hashedBinary;
                        content = content.Replace(oldValue: relative, newValue: newFileName, comparisonType: StringComparison.Ordinal);
                        hasChange = true;
                    }
                }

                if (hasChange)
                {
                    this.Output.WriteLine($"*: {file.RootRelativePath} has content changes");
                    textFiles[file.Path] = content;
                    changes = true;
                }

                changes |= hasChange;
            }
        } while (changes);
    }

    private static string GetRelativePath(string documentPath, string referencedFilePath)
    {
        string[] documentPathParts = documentPath.Split(Path.DirectorySeparatorChar);
        string[] referencedFilePathParts = referencedFilePath.Split(Path.DirectorySeparatorChar);

        int commonLength = documentPathParts.TakeWhile((t, i) => t == referencedFilePathParts[i])
                                            .Count();

        return string.Join(Path.DirectorySeparatorChar.ToString(), referencedFilePathParts.Skip(commonLength));
    }

    private static async Task<Dictionary<string, string>> LoadTextFilesAsync(IReadOnlyList<StrippedFile> allTextFiles)
    {
        Dictionary<string, string> textFiles = new(StringComparer.Ordinal);

        foreach (StrippedFile file in allTextFiles)
        {
            string contents = await File.ReadAllTextAsync(file.Path);

            textFiles.Add(key: file.Path, value: contents);
        }

        return textFiles;
    }

    private string MakeRelativeHashFileName(StrippedFile file, string hash)
    {
        string path = file.RootRelativePath.Substring(startIndex: 0, file.RootRelativePath.Length - file.FileName.Length);

        if (this._hashedFileDetector.IsHashedFileName(path))
        {
            return path + RemoveHash(file) + "mrhash." + hash + "." + file.Extension;
        }

        return path + file.FileName.Substring(startIndex: 0, file.FileName.Length - file.Extension.Length) + "mrhash." + hash + "." + file.Extension;
    }

    private static string RemoveHash(StrippedFile file)
    {
        return file.FileName.Substring(startIndex: 0, file.FileName.Length - file.Extension.Length);
    }

    private static async Task<string> HashFileAsync(string filePath)
    {
        byte[] b = await File.ReadAllBytesAsync(filePath);

        return HashFileContent(b);
    }

    private static string HashFileContent(byte[] b)
    {
        byte[] x = SHA256.HashData(b);

        return BitConverter.ToString(x)
                           .ToLowerInvariant()
                           .Replace(oldValue: "-", newValue: string.Empty, comparisonType: StringComparison.Ordinal);
    }

    private StrippedFile GetStrippedFile(string fileName)
    {
        string f = Path.GetFileName(fileName);
        string e = Path.GetExtension(fileName)
                       .TrimStart('.');

        return new(IsRenamable: !IsFixedResourceName(f),
                   IsText: IsText(e),
                   HasHash: this._hashedFileDetector.IsHashedFileName(fileName),
                   Path: fileName,
                   RootRelativePath: fileName.Substring(SOURCE.Length),
                   FileName: f,
                   Extension: e);
    }

    private static bool IsFixedResourceName(string fileName)
    {
        return fileName is "index.html" or "favicon.ico" or "manifest.json" or "robots.txt" or "mimetypes.json" or "packagemanifest.json" or "routing.json";
    }

    private static bool IsText(string extension)
    {
        return extension is "html" or "js" or "css" or "json" or "txt" or "xml" or "svg" or "map";
    }

    [DebuggerDisplay("{Path}")]
    private sealed record StrippedFile(string Path, string RootRelativePath, string FileName, string Extension, bool IsRenamable, bool IsText, bool HasHash);

    /// <summary>
    ///     Detects whether files have a hash in their filename so they can be treated specially when caching.
    /// </summary>
    private interface IHashedFileDetector
    {
        /// <summary>
        ///     Checks to see if the given filename contains a hash.
        /// </summary>
        /// <param name="fileName">The filename.</param>
        /// <returns>True, if the filename contains a hash; otherwise, false.</returns>
        bool IsHashedFileName(string fileName);
    }

    private sealed class HashedFileDetector : IHashedFileDetector
    {
        private static readonly IReadOnlyList<string> NonHexSuffix = new[]
                                                                     {
                                                                         "bundle",
                                                                         "chunked",
                                                                         "chunk"
                                                                     };

        /// <inheritdoc />
        public bool IsHashedFileName(string fileName)
        {
            for (string filenameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                 filenameWithoutExtension.Contains(value: '.', comparisonType: StringComparison.Ordinal);
                 filenameWithoutExtension = Path.GetFileNameWithoutExtension(filenameWithoutExtension))

            {
                string hash = ExtractHashFromFileName(filenameWithoutExtension);

                if (string.IsNullOrWhiteSpace(hash))
                {
                    return false;
                }

                if (IsNonHexSuffix(hash))
                {
                    continue;
                }

                return IsValidHash(hash);
            }

            return false;
        }

        private static bool IsValidHash(string hash)
        {
            return hash.All(IsHexDigit);
        }

        private static bool IsNonHexSuffix(string hash)
        {
            return NonHexSuffix.Any(suffix => StringComparer.InvariantCultureIgnoreCase.Equals(x: hash, y: suffix));
        }

        private static bool IsHexDigit(char x)
        {
            return x is >= '0' and <= '9' or >= 'a' and <= 'f' or >= 'A' and <= 'F';
        }

        private static string ExtractHashFromFileName(string filenameWithoutExtension)
        {
            return Path.GetExtension(filenameWithoutExtension)
                       .TrimStart(trimChar: '.');
        }
    }
}