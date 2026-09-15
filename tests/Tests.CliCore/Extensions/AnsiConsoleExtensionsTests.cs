using System;
using Aurore.Foundation.CliCore.Extensions;
using Spectre.Console;

namespace Aurore.Foundation.Tests.CliCore.Extensions;

public class AnsiConsoleExtensionsTests
{
    [Theory(DisplayName = "WriteInfo/Success/Warning/Error don't throw for messages containing markup-special characters")]
    [InlineData("path/[DEBUG]/output.json")]
    [InlineData("unbalanced ] bracket")]
    [InlineData("[[already escaped]]")]
    public void SeverityMessagesDoNotThrowForBracketContainingText(string message)
    {
        // Act
        var exception = Record.Exception(() =>
        {
            AnsiConsole.WriteInfo(message);
            AnsiConsole.WriteSuccess(message);
            AnsiConsole.WriteWarning(message);
            AnsiConsole.WriteError(message);
        });

        // Assert
        Assert.Null(exception);
    }

    [Fact(DisplayName = "WriteError(Exception) doesn't throw when the exception message contains markup-special characters")]
    public void WriteErrorDoesNotThrowForExceptionWithBracketsInMessage()
    {
        // Arrange
        var ex = new InvalidOperationException("Command must be decorated with [CommandInfo].");

        // Act
        var exception = Record.Exception(() => AnsiConsole.WriteError(ex));

        // Assert
        Assert.Null(exception);
    }

    [Fact(DisplayName = "WriteError(label, Exception) doesn't throw when the exception message contains markup-special characters")]
    public void WriteErrorWithLabelDoesNotThrowForExceptionWithBracketsInMessage()
    {
        // Arrange
        var ex = new InvalidOperationException("Command must be decorated with [CommandInfo].");

        // Act
        var exception = Record.Exception(() => AnsiConsole.WriteError("Startup", ex));

        // Assert
        Assert.Null(exception);
    }

    [Theory(DisplayName = "Labeled writers don't throw when the label or value contains markup-special characters")]
    [InlineData("La[bel", "Val]ue")]
    [InlineData("[[Label]]", "[[Value]]")]
    public void LabeledWritersDoNotThrowForBracketContainingText(string label, string value)
    {
        // Act
        var exception = Record.Exception(() =>
        {
            AnsiConsole.WriteInfo(label, value);
            AnsiConsole.WriteSuccess(label, value);
            AnsiConsole.WriteWarning(label, value);
            AnsiConsole.WriteError(label, value);
            AnsiConsole.WriteEmphasized(label, value);
        });

        // Assert
        Assert.Null(exception);
    }
}
