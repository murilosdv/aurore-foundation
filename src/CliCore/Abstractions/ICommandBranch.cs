using Spectre.Console.Cli;

namespace Aurore.Foundation.CliCore.Abstractions;

/// <summary>
/// Marks a type as a command branch that groups related commands under a shared name, to be
/// registered via <c>ConfigureBranch</c>.
/// </summary>
public interface ICommandBranch
{
    /// <summary>
    /// Registers the branch's nested commands and/or sub-branches.
    /// </summary>
    /// <param name="branch">The configurator scoped to this branch.</param>
    static abstract void Configure(IConfigurator<CommandSettings> branch);
}
