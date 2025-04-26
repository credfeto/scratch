using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Experiments.Helpers;
using Microsoft.Extensions.Logging;

namespace Experiments.Implementations;

public sealed class HashedContentOptimizer : IHashedContentOptimizer
{
    private static readonly IReadOnlyList<string> FixedResourceNames =
    [
        "index.html",
        "favicon.ico",
        "manifest.json",
        "robots.txt",
        "mimetypes.json",
        "packagemanifest.json",
        "routing.json",
        "api.json",
    ];

    private static readonly IReadOnlyList<string> TextExtensions =
    [
        "html",
        "js",
        "css",
        "json",
        "txt",
        "xml",
        "svg",
        "map",
    ];

    private readonly IHashedFileDetector _hashedFileDetector;
    private readonly ILogger<HashedContentOptimizer> _logger;

    public HashedContentOptimizer(IHashedFileDetector hashedFileDetector, ILogger<HashedContentOptimizer> logger)
    {
        this._hashedFileDetector = hashedFileDetector;
        this._logger = logger;
    }

    public async Task OptimizeAsync(string source, string destination, CancellationToken cancellationToken)
    {
        if (Directory.Exists(destination))
        {
            Directory.Delete(path: destination, recursive: true);
        }

        Directory.CreateDirectory(destination);

        IReadOnlyList<StrippedFile> files = this.FindFiles(source);
        IReadOnlyList<StrippedFile> binaryFiles = GetBinaryFiles(files);
        IReadOnlyList<StrippedFile> allTextFiles = GetAllTextFiles(files);

        Dictionary<string, string> fileHashes = await this.HashBinaryFilesAsync(
            binaryFiles: binaryFiles,
            cancellationToken: cancellationToken
        );
        Dictionary<string, string> textFiles = await LoadTextFilesAsync(
            allTextFiles: allTextFiles,
            cancellationToken: cancellationToken
        );
        IReadOnlyList<StrippedFile> renamableTextFiles = GetRenamableTextFiles(allTextFiles);
        IReadOnlyList<StrippedFile> fixedNameTextFiles = GetFixedNameTextFiles(allTextFiles);

        this.ReplaceBinaryFilesInTextFiles(
            renamableTextFiles: renamableTextFiles,
            textFiles: textFiles,
            binaryFiles: binaryFiles,
            fileHashes: fileHashes
        );

        this.ReplaceTextFilesInRenamableTextFiles(
            renamableTextFiles: renamableTextFiles,
            fileHashes: fileHashes,
            textFiles: textFiles
        );

        this.ChangeReferencesInNonRenamableTextFiles(
            fixedNameTextFiles: fixedNameTextFiles,
            textFiles: textFiles,
            fileHashes: fileHashes
        );

        await this.SaveFilesAsync(
            source: source,
            destination: destination,
            binaryFiles: binaryFiles,
            fileHashes: fileHashes,
            allTextFiles: allTextFiles,
            textFiles: textFiles,
            cancellationToken: cancellationToken
        );
    }

    private static IReadOnlyList<StrippedFile> GetFixedNameTextFiles(IReadOnlyList<StrippedFile> allTextFiles)
    {
        return [.. allTextFiles.Where(f => !f.IsRenamable)];
    }

    private static IReadOnlyList<StrippedFile> GetRenamableTextFiles(IReadOnlyList<StrippedFile> allTextFiles)
    {
        return [.. allTextFiles.Where(f => f.IsRenamable)];
    }

    private static IReadOnlyList<StrippedFile> GetAllTextFiles(IReadOnlyList<StrippedFile> files)
    {
        return [.. files.Where(f => f.IsText)];
    }

    private static IReadOnlyList<StrippedFile> GetBinaryFiles(IReadOnlyList<StrippedFile> files)
    {
        return [.. files.Where(f => f is { IsText: false, IsRenamable: true })];
    }

    private IReadOnlyList<StrippedFile> FindFiles(string source)
    {
        return
        [
            .. Directory
                .EnumerateFiles(path: source, searchPattern: "*", searchOption: SearchOption.AllDirectories)
                .Select(p => this.GetStrippedFile(sourceBasePath: source, fileName: p))
                .OrderBy(keySelector: s => s.Path, comparer: StringComparer.Ordinal),
        ];
    }

