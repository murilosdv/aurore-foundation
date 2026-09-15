using System;
using System.Linq;
using Aurore.Foundation.CliCore.Extensions;
using Aurore.Foundation.Tests.CliCore.Fixtures;
using Spectre.Console.Cli;

namespace Aurore.Foundation.Tests.CliCore.Extensions;

public class CommandExtensionsTests
{
    [Fact(DisplayName = "GetCommandMetadata returns the declared name and description")]
    public void GetCommandMetadataReturnsDeclaredValues()
    {
        // Act
        var meta = typeof(GreetCommand).GetCommandMetadata();

        // Assert
        Assert.Equal("greet", meta.Name);
        Assert.Equal("Greets someone by name", meta.Description);
    }

    [Fact(DisplayName = "GetCommandMetadata throws when the type has no CommandInfo attribute")]
    public void GetCommandMetadataThrowsWhenMissing()
    {
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => typeof(UndecoratedCommand).GetCommandMetadata());
    }

    [Fact(DisplayName = "GetCommandExamples returns every declared CommandExample attribute's args")]
    public void GetCommandExamplesReturnsDeclaredExamples()
    {
        // Act
        var examples = typeof(GreetCommand).GetCommandExamples().ToList();

        // Assert
        var example = Assert.Single(examples);
        Assert.Equal(["greet", "--name", "Aurore"], example.Args);
    }

    [Fact(DisplayName = "GetCommandExamples returns empty when no example attribute is declared")]
    public void GetCommandExamplesReturnsEmptyWhenNoneDeclared()
    {
        // Act & Assert
        Assert.Empty(typeof(UndecoratedCommand).GetCommandExamples());
    }

    [Fact(DisplayName = "GetCommandAliases returns every declared alias")]
    public void GetCommandAliasesReturnsDeclaredAliases()
    {
        // Act
        var aliases = typeof(GreetCommand).GetCommandAliases().ToList();

        // Assert
        Assert.Equal(["hi", "hey"], aliases);
    }

    [Fact(DisplayName = "GetCommandAliases returns empty when no alias attribute is declared")]
    public void GetCommandAliasesReturnsEmptyWhenNoneDeclared()
    {
        // Act & Assert
        Assert.Empty(typeof(UndecoratedCommand).GetCommandAliases());
    }

    [Fact(DisplayName = "ConfigureCommand registers the command under its primary name")]
    public void ConfigureCommandRegistersPrimaryName()
    {
        // Arrange
        var app = new CommandApp();
        app.Configure(config => config.ConfigureCommand<GreetCommand>());

        // Act
        var exitCode = app.Run(["greet", "--name", "Aurore"], TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(0, exitCode);
    }

    [Fact(DisplayName = "ConfigureCommand also registers every declared alias")]
    public void ConfigureCommandRegistersAliases()
    {
        // Arrange
        var app = new CommandApp();
        app.Configure(config => config.ConfigureCommand<GreetCommand>());

        // Act
        var exitCode = app.Run(["hi", "--name", "Aurore"], TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(0, exitCode);
    }

    [Fact(DisplayName = "ConfigureBranch registers the branch and its nested commands under its primary name")]
    public void ConfigureBranchRegistersPrimaryName()
    {
        // Arrange
        var app = new CommandApp();
        app.Configure(config => config.ConfigureBranch<ColorsBranch>());

        // Act
        var exitCode = app.Run(["colors", "list"], TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(0, exitCode);
    }

    [Fact(DisplayName = "ConfigureBranch also registers every declared branch alias")]
    public void ConfigureBranchRegistersAliases()
    {
        // Arrange
        var app = new CommandApp();
        app.Configure(config => config.ConfigureBranch<ColorsBranch>());

        // Act
        var exitCode = app.Run(["clr", "list"], TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(0, exitCode);
    }

    [Fact(DisplayName = "UseDefaultExceptionHandler intercepts an exception thrown by a command and returns the configured exit code")]
    public void UseDefaultExceptionHandlerReturnsConfiguredExitCode()
    {
        // Arrange
        var app = new CommandApp();
        app.Configure(config =>
        {
            config.UseDefaultExceptionHandler(exitCode: 42);

            config.ConfigureCommand<ThrowingCommand>();
        });

        // Act
        var exitCode = app.Run(["throw"], TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, exitCode);
    }

    [Fact(DisplayName = "UseDefaultExceptionHandler doesn't interfere with a command that completes normally")]
    public void UseDefaultExceptionHandlerDoesNotAffectSuccessfulCommands()
    {
        // Arrange
        var app = new CommandApp();
        app.Configure(config =>
        {
            config.UseDefaultExceptionHandler();

            config.ConfigureCommand<GreetCommand>();
        });

        // Act
        var exitCode = app.Run(["greet", "--name", "Aurore"], TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(0, exitCode);
    }
}
