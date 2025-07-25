using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Experiments.Helpers;
using Experiments.ReferenceObjects;
using FunFair.Test.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.IO;
using Xunit;

namespace Experiments;

public sealed class ResourceDllTests : LoggingFolderCleanupTestBase
{
    private const string CONTENT_RESOURCE_NAME = "Content";
    private const string ADDITIONAL_RESOURCE_NAME = "Additional";

    private static readonly string[] ExampleTempFiles = ["Example.txt", "Banana.jpg"];

    private static readonly RecyclableMemoryStreamManager MemoryStreamManager = new();

    public ResourceDllTests(ITestOutputHelper output)
        : base(output)
    {
    }

    [Fact]
    [SuppressMessage(category: "Microsoft.Design", checkId: "CA2000: Call dispose", Justification = "Because")]
    public async Task GenerateResourceDllAsync()
    {
        string source = Path.Combine(path1: this.TempFolder,
                                     Guid.NewGuid()
                                         .ToString());
        string target = Path.Combine(path1: this.TempFolder,
                                     Guid.NewGuid()
                                         .ToString());

        FolderHelpers.EnsureFolderExists(source);
        FolderHelpers.EnsureFolderExists(target);

        string filename = Path.Combine(path1: target, CONTENT_RESOURCE_NAME + ".resources");

        IReadOnlyList<string> filesToPack = await FileHelper.CreateTempFilesAsync(basePath: source, filenames: ExampleTempFiles, this.CancellationToken());

        await this.GenerateResourcesFromFilesAsync(filename: filename, filesToPack: filesToPack);

        Dictionary<string, string> additionalData = new(StringComparer.Ordinal)
                                                    {
                                                        ["Test1"] = Guid.NewGuid()
                                                                        .ToString(),
                                                        ["Test2"] = Guid.NewGuid()
                                                                        .ToString(),
                                                        [".jpg"] = "application/json"
                                                    };

        const string ns = "Credfeto.Resources.Example";
        ResourceDescription[] resources =
        [
            new(MakeFullyQualifiedResourceName(ns: ns, name: CONTENT_RESOURCE_NAME), dataProvider: () => File.OpenRead(filename), isPublic: true),
            new(MakeFullyQualifiedResourceName(ns: ns, name: ADDITIONAL_RESOURCE_NAME), dataProvider: () => GenerateResourcesFromDictionary(additionalData), isPublic: true)
        ];

        const string assemblyName = ns + ".dll";
        string assemblyFileName = Path.Combine(path1: target, path2: assemblyName);

        EmitResult compilationResult = this.Compile(assemblyName: assemblyName, assemblyFileName: assemblyFileName, resources: resources);
        Assert.True(condition: compilationResult.Success, userMessage: "Compilation failed");

        Assembly ass = await LoadAssemblyFromFileAsync(new(assemblyFileName), fileName: assemblyFileName, this.CancellationToken());

        await this.VerifyResourcesAsync(ass: ass, MakePartiallyQualifiedResourceName(ns: ns, name: CONTENT_RESOURCE_NAME), filesToPack: filesToPack);

        this.VerifyAdditionalResource(ass: ass, MakePartiallyQualifiedResourceName(ns: ns, name: ADDITIONAL_RESOURCE_NAME));
    }

    [SuppressMessage(category: "Meziantou.Analyzer", checkId: "MA0045: Use async", Justification = "Not here")]
    private static Stream GenerateResourcesFromDictionary(Dictionary<string, string> additionalData)
    {
        MemoryStream rws = new();

        using (MemoryStream rws2 = new())
        {
            using (ResourceWriter writer = new(rws2))
            {
                foreach ((string key, string value) in additionalData)
                {
                    writer.AddResource(name: key, value: value);
                }

                writer.AddResource(name: "$KEYS", string.Join(separator: ',', values: additionalData.Keys));

                writer.Generate();
                rws2.Seek(offset: 0, loc: SeekOrigin.Begin);
                rws2.CopyTo(rws);
                rws.Seek(offset: 0, loc: SeekOrigin.Begin);

                return rws;
            }
        }
    }

