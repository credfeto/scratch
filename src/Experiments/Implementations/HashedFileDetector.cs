using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Experiments.Implementations;

internal sealed class HashedFileDetector : IHashedFileDetector
{
    private static readonly IReadOnlyList<string> NonHexSuffix = ["bundle", "chunked", "chunk"];

    public bool IsHashedFileName(string fileName)
    {
        for (
            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            filenameWithoutExtension.Contains(value: '.', comparisonType: StringComparison.Ordinal);
            filenameWithoutExtension = Path.GetFileNameWithoutExtension(filenameWithoutExtension)
        )
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
        return NonHexSuffix.Any(suffix => StringComparer.OrdinalIgnoreCase.Equals(x: hash, y: suffix));
    }

    private static bool IsHexDigit(char x)
    {
        return x is >= '0' and <= '9' or >= 'a' and <= 'f' or >= 'A' and <= 'F';
    }

    private static string ExtractHashFromFileName(string filenameWithoutExtension)
    {
        return Path.GetExtension(filenameWithoutExtension).TrimStart(trimChar: '.');
    }
}
