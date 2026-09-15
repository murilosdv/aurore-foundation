using System.Threading;
using Aurore.Foundation.CLI.Core.Attributes;
using Spectre.Console.Cli;

namespace Aurore.Foundation.Tests.CliCore.Fixtures;

[CommandInfo("greet", "Greets someone by name")]
[CommandExample("greet", "--name", "Aurore")]
[CommandAlias("hi")]
[CommandAlias("hey")]
public sealed class GreetCommand : Command<GreetCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandOption("--name")]
        public string? Name { get; set; }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        return 0;
    }
}

public sealed class UndecoratedCommand : Command<EmptyCommandSettings>
{
    protected override int Execute(CommandContext context, EmptyCommandSettings settings, CancellationToken cancellationToken)
    {
        return 0;
    }
}

[CommandInfo("greet-injected", "Greets using an injected greeter")]
public sealed class InjectedGreetCommand(IGreeter greeter) : Command<EmptyCommandSettings>
{
    protected override int Execute(CommandContext context, EmptyCommandSettings settings, CancellationToken cancellationToken)
    {
        greeter.Greet();

        return 0;
    }
}
