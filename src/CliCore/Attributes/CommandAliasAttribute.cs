using System;

namespace Aurore.Foundation.CLI.Core.Attributes;

/// <summary>
/// Declares an alternate name a command or branch can also be invoked by. Apply more than once
/// to declare multiple aliases.
/// </summary>
/// <param name="alias">The alias to register alongside the command's or branch's primary name.</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class CommandAliasAttribute(string alias) : Attribute
{
    /// <summary>
    /// The alias to register alongside the command's or branch's primary name.
    /// </summary>
    public string Alias { get; } = alias;
}
