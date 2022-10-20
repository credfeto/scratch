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

        IReadOnlyList<StrippedFile> files = this.FindFiles(source);
        IReadOnlyList<StrippedFile> binaryFiles = GetBinaryFiles(files);
        IReadOnlyList<StrippedFile> allTextFiles = GetAllTextFiles(files);

        Dictionary<string, string> fileHashes = await this.HashBinaryFilesAsync(binaryFiles);
        Dictionary<string, string> textFiles = await LoadTextFilesAsync(allTextFiles);
        IReadOnlyList<StrippedFile> renamableTextFiles = GetRenamableTextFiles(allTextFiles);
        IReadOnlyList<StrippedFile> fixedNameTextFiles = GetFixedNameTextFiles(allTextFiles);

        this.ReplaceBinaryFilesInTextFiles(renamableTextFiles: renamableTextFiles, textFiles: textFiles, binaryFiles: binaryFiles, fileHashes: fileHashes);

        this.ReplaceTextFilesInRenamableTextFiles(renamableTextFiles: renamableTextFiles, fileHashes: fileHashes, textFiles: textFiles);

        this.ChangeReferencesInNonRenamableTextFiles(fixedNameTextFiles: fixedNameTextFiles, textFiles: textFiles, fileHashes: fileHashes);

        this.SaveFiles(destination: destination, binaryFiles: binaryFiles, fileHashes: fileHashes, allTextFiles: allTextFiles);
    }

    private static IReadOnlyList<StrippedFile> GetFixedNameTextFiles(IReadOnlyList<StrippedFile> allTextFiles)
    {
        return allTextFiles.Where(f => !f.IsRenamable)
                           .ToArray();
    }

    private static IReadOnlyList<StrippedFile> GetRenamableTextFiles(IReadOnlyList<StrippedFile> allTextFiles)
    {
        return allTextFiles.Where(f => f.IsRenamable)
                           .ToArray();
    }

    private static IReadOnlyList<StrippedFile> GetAllTextFiles(IReadOnlyList<StrippedFile> files)
    {
        return files.Where(f => f.IsText)
                    .ToArray();
    }

    private static IReadOnlyList<StrippedFile> GetBinaryFiles(IReadOnlyList<StrippedFile> files)
    {
        return files.Where(f => !f.IsText && f.IsRenamable)
                    .ToArray();
    }

    private IReadOnlyList<StrippedFile> FindFiles(string source)
    {
        return Directory.GetFiles(path: source, searchPattern: "*", searchOption: SearchOption.AllDirectories)
                        .Select(p => this.GetStrippedFile(sourceBasePath: source, fileName: p))
                        .OrderBy(s => s.Path)
                        .ToArray();
    }

    private void SaveFiles(string destination, IReadOnlyList<StrippedFile> binaryFiles, Dictionary<string, string> fileHashes, IReadOnlyList<StrippedFile> allTextFiles)
    {
        this._logger.LogInformation($"Writing files to {destination}");

        foreach (StrippedFile binaryFile in binaryFiles)
        {
            string hashedPath = fileHashes[binaryFile.Path];
            string destinationPath = Path.Combine(path1: destination, path2: hashedPath);
            this._logger.LogInformation($"*: Writing {binaryFile.Path} to {destinationPath}");
        }

        foreach (StrippedFile textFile in allTextFiles)
        {
            string hashedPath = fileHashes[textFile.Path];
            string destinationPath = Path.Combine(path1: destination, path2: hashedPath);
            this._logger.LogInformation($"*: Writing {textFile.Path} to {destinationPath}");
        }
    }

    private void ChangeReferencesInNonRenamableTextFiles(IReadOnlyList<StrippedFile> fixedNameTextFiles, Dictionary<string, string> textFiles, Dictionary<string, string> fileHashes)
    {
        this._logger.LogInformation("Changing references in non-renamable text files");

        foreach (StrippedFile file in fixedNameTextFiles)
        {
            this._logger.LogInformation($"*: {file.Path}");
            bool changed = false;
            string content = textFiles[file.Path];

            foreach ((string hashedFilePath, string newHashedPath) in fileHashes)
            {
                string relative = PathHelpers.GetRelativePath(documentFullPath: file.Path, referencedFileFullPath: hashedFilePath);

                if (content.Contains(value: relative, comparisonType: StringComparison.Ordinal))
                {
                    this._logger.LogInformation($"  --> Adjusting {hashedFilePath} to {newHashedPath}");
                    string newRelative = PathHelpers.GetRelativePath(documentFullPath: file.Path, referencedFileFullPath: newHashedPath);

                    content = content.Replace(oldValue: relative, newValue: newRelative, comparisonType: StringComparison.Ordinal);
                    changed = true;
                }
            }

            if (changed)
            {
                textFiles[file.Path] = content;
            }

            fileHashes.Add(key: file.Path, value: file.Path);
        }
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
                bool hasReferenceToOtherFiles = false;

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
                        hasReferenceToOtherFiles = true;

                        break;
                    }
                }

                if (!hasReferenceToOtherFiles)
                {
                    string hashRelative = this.HashRenamableContent(fileHashes: fileHashes, content: content, file: file);

                    MakeReplacement(renamableTextFiles: renamableTextFiles, fileHashes: fileHashes, textFiles: textFiles, file: file, hashRelative: hashRelative);

                    changes = true;
                }
                else
                {
                    this._logger.LogInformation($"*: {file.RootRelativePath} : Has references to other files");
                }
            }
        } while (changes);

        this.AddRemainingRenamableFiles(renamableTextFiles: renamableTextFiles, fileHashes: fileHashes, textFiles: textFiles);
    }

    private string HashRenamableContent(Dictionary<string, string> fileHashes, string content, StrippedFile file)
    {
        string hash = HashFileContent(Encoding.UTF8.GetBytes(content));
        string hashRelative = this.MakeRelativeHashFileName(file: file, hash: hash);

        this._logger.LogInformation($"*: {file.RootRelativePath} : {hashRelative}");

        fileHashes.Add(key: file.Path, value: hashRelative);

        return hashRelative;
    }

    private void AddRemainingRenamableFiles(IReadOnlyList<StrippedFile> renamableTextFiles, Dictionary<string, string> fileHashes, Dictionary<string, string> textFiles)
    {
        foreach (StrippedFile file in renamableTextFiles)
        {
            if (fileHashes.ContainsKey(file.Path))
            {
                continue;
            }

            string content = textFiles[file.Path];
            this.HashRenamableContent(fileHashes: fileHashes, content: content, file: file);
        }
    }

    private static void MakeReplacement(IReadOnlyList<StrippedFile> renamableTextFiles,
                                        Dictionary<string, string> fileHashes,
                                        Dictionary<string, string> textFiles,
                                        StrippedFile file,
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

            if (referencingContent.Contains(value: relativeInReferencing, comparisonType: StringComparison.Ordinal))
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