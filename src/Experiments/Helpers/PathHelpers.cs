using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Experiments.Helpers;

internal static class PathHelpers
{
    public static string GetRelativePath(string documentFullPath, string referencedFileFullPath)
    {
        IReadOnlyList<string> documentPathParts = documentFullPath.Split(Path.DirectorySeparatorChar);
        IReadOnlyList<string> referencedFilePathParts = referencedFileFullPath.Split(Path.DirectorySeparatorChar);

        int commonLength = FindCommonPartLength(
            documentPathParts: documentPathParts,
            referencedFilePathParts: referencedFilePathParts
        );

        if (commonLength == referencedFilePathParts.Count && documentPathParts.Count == commonLength)
        {
            return documentPathParts[commonLength - 1];
        }

        int upDir = documentPathParts.Count - commonLength - 1;

        return string.Join(
            Path.DirectorySeparatorChar.ToString(),
            Enumerable.Repeat(element: "..", count: upDir).Concat(referencedFilePathParts.Skip(commonLength))
        );
    }

    private static int FindCommonPartLength(
        IReadOnlyList<string> documentPathParts,
        IReadOnlyList<string> referencedFilePathParts
    )
    {
        if (documentPathParts.Count < referencedFilePathParts.Count)
        {
            return GetCommon(path1: referencedFilePathParts, path2: documentPathParts);
        }

        return GetCommon(path1: documentPathParts, path2: referencedFilePathParts);
    }

    private static int GetCommon(IReadOnlyList<string> path1, IReadOnlyList<string> path2)
    {
        int count = 0;

        for (int i = 0; i < path1.Count; ++i)
        {
            if (!StringComparer.Ordinal.Equals(path1[i], path2[i]))
            {
                break;
            }

            ++count;
        }

        return count;
    }
}
