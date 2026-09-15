using System.Threading;
using Aurore.Foundation.CLI.Core.Abstractions;
using Aurore.Foundation.CLI.Core.Attributes;
using Aurore.Foundation.CLI.Core.Extensions;
using Spectre.Console.Cli;

namespace Aurore.Foundation.Tests.CliCore.Fixtures;

[CommandInfo("colors", "Groups color-related commands")]
[CommandAlias("clr")]
public sealed class ColorsBranch : ICommandBranch
{
    public static void Configure(IConfigurator<CommandSettings> branch)
    {
        branch.ConfigureCommand<ListColorsCommand>();
    }
}

[CommandInfo("list", "Lists known colors")]
public sealed class ListColorsCommand : Command<EmptyCommandSettings>
{
    protected override int Execute(CommandContext context, EmptyCommandSettings settings, CancellationToken cancellationToken)
    {
        return 0;
    }
}
