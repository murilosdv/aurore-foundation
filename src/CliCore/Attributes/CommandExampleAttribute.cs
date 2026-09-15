using System;

namespace Aurore.Foundation.CLI.Core.Attributes;

/// <summary>
/// Declares a usage example shown in the command's help text. Apply more than once to declare
/// multiple examples.
/// </summary>
/// <param name="args">The example's full argument list, exactly as a user would type it.</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class CommandExampleAttribute(params string[] args) : Attribute
{
    /// <summary>
    /// The example's full argument list, exactly as a user would type it.
    /// </summary>
    public string[] Args { get; } = args;
}
