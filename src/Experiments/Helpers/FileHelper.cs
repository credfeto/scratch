using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Experiments.Helpers;

internal static class FileHelper
{
    public static async Task<IReadOnlyList<string>> CreateTempFilesAsync(
        string basePath,
        string[] filenames,
        CancellationToken cancellationToken
    )
    {
        FolderHelpers.EnsureFolderExists(basePath);

        return await Task.WhenAll(
            filenames.Select(file =>
                                 WriteBytesAsync(basePath: basePath, file: file, cancellationToken: cancellationToken)
            )
        );
    }

    private static async Task<string> WriteBytesAsync(string basePath, string file, CancellationToken cancellationToken)
    {
        string fullFileName = Path.Combine(path1: basePath, path2: file);

        await File.WriteAllBytesAsync(
            path: fullFileName,
            Encoding.UTF8.GetBytes(file),
            cancellationToken: cancellationToken
        );

        return fullFileName;
    }
}