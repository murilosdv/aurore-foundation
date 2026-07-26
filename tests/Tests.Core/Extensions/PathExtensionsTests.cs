using System;
using System.IO;
using Aurore.Foundation.Core.Extensions;

namespace Tests.Core.Extensions;

public class PathExtensionsTests
{
    [Fact(DisplayName = "NormalizeRuntimePath converts a WSL-style path into its Windows equivalent, or leaves it unchanged off Windows")]
    public void NormalizeRuntimePathConvertsWslPathToWindows()
    {
        // Arrange
        var wslPath = "/mnt/d/git/repo";
        var expected = OperatingSystem.IsWindows() ? @"D:\git\repo" : wslPath;

        // Act
        var result = Path.NormalizeRuntimePath(wslPath);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact(DisplayName = "NormalizeRuntimePath combines multiple segments into a single full path")]
    public void NormalizeRuntimePathCombinesSegments()
    {
        // Arrange
        var expected = Path.GetFullPath(Path.Combine(@"C:\base", "sub", "file.txt"));

        // Act
        var result = Path.NormalizeRuntimePath(@"C:\base", "sub", "file.txt");

        // Assert
        Assert.Equal(expected, result);
    }
}