    private async Task GenerateResourcesFromFilesAsync(string filename, IReadOnlyList<string> filesToPack)
    {
        using (ResourceWriter writer = new(filename))
        {
            foreach (string file in filesToPack)
            {
                string name = MakeEntryHash(file);

                byte[] bytes = await File.ReadAllBytesAsync(path: file, this.CancellationToken());

                Stream stream = new MemoryStream(buffer: bytes, writable: false);

                writer.AddResource(name: name, value: stream, closeAfterWrite: true);
            }

            writer.Generate();
        }
    }

    private void VerifyAdditionalResource(Assembly ass, string res)
    {
        ResourceManager rm = new(baseName: res, assembly: ass);

        string[] keys = rm.GetString(name: "$KEYS", culture: CultureInfo.InvariantCulture)
                          ?.Split(',') ?? [];

        foreach (string key in keys)
        {
            this.Output.WriteLine($"{key} = {rm.GetString(name: key, culture: CultureInfo.InvariantCulture)}");
        }
    }

    private static string AddAssemblyMetadata(Settings settings, string publicKey)
    {
        const string metadataKey = "DeveloperPublicKey";

        return new StringBuilder().AppendLine("using System;")
                                  .AppendLine("using System.Reflection;")
                                  .AppendLine()
                                  .AppendLine($"[assembly: {nameof(AssemblyTitleAttribute)}({EncodeString(settings.ProductName)})]")
                                  .AppendLine($"[assembly: {nameof(AssemblyProductAttribute)}({EncodeString(settings.Summary)})]")
                                  .AppendLine($"[assembly: {nameof(AssemblyDescriptionAttribute)}({EncodeString(settings.Description)})]")
                                  .AppendLine($"[assembly: {nameof(AssemblyVersionAttribute)}({EncodeString(settings.Version.DottedVersion.ToString())})]")
                                  .AppendLine($"[assembly: {nameof(AssemblyFileVersionAttribute)}({EncodeString(settings.Version.DottedVersion.ToString())})]")
                                  .AppendLine($"[assembly: {nameof(AssemblyInformationalVersionAttribute)}({EncodeString(settings.Version.Version)})]")
                                  .AppendLine($"[assembly: {nameof(AssemblyCompanyAttribute)}({EncodeString(settings.Author)})]")
                                  .AppendLine($"[assembly: {nameof(AssemblyMetadataAttribute)}(key:{EncodeString(metadataKey)}, value:{EncodeString(publicKey)})]")
                                  .ToString();
    }