    private async Task SaveFilesAsync(
        string source,
        string destination,
        IReadOnlyList<StrippedFile> binaryFiles,
        Dictionary<string, string> fileHashes,
        IReadOnlyList<StrippedFile> allTextFiles,
        Dictionary<string, string> textFiles,
        CancellationToken cancellationToken
    )
    {
        this._logger.LogInformation($"Writing files to {destination}");

        foreach (StrippedFile binaryFile in binaryFiles)
        {
            string destinationPath = this.GetDestinationPath(
                source: source,
                destination: destination,
                fileHashes: fileHashes,
                file: binaryFile
            );
            this._logger.LogInformation($"*: Writing {binaryFile.Path} to {destinationPath}");

            EnsureFolderExists(destinationPath);
            File.Copy(sourceFileName: binaryFile.Path, destFileName: destinationPath, overwrite: true);
        }

        foreach (StrippedFile textFile in allTextFiles)
        {
            string destinationPath = this.GetDestinationPath(
                source: source,
                destination: destination,
                fileHashes: fileHashes,
                file: textFile
            );
            this._logger.LogInformation($"*: Writing {textFile.Path} to {destinationPath}");

            string content = textFiles[textFile.Path];
            EnsureFolderExists(destinationPath);
            await File.WriteAllTextAsync(
                path: destinationPath,
                contents: content,
                encoding: Encoding.UTF8,
                cancellationToken: cancellationToken
            );
        }
    }

    private string GetDestinationPath(
        string source,
        string destination,
        Dictionary<string, string> fileHashes,
        StrippedFile file
    )
    {
        string hashedPath = fileHashes[file.Path];
        this._logger.LogInformation($"Generating Destination file for {hashedPath} from {source} to {destination}");
        string destinationPath = Path.Combine(path1: destination, hashedPath[source.Length..]);

        return destinationPath;
    }

    private static void EnsureFolderExists(string destinationPath)
    {
        string? dp = Path.GetDirectoryName(destinationPath);

        if (string.IsNullOrEmpty(dp))
        {
            throw new PathTooLongException($"Could not resolve path for {destinationPath}");
        }

        if (!Directory.Exists(dp))
        {
            Directory.CreateDirectory(dp);
        }
    }

    private void ChangeReferencesInNonRenamableTextFiles(
        IReadOnlyList<StrippedFile> fixedNameTextFiles,
        Dictionary<string, string> textFiles,
        Dictionary<string, string> fileHashes
    )
    {
        this._logger.LogInformation("Changing references in non-renamable text files");

        foreach (StrippedFile file in fixedNameTextFiles)
        {
            this._logger.LogInformation($"*: {file.Path}");
            bool changed = false;
            string content = textFiles[file.Path];

            foreach ((string hashedFilePath, string newHashedPath) in fileHashes)
            {
                string relative = PathHelpers.GetRelativePath(
                    documentFullPath: file.Path,
                    referencedFileFullPath: hashedFilePath
                );
                string newRelative = PathHelpers.GetRelativePath(
                    documentFullPath: file.Path,
                    referencedFileFullPath: newHashedPath
                );

                changed |= this.ChangeContent(
                    relative: relative,
                    newRelative: newRelative,
                    hashedFilePath: hashedFilePath,
                    newHashedPath: newHashedPath,
                    content: ref content
                );
            }

            if (changed)
            {
                textFiles[file.Path] = content;
            }

            fileHashes.Add(key: file.Path, value: file.Path);
        }
    }

    private bool ChangeContent(
        string relative,
        string newRelative,
        string hashedFilePath,
        string newHashedPath,
        ref string content
    )
    {
        Regex regex = GetRegex(relative);

        string changedContent = regex.Replace(input: content, evaluator: Evaluator);

        if (!StringComparer.Ordinal.Equals(x: content, y: changedContent))
        {
            this._logger.LogInformation($"  --> Adjusting {hashedFilePath} to {newHashedPath}");
            this._logger.LogInformation($"  --> Adjusting {relative} to {newRelative}");

            content = changedContent;

            return true;
        }

        return false;

        string Evaluator(Match m)
        {
            string txt = m.Value;

            if (string.IsNullOrEmpty(txt))
            {
                return string.Empty;
            }

            return Quote(txt[0].ToString(), txt[^1].ToString(), toQuote: newRelative);
        }
    }

    private static Regex GetRegex(string relative)
    {
        string escaped = Regex.Escape(relative);

        string expression = BuildExpression(escaped);

        Regex regex = new(
            pattern: expression,
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.ExplicitCapture,
            TimeSpan.FromSeconds(1)
        );

        return regex;
    }

