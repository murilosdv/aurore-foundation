using Aurore.Foundation.CLI.Core.Extensions;
using Aurore.Foundation.Tests.CliCore.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Aurore.Foundation.Tests.CliCore.Extensions;

public class CommandAppExtensionsTests
{
    [Fact(DisplayName = "CreateWithServices resolves constructor-injected dependencies when running a command")]
    public void CreateWithServicesResolvesDependencies()
    {
        // Arrange
        var greeted = false;

        var app = CommandApp.CreateWithServices(services =>
            services.AddSingleton<IGreeter>(new Greeter(() => greeted = true)));

        app.Configure(config => config.ConfigureCommand<InjectedGreetCommand>());

        // Act
        var exitCode = app.Run(["greet-injected"], TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(0, exitCode);
        Assert.True(greeted);
    }
}