    private static string EncodeString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return $"string.{nameof(string.Empty)}";
        }

        return "\"" + Escape(value) + "\"";
    }

    private static string Escape(string value)
    {
        return value.Replace(oldValue: "\"", newValue: "\\\"", comparisonType: StringComparison.Ordinal);
    }

    private EmitResult Compile(string assemblyName, string assemblyFileName, ResourceDescription[] resources)
    {
        Settings settings = new(Author: "Test",
                                new("1.0.0.1-test"),
                                ContentType: ComponentType.LOBBY,
                                ProductName: assemblyName,
                                Copyright: "Me",
                                Description: "Description",
                                Summary: "Summary",
                                LicenseUrl: null,
                                ProjectUrl: null,
                                IconUrl: null,
                                ReleaseNotes: "RN",
                                ["Tag1"],
                                Id: assemblyName);

        string source = AddAssemblyMetadata(settings: settings, publicKey: "0x123457");

        SyntaxTree tree = SyntaxFactory.ParseSyntaxTree(text: source,
                                                        new CSharpParseOptions(languageVersion: LanguageVersion.Latest, documentationMode: DocumentationMode.None, kind: SourceCodeKind.Regular),
                                                        cancellationToken: this.CancellationToken());

        CSharpCompilationOptions options = GenerateOptions();
        IReadOnlyList<MetadataReference> references = BuildReferences();
        CSharpCompilation compilation = CSharpCompilation.Create(assemblyName: assemblyName, references: references, options: options)
                                                         .AddSyntaxTrees(tree);

        EmitResult compilationResult = compilation.Emit(outputPath: assemblyFileName, manifestResources: resources, cancellationToken: this.CancellationToken());

        return compilationResult;
    }

    private static IReadOnlyList<MetadataReference> BuildReferences()
    {
        string[] references =
        [
            Ref<string>()
        ];

        return
        [
            .. references.Distinct(StringComparer.Ordinal)
                         .Order(StringComparer.OrdinalIgnoreCase)
                         .Select(selector: fileName => MetadataReference.CreateFromFile(fileName))
        ];
    }

    [SuppressMessage(category: "FunFair.CodeAnalysis", checkId: "FFS0008:Don't disable warnings with #pragma", Justification = "Needed in this case")]
    private static string Ref<T>()
    {
#pragma warning disable IL3000
        return typeof(T).GetTypeInfo()
                        .Assembly.Location;
#pragma warning restore IL3000
    }

    private static string MakeFullyQualifiedResourceName(string ns, string name)
    {
        return ns + name + ".resources";
    }

    private static string MakePartiallyQualifiedResourceName(string ns, string name)
    {
        return ns + name;
    }

    private async Task VerifyResourcesAsync(Assembly ass, string res, IReadOnlyList<string> filesToPack)
    {
        ResourceManager rm = new(baseName: res, assembly: ass);

        foreach (string file in filesToPack)
        {
            string name = MakeEntryHash(file);
            UnmanagedMemoryStream? s = rm.GetStream(name: name, culture: CultureInfo.InvariantCulture);

            if (s is not null)
            {
                byte[] sourceBytes = await File.ReadAllBytesAsync(path: file, this.CancellationToken());

                await using (MemoryStream ms = new())
                {
                    await s.CopyToAsync(destination: ms, this.CancellationToken());
                    byte[] packedBytes = ms.ToArray();

                    Assert.True(sourceBytes.SequenceEqual(packedBytes), $"{file}: Content is different");
                }
            }
            else
            {
                Assert.Fail($"{file}: Content is missing");
            }
        }
    }

    private static CSharpCompilationOptions GenerateOptions()
    {
        return new(outputKind: OutputKind.DynamicallyLinkedLibrary,
                   optimizationLevel: OptimizationLevel.Release,
                   checkOverflow: true,
                   allowUnsafe: false,
                   platform: Platform.AnyCpu,
                   generalDiagnosticOption: ReportDiagnostic.Suppress,
                   warningLevel: 4,
                   specificDiagnosticOptions:
                   [
                       new(key: "CS8021", value: ReportDiagnostic.Suppress)
                   ],
                   deterministic: true,
                   nullableContextOptions: NullableContextOptions.Enable);
    }

    private static string MakeEntryHash(string mapping)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(mapping));

        return Base64Url.EncodeToString(hash);
    }

    private static async ValueTask<Assembly> LoadAssemblyFromFileAsync(AssemblyLoadContext loadContext, string fileName, CancellationToken cancellationToken)
    {
        await using (MemoryStream ms = MemoryStreamManager.GetStream())
        {
            await using (FileStream fs = OpenFile(fileName))
            {
                await fs.CopyToAsync(destination: ms, cancellationToken: cancellationToken);
                ms.Seek(offset: 0, loc: SeekOrigin.Begin);

                return loadContext.LoadFromStream(ms);
            }
        }
    }

    private static FileStream OpenFile(string fileName)
    {
        return new(path: fileName,
                   mode: FileMode.Open,
                   access: FileAccess.Read,
                   share: FileShare.Read,
                   bufferSize: 1, // bufferSize == 1 used to avoid unnecessary buffer in FileStream
                   FileOptions.Asynchronous | FileOptions.SequentialScan);
    }

    [DebuggerDisplay(value: "Version: {Version} Pre-Release: {IsPreRelease}")]
    private readonly record struct PackageVersion : IComparable<PackageVersion>, IComparable
    {
        [SuppressMessage(category: "Microsoft.IDE", checkId: "IDE0057: Simplify substring", Justification = "For compatibility with netstandard")]
        public PackageVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentNullException(nameof(version));
            }

            int pos = GetPreReleaseIndex(version);

            if (pos != -1)
            {
                string dotted = version.Substring(startIndex: 0, length: pos);

                if (!System.Version.TryParse(input: dotted, out Version? dv))
                {
                    throw new ArgumentOutOfRangeException(nameof(version), actualValue: version, message: "Version is not a version");
                }

                this.DottedVersion = dv;
                this.PreReleaseTag = version.Substring(pos + 1);
            }
            else
            {
                if (!System.Version.TryParse(input: version, out Version? dv))
                {
                    throw new ArgumentOutOfRangeException(nameof(version), actualValue: version, message: "Version is not a version");
                }

                this.DottedVersion = dv;
                this.PreReleaseTag = string.Empty;
            }

            this.Version = version;
        }

        public string Version { get; }

        public Version DottedVersion { get; }

        public string PreReleaseTag { get; }

        public bool IsPreRelease => !string.IsNullOrWhiteSpace(this.PreReleaseTag);

        public int CompareTo(object? obj)
        {
            if (obj is null)
            {
                return 1;
            }

            return obj is PackageVersion typed
                ? CompareToCommon(this, right: typed)
                : NotAPackageVersion(obj);
        }

        public int CompareTo(PackageVersion other)
        {
            return CompareToCommon(this, right: other);
        }

        [SuppressMessage(category: "Meziantou.Analyzer", checkId: "MA0089:Use overload with char instead of string", Justification = "For compatibility with netstandard")]
        [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1865:Use overload with char instead of string", Justification = "For compatibility with netstandard")]
        private static int GetPreReleaseIndex(string version)
        {
            return version.IndexOf(value: "-", comparisonType: StringComparison.Ordinal);
        }

        public override string ToString()
        {
            return this.Version;
        }

        [SuppressMessage(category: "SonarAnalyzer.CSharp", checkId: "S1172: Parameter unused", Justification = "Used for name")]
        [SuppressMessage(category: "ReSharper", checkId: "EntityNameCapturedOnly.Local", Justification = "Used for name")]
        private static int NotAPackageVersion(object? obj)
        {
            throw new ArgumentException(message: "Must be of type PackageVersion", nameof(obj));
        }

        public static bool operator <(in PackageVersion left, in PackageVersion right)
        {
            return CompareToCommon(left: left, right: right) < 0;
        }

        public static bool operator >(in PackageVersion left, in PackageVersion right)
        {
            return CompareToCommon(left: left, right: right) > 0;
        }

        public static bool operator <=(in PackageVersion left, in PackageVersion right)
        {
            return CompareToCommon(left: left, right: right) <= 0;
        }

        public static bool operator >=(in PackageVersion left, in PackageVersion right)
        {
            return CompareToCommon(left: left, right: right) >= 0;
        }

        private static int CompareToCommon(in PackageVersion left, in PackageVersion right)
        {
            int cmp = left.DottedVersion.CompareTo(right.DottedVersion);

            if (cmp != 0)
            {
                return cmp;
            }

            if (left.IsPreRelease && right.IsPreRelease)
            {
                return StringComparer.OrdinalIgnoreCase.Compare(x: left.PreReleaseTag, y: right.PreReleaseTag);
            }

            if (left.IsPreRelease && !right.IsPreRelease)
            {
                return -1;
            }

            if (!left.IsPreRelease && right.IsPreRelease)
            {
                return 1;
            }

            return 0;
        }
    }

    [SuppressMessage(category: "Meziantou.Analyzer", checkId: "MA0109:Consider adding a Span<T> overload", Justification = "Not needed here")]
    [DebuggerDisplay("{Id}\\{Version} :{ProductName}")]
    private sealed record Settings(
        string Author,
        PackageVersion Version,
        ComponentType ContentType,
        string ProductName,
        string Copyright,
        string Description,
        string Summary,
        Uri? LicenseUrl,
        Uri? ProjectUrl,
        Uri? IconUrl,
        string ReleaseNotes,
        IReadOnlyList<string> Tags,
        string Id);
}