    private static string BuildExpression(string escapedFilename)
    {
        return string.Join(separator: '|', BuildCaptures(escapedFilename));
    }

    private static IEnumerable<string> BuildCaptures(string escapedFilename)
    {
        yield return Quote(before: "\"", after: "\"", Capture(groupName: "FileDoubleQuote", pattern: escapedFilename));
        yield return Quote(before: "'", after: "'", Capture(groupName: "FileSingleQuote", pattern: escapedFilename));
        yield return Quote(before: "(\\", after: "\\)", Capture(groupName: "FileBraces", pattern: escapedFilename));
    }

    private static string Capture(string groupName, string pattern)
    {
        return string.Concat("(?<", groupName, ">", pattern, ")");
    }

    private static string Quote(string before, string after, string toQuote)
    {
        return string.Concat(str0: before, str1: toQuote, str2: after);
    }

    private void ReplaceTextFilesInRenamableTextFiles(
        IReadOnlyList<StrippedFile> renamableTextFiles,
        Dictionary<string, string> fileHashes,
        Dictionary<string, string> textFiles
    )
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
                if (fileHashes.ContainsKey(file.Path))
                {
                    // already
                    continue;
                }

                string content = textFiles[file.Path];

                bool hasReferenceToOtherFiles = HasReferenceToOtherFiles(
                    renamableTextFiles: renamableTextFiles,
                    file: file,
                    content: content
                );

