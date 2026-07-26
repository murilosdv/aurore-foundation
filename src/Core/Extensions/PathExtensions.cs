using System;
using System.IO;
using System.Linq;

namespace Aurore.Foundation.Core.Extensions;

/// <summary>
/// Provides extension members for <see cref="Path"/> to normalize paths across Windows and WSL runtimes.
/// </summary>
public static class PathExtensions
{
    private const string WslPrefix = "/mnt/";

    extension(Path)
    {
        /// <summary>
        /// Combines and normalizes the given path segments into a full path appropriate for the current runtime,
        /// converting between Windows-style (<c>D:\...</c>) and WSL-style (<c>/mnt/d/...</c>) forms as needed.
        /// </summary>
        /// <param name="paths">The path segments to combine, each of which may be in Windows or WSL form.</param>
        /// <returns>
        /// The combined, full path in whichever form the current runtime natively understands: Windows-style on
        /// Windows, WSL-style on WSL, or unconverted on any other runtime.
        /// </returns>
        /// <remarks>
        /// Each segment is converted to the current runtime's native form before being handed to
        /// <see cref="Path.Combine(string[])"/>/<see cref="Path.GetFullPath(string)"/> — those APIs only resolve
        /// paths correctly when given a root in the format the running OS actually recognizes as absolute.
        /// </remarks>
        public static string NormalizeRuntimePath(params string[] paths)
        {
            var normalized = paths
                .Select(NormalizeSegment)
                .ToArray();

            return Path.GetFullPath(Path.Combine(normalized));
        }

        private static string NormalizeSegment(string path)
        {
            if (OperatingSystem.IsWindows())
                return FromWslPath(path);

            return OperatingSystem.IsWsl()
                ? ToWslPath(path)
                : path;
        }

        private static string FromWslPath(string path)
        {
            if (!path.StartsWith(WslPrefix, StringComparison.OrdinalIgnoreCase))
                return path;

            // /mnt/d/git/repo -> D:\git\repo
            var drive = char.ToUpperInvariant(path[5]);

            var remainder = path[6..]
                .Replace('/', '\\');

            return $"{drive}:\\{remainder}";
        }

        private static string ToWslPath(string path)
        {
            if (path.Length < 2 || path[1] != ':' || !char.IsAsciiLetter(path[0]))
                return path;

            // D:\git\repo -> /mnt/d/git/repo
            var drive = char.ToLowerInvariant(path[0]);

            var remainder = path[2..]
                .Replace('\\', '/')
                .TrimStart('/');

            return $"/mnt/{drive}/{remainder}";
        }
    }
}
