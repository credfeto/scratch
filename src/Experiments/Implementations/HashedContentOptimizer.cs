using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Experiments.Helpers;
using Microsoft.Extensions.Logging;

namespace Experiments.Implementations;

public sealed class HashedContentOptimizer : IHashedContentOptimizer
{
    private readonly IHashedFileDetector _hashedFileDetector;
    private readonly ILogger<HashedContentOptimizer> _logger;

    public HashedContentOptimizer(IHashedFileDetector hashedFileDetector, ILogger<HashedContentOptimizer> logger)
    {
        this._hashedFileDetector = hashedFileDetector;
        this._logger = logger;
    }

    public async Task OptimizeAsync(string source, string destination)
    {
        if (Directory.Exists(destination))
        {
            Directory.Delete(path: destination, recursive: true);
        }

        Directory.CreateDirectory(destination);

        IReadOnlyList<StrippedFile> files = Directory.GetFiles(path: source, searchPattern: "*", searchOption: SearchOption.AllDirectories)
                                                     .Select(p => this.GetStrippedFile(sourceBasePath: source, fileName: p))
                                                     .OrderBy(s => s.Path)
                                                     .ToArray();

        this._logger.LogInformation("Content:");

        foreach (StrippedFile file in files)
        {
            this._logger.LogInformation($"*: {file.RootRelativePath}");
        }

        IReadOnlyList<StrippedFile> renamableFile = files.Where(f => f.IsRenamable)
                                                         .ToArray();

        IReadOnlyList<StrippedFile> fixedNameFiles = files.Where(f => !f.IsRenamable)
                                                          .ToArray();

        this._logger.LogInformation("Renamable:");

        foreach (StrippedFile file in renamableFile)
        {
            this._logger.LogInformation($"*: {file.RootRelativePath}");
        }

        this._logger.LogInformation("Fixed Names:");

        foreach (StrippedFile file in fixedNameFiles)
        {
            this._logger.LogInformation($"*: {file.RootRelativePath}");
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
            this._logger.LogInformation($"{nameof(this.ReplaceTextFilesInRenamableTextFiles)}: Pass {pass}");
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

                    string relative = PathHelpers.GetRelativePath(documentFullPath: file.Path, referencedFileFullPath: referencedFile.Path);

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

                    this._logger.LogInformation($"*: {file.RootRelativePath} : {hashRelative}");

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

            string relativeInReferencing = PathHelpers.GetRelativePath(documentFullPath: referencing.Path, referencedFileFullPath: file.Path);
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
        this._logger.LogInformation("Hashed Binary:");

        Dictionary<string, string> fileHashes = new(StringComparer.Ordinal);

        foreach (StrippedFile file1 in binaryFiles)
        {
            string hash = await HashFileAsync(file1.Path);

            string hashRelative = this.MakeRelativeHashFileName(file: file1, hash: hash);

            this._logger.LogInformation($"*: {file1.RootRelativePath} : {hashRelative}");

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
            this._logger.LogInformation($"{nameof(this.ReplaceBinaryFilesInTextFiles)}: Pass {pass}");
            changes = false;

            foreach (StrippedFile file in renamableTextFiles)
            {
                this._logger.LogInformation($"Checking {file.Path}...");
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

                    string relative = PathHelpers.GetRelativePath(documentFullPath: file.Path, referencedFileFullPath: binaryFile.Path);

                    if (content.Contains(value: relative, comparisonType: StringComparison.Ordinal))
                    {
                        string newFileName = relative.Substring(startIndex: 0, relative.Length - binaryFile.FileName.Length) + hashedBinary;
                        content = content.Replace(oldValue: relative, newValue: newFileName, comparisonType: StringComparison.Ordinal);
                        hasChange = true;
                    }
                }

                if (hasChange)
                {
                    this._logger.LogInformation($"*: {file.RootRelativePath} has content changes");
                    textFiles[file.Path] = content;
                    changes = true;
                }

                changes |= hasChange;
            }
        } while (changes);
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

    private StrippedFile GetStrippedFile(string sourceBasePath, string fileName)
    {
        string f = Path.GetFileName(fileName);
        string e = Path.GetExtension(fileName)
                       .TrimStart('.');

        return new(IsRenamable: !IsFixedResourceName(f),
                   IsText: IsText(e),
                   HasHash: this._hashedFileDetector.IsHashedFileName(fileName),
                   Path: fileName,
                   RootRelativePath: fileName.Substring(sourceBasePath.Length),
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
}