                if (!hasReferenceToOtherFiles)
                {
                    this.MakeHashRelativeReplacement(
                        renamableTextFiles: renamableTextFiles,
                        fileHashes: fileHashes,
                        textFiles: textFiles,
                        content: content,
                        file: file
                    );

                    changes = true;
                }
                else
                {
                    this._logger.LogInformation($"*: {file.RootRelativePath} : Has references to other files");
                }
            }
        } while (changes);

        this.AddRemainingRenamableFiles(
            renamableTextFiles: renamableTextFiles,
            fileHashes: fileHashes,
            textFiles: textFiles
        );
    }

    private static bool HasReferenceToOtherFiles(
        IReadOnlyList<StrippedFile> renamableTextFiles,
        StrippedFile file,
        string content
    )
    {
        return renamableTextFiles
            .Where(referencedFile => !StringComparer.Ordinal.Equals(x: referencedFile.Path, y: file.Path))
            .Select(referencedFile =>
                PathHelpers.GetRelativePath(documentFullPath: file.Path, referencedFileFullPath: referencedFile.Path)
            )
            .Select(GetRegex)
            .Any(regex => regex.IsMatch(content));
    }

    private void MakeHashRelativeReplacement(
        IReadOnlyList<StrippedFile> renamableTextFiles,
        Dictionary<string, string> fileHashes,
        Dictionary<string, string> textFiles,
        string content,
        StrippedFile file
    )
    {
        string hashRelative = this.HashRenamableContent(fileHashes: fileHashes, content: content, file: file);

        this.MakeReplacement(
            renamableTextFiles: renamableTextFiles,
            fileHashes: fileHashes,
            textFiles: textFiles,
            file: file,
            newHashedPath: hashRelative
        );
    }

    private string HashRenamableContent(Dictionary<string, string> fileHashes, string content, StrippedFile file)
    {
        string hash = HashFileContent(Encoding.UTF8.GetBytes(content));
        string hashedFile = this.MakeNewHashFileName(file: file, hash: hash);

        this._logger.LogInformation($"*: {file.Path} : {hashedFile}");

        fileHashes.Add(key: file.Path, value: hashedFile);

        return hashedFile;
    }

    private void AddRemainingRenamableFiles(
        IReadOnlyList<StrippedFile> renamableTextFiles,
        Dictionary<string, string> fileHashes,
        Dictionary<string, string> textFiles
    )
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

    private void MakeReplacement(
        IReadOnlyList<StrippedFile> renamableTextFiles,
        Dictionary<string, string> fileHashes,
        Dictionary<string, string> textFiles,
        StrippedFile file,
        string newHashedPath
    )
    {
        foreach (StrippedFile referencing in renamableTextFiles)
        {
            if (fileHashes.ContainsKey(referencing.Path))
            {
                continue;
            }

            string relativeInReferencing = PathHelpers.GetRelativePath(
                documentFullPath: referencing.Path,
                referencedFileFullPath: file.Path
            );
            string newRelative = PathHelpers.GetRelativePath(
                documentFullPath: referencing.Path,
                referencedFileFullPath: newHashedPath
            );
            string referencingContent = textFiles[referencing.Path];

            bool changed = this.ChangeContent(
                relative: relativeInReferencing,
                newRelative: newRelative,
                hashedFilePath: file.Path,
                newHashedPath: newHashedPath,
                content: ref referencingContent
            );

            if (changed)
            {
                textFiles[referencing.Path] = referencingContent;
            }
        }
    }

    private async Task<Dictionary<string, string>> HashBinaryFilesAsync(
        IReadOnlyList<StrippedFile> binaryFiles,
        CancellationToken cancellationToken
    )
    {
        this._logger.LogInformation("Hashed Binary:");

        Dictionary<string, string> fileHashes = new(StringComparer.Ordinal);

        foreach (StrippedFile file in binaryFiles)
        {
            string hash = await HashFileAsync(filePath: file.Path, cancellationToken: cancellationToken);

            string hashedFile = this.MakeNewHashFileName(file: file, hash: hash);

            this._logger.LogInformation($"*: {file.Path} : {hashedFile}");

            fileHashes.Add(key: file.Path, value: hashedFile);
        }

        return fileHashes;
    }

    private void ReplaceBinaryFilesInTextFiles(
        IReadOnlyList<StrippedFile> renamableTextFiles,
        Dictionary<string, string> textFiles,
        IReadOnlyList<StrippedFile> binaryFiles,
        Dictionary<string, string> fileHashes
    )
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
                    string relative = PathHelpers.GetRelativePath(
                        documentFullPath: file.Path,
                        referencedFileFullPath: binaryFile.Path
                    );
                    string newRelative = string.Concat(
                        relative.AsSpan(start: 0, relative.Length - binaryFile.FileName.Length),
                        str1: hashedBinary
                    );

                    hasChange |= this.ChangeContent(
                        relative: relative,
                        newRelative: newRelative,
                        hashedFilePath: binaryFile.Path,
                        newHashedPath: hashedBinary,
                        content: ref content
                    );
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

    private static async Task<Dictionary<string, string>> LoadTextFilesAsync(
        IReadOnlyList<StrippedFile> allTextFiles,
        CancellationToken cancellationToken
    )
    {
        Dictionary<string, string> textFiles = new(StringComparer.Ordinal);

        foreach (StrippedFile file in allTextFiles)
        {
            string contents = await File.ReadAllTextAsync(path: file.Path, cancellationToken: cancellationToken);

            textFiles.Add(key: file.Path, value: contents);
        }

        return textFiles;
    }

    private string MakeNewHashFileName(StrippedFile file, string hash)
    {
        string path = file.Path[..^file.FileName.Length];

        if (this._hashedFileDetector.IsHashedFileName(path))
        {
            return path + RemoveHash(file) + "mrhash." + hash + "." + file.Extension;
        }

        return path + file.FileName[..^file.Extension.Length] + "mrhash." + hash + "." + file.Extension;
    }

    private static string RemoveHash(StrippedFile file)
    {
        return file.FileName[..^file.Extension.Length];
    }

    private static async Task<string> HashFileAsync(string filePath, CancellationToken cancellationToken)
    {
        byte[] b = await File.ReadAllBytesAsync(path: filePath, cancellationToken: cancellationToken);

        return HashFileContent(b);
    }

    private static string HashFileContent(byte[] b)
    {
        byte[] x = SHA256.HashData(b);

        return Convert.ToHexString(x).ToLowerInvariant();
    }

    private StrippedFile GetStrippedFile(string sourceBasePath, string fileName)
    {
        string f = Path.GetFileName(fileName);
        string e = Path.GetExtension(fileName).TrimStart('.');

        return new(
            Path: fileName,
            fileName[sourceBasePath.Length..],
            FileName: f,
            Extension: e,
            !IsFixedResourceName(f),
            IsText(e),
            this._hashedFileDetector.IsHashedFileName(fileName)
        );
    }

    private static bool IsFixedResourceName(string fileName)
    {
        return FixedResourceNames.Contains(value: fileName, comparer: StringComparer.Ordinal);
    }

    private static bool IsText(string extension)
    {
        return TextExtensions.Contains(value: extension, comparer: StringComparer.Ordinal);
    }

    [DebuggerDisplay("{Path}")]
    private sealed record StrippedFile(
        string Path,
        string RootRelativePath,
        string FileName,
        string Extension,
        bool IsRenamable,
        bool IsText,
        bool HasHash
    );
}
