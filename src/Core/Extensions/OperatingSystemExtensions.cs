using System;

namespace Aurore.Foundation.Core.Extensions;

/// <summary>
/// Provides extension members for <see cref="OperatingSystem"/> to detect the runtime environment.
/// </summary>
public static class OperatingSystemExtensions
{
    extension(OperatingSystem)
    {
        /// <summary>
        /// Determines whether the current process is running under Windows Subsystem for Linux (WSL).
        /// </summary>
        /// <returns><see langword="true"/> if running on Linux with the <c>WSL_DISTRO_NAME</c> environment variable set; otherwise, <see langword="false"/>.</returns>
        public static bool IsWsl()
        {
            return
                OperatingSystem.IsLinux() &&
                Environment.GetEnvironmentVariable("WSL_DISTRO_NAME") is not null;
        }

        /// <summary>
        /// Determines whether the current process is running on a native (non-WSL) Linux environment.
        /// </summary>
        /// <returns><see langword="true"/> if running on Linux and not under WSL; otherwise, <see langword="false"/>.</returns>
        public static bool IsNativeLinux()
        {
            return OperatingSystem.IsLinux() && IsWsl() is false;
        }
    }
}
