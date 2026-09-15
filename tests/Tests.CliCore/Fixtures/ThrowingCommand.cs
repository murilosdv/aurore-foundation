using System;
using System.Threading;
using Aurore.Foundation.CLI.Core.Attributes;
using Spectre.Console.Cli;

namespace Aurore.Foundation.Tests.CliCore.Fixtures;

[CommandInfo("throw", "Always throws, for testing error handling")]
public sealed class ThrowingCommand : Command<EmptyCommandSettings>
{
    protected override int Execute(CommandContext context, EmptyCommandSettings settings, CancellationToken cancellationToken)
    {
        throw new InvalidOperationException("Simulated command failure.");
    }
}
