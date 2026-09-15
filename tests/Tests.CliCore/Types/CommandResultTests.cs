using Aurore.Foundation.CLI.Core.Types;

namespace Aurore.Foundation.Tests.CliCore.Types;

public class CommandResultTests
{
    [Fact(DisplayName = "IsSuccess is true when the exit code is zero")]
    public void IsSuccessTrueForZeroExitCode()
    {
        // Arrange
        var result = new CommandResult("output", string.Empty, 0);

        // Act & Assert
        Assert.True(result.IsSuccess);
    }

    [Theory(DisplayName = "IsSuccess is false for any non-zero exit code")]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(127)]
    public void IsSuccessFalseForNonZeroExitCode(int exitCode)
    {
        // Arrange
        var result = new CommandResult(string.Empty, "error", exitCode);

        // Act & Assert
        Assert.False(result.IsSuccess);
    }
}
