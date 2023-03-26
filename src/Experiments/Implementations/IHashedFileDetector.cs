namespace Experiments.Implementations;

/// <summary>
///     Detects whether files have a hash in their filename so they can be treated specially when caching.
/// </summary>
public interface IHashedFileDetector
{
    /// <summary>
    ///     Checks to see if the given filename contains a hash.
    /// </summary>
    /// <param name="fileName">The filename.</param>
    /// <returns>True, if the filename contains a hash; otherwise, false.</returns>
    bool IsHashedFileName(string fileName);
}