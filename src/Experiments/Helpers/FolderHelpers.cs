using System.IO;

namespace Experiments.Helpers;

internal static class FolderHelpers
{
    public static void EnsureFolderExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}
