using System;

namespace Aurore.Foundation.CliCore.Attributes;

/// <summary>
/// Declares the name and description a command or branch is registered under. Required on every
/// type registered via <c>ConfigureCommand</c> or <c>ConfigureBranch</c>.
/// </summary>
/// <param name="name">The command or branch name, as typed on the command line.</param>
/// <param name="description">The description shown in the command's or branch's help text.</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class CommandInfoAttribute(string name, string description) : Attribute
{
    /// <summary>
    /// The command or branch name, as typed on the command line.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// The description shown in the command's or branch's help text.
    /// </summary>
    public string Description { get; } = description;
}
