using System;
using System.Buffers.Text;
using System.Collections.Generic;
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
using FunFair.Test.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.IO;
using Xunit;

namespace Experiments;

public sealed class ResourceDllTests : LoggingFolderCleanupTestBase
{
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

        const string resourceBaseName = "example";

        string filename = Path.Combine(path1: target, resourceBaseName + ".resources");

        IReadOnlyList<string> filesToPack = await FileHelper.CreateTempFilesAsync(basePath: source, filenames: ExampleTempFiles, this.CancellationToken());

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

        const string ns = "Credfeto.Resources.Example";
        ResourceDescription[] resources =
        [
            new(MakeFullyQualifiedResourceName(ns: ns, name: resourceBaseName), dataProvider: () => File.OpenRead(filename), isPublic: true)
        ];

        CSharpCompilationOptions options = GenerateOptions();

        const string assemblyName = ns + ".dll";
        string assemblyFileName = Path.Combine(path1: target, path2: assemblyName);
        CSharpCompilation compilation = CSharpCompilation.Create(assemblyName: assemblyName, options: options);

        EmitResult compilationResult = compilation.Emit(outputPath: assemblyFileName, manifestResources: resources, cancellationToken: this.CancellationToken());
        Assert.True(condition: compilationResult.Success, userMessage: "Compilation failed");

        await this.VerifyResourcesAsync(assemblyFileName: assemblyFileName, MakePartiallyQualifiedResourceName(ns: ns, name: resourceBaseName), filesToPack: filesToPack);
    }

    private static string MakeFullyQualifiedResourceName(string ns, string name)
    {
        return ns + name + ".resources";
    }

    private static string MakePartiallyQualifiedResourceName(string ns, string name)
    {
        return ns + name;
    }

    private async Task VerifyResourcesAsync(string assemblyFileName, string res, IReadOnlyList<string> filesToPack)
    {
        Assembly ass = await LoadAssemblyFromFileAsync(new(assemblyFileName), fileName: assemblyFileName, this.CancellationToken());

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
}