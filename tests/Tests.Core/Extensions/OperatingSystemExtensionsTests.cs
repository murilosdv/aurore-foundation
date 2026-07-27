using System;
using Aurore.Foundation.Core.Extensions;

namespace Aurore.Foundation.Tests.Core.Extensions;

public class OperatingSystemExtensionsTests
{
    [Fact(DisplayName = "IsWsl returns true only when running on Linux with WSL_DISTRO_NAME set")]
    public void IsWslReflectsPlatformWhenEnvironmentVariableIsSet()
    {
        // Arrange
        var original = Environment.GetEnvironmentVariable("WSL_DISTRO_NAME");
        Environment.SetEnvironmentVariable("WSL_DISTRO_NAME", "Ubuntu");

        try
        {
            // Act
            var result = OperatingSystem.IsWsl();

            // Assert
            Assert.Equal(OperatingSystem.IsLinux(), result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("WSL_DISTRO_NAME", original);
        }
    }

    [Fact(DisplayName = "IsWsl returns false when WSL_DISTRO_NAME is not set")]
    public void IsWslReturnsFalseWithoutEnvironmentVariable()
    {
        // Arrange
        var original = Environment.GetEnvironmentVariable("WSL_DISTRO_NAME");
        Environment.SetEnvironmentVariable("WSL_DISTRO_NAME", null);

        try
        {
            // Act
            var result = OperatingSystem.IsWsl();

            // Assert
            Assert.False(result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("WSL_DISTRO_NAME", original);
        }
    }

    [Fact(DisplayName = "IsNativeLinux returns true only on Linux without WSL_DISTRO_NAME set")]
    public void IsNativeLinuxReflectsPlatformWithoutWslVariable()
    {
        // Arrange
        var original = Environment.GetEnvironmentVariable("WSL_DISTRO_NAME");
        Environment.SetEnvironmentVariable("WSL_DISTRO_NAME", null);

        try
        {
            // Act
            var result = OperatingSystem.IsNativeLinux();

            // Assert
            Assert.Equal(OperatingSystem.IsLinux(), result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("WSL_DISTRO_NAME", original);
        }
    }

    [Fact(DisplayName = "IsNativeLinux returns false when WSL_DISTRO_NAME is set")]
    public void IsNativeLinuxReturnsFalseWhenWslVariableIsSet()
    {
        // Arrange
        var original = Environment.GetEnvironmentVariable("WSL_DISTRO_NAME");
        Environment.SetEnvironmentVariable("WSL_DISTRO_NAME", "Ubuntu");

        try
        {
            // Act
            var result = OperatingSystem.IsNativeLinux();

            // Assert
            Assert.False(result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("WSL_DISTRO_NAME", original);
        }
    }
}
