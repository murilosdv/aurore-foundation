using System;
using System.Collections.Generic;
using System.Linq;
using Aurore.Foundation.CLI.Core.Abstractions;
using Aurore.Foundation.CLI.Core.Attributes;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aurore.Foundation.CLI.Core.Extensions;

/// <summary>
/// Provides attribute-driven registration of commands and branches against Spectre.Console.Cli,
/// and reflection helpers for reading their <see cref="CommandInfoAttribute"/>,
/// <see cref="CommandExampleAttribute"/> and <see cref="CommandAliasAttribute"/> metadata.
/// </summary>
public static class CommandExtensions
{
    extension(Type type)
    {
        /// <summary>
        /// Gets every <see cref="CommandExampleAttribute"/> declared on this type.
        /// </summary>
        public IEnumerable<CommandExampleAttribute> GetCommandExamples()
        {
            return type.GetAttributes<CommandExampleAttribute>();
        }

        /// <summary>
        /// Gets every alias declared on this type via <see cref="CommandAliasAttribute"/>.
        /// </summary>
        public IEnumerable<string> GetCommandAliases()
        {
            return type.GetAttributes<CommandAliasAttribute>().Select(x => x.Alias);
        }

        /// <summary>
        /// Gets this type's <see cref="CommandInfoAttribute"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">The type isn't decorated with <see cref="CommandInfoAttribute"/>.</exception>
        public CommandInfoAttribute GetCommandMetadata()
        {
            return type.GetAttributes<CommandInfoAttribute>().FirstOrDefault()
                ?? throw new InvalidOperationException("Command must be decorated with [[CommandInfo]].");
        }

        private IEnumerable<TAttribute> GetAttributes<TAttribute>()
        {
            return type
                .GetCustomAttributes(typeof(TAttribute), false)
                .Cast<TAttribute>();
        }
    }

    /// <summary>
    /// Installs a handler that renders any exception thrown while running a command via
    /// <see cref="AnsiConsole.WriteException(Exception, ExceptionFormats)"/> and returns a fixed
    /// exit code, instead of Spectre's own default exception handling.
    /// </summary>
    /// <param name="configurator">The app configurator to install the handler on.</param>
    /// <param name="exitCode">The exit code to return when a command throws.</param>
    /// <returns>The same configurator, for chaining.</returns>
    public static IConfigurator UseDefaultExceptionHandler(this IConfigurator configurator, int exitCode = 1)
    {
        configurator.Settings.ExceptionHandler = (exception, _) =>
        {
            AnsiConsole.WriteException(exception);

            return exitCode;
        };

        return configurator;
    }

    /// <summary>
    /// Registers <typeparamref name="TCommand"/> under the name declared by its
    /// <see cref="CommandInfoAttribute"/>, applying its declared examples and aliases.
    /// </summary>
    /// <typeparam name="TCommand">The command type to register.</typeparam>
    /// <param name="configurator">The app configurator to register the command on.</param>
    public static ICommandConfigurator ConfigureCommand<TCommand>(this IConfigurator configurator)
        where TCommand : class, ICommandLimiter<CommandSettings>
    {
        return ConfigureCommandInternal<TCommand, IConfigurator>(configurator, (c, name) => c.AddCommand<TCommand>(name));
    }

    /// <summary>
    /// Registers <typeparamref name="TCommand"/> under the name declared by its
    /// <see cref="CommandInfoAttribute"/>, applying its declared examples and aliases.
    /// </summary>
    /// <typeparam name="TCommand">The command type to register.</typeparam>
    /// <param name="configurator">The branch configurator to register the command on.</param>
    public static ICommandConfigurator ConfigureCommand<TCommand>(this IConfigurator<CommandSettings> configurator)
        where TCommand : class, ICommandLimiter<CommandSettings>
    {
        return ConfigureCommandInternal<TCommand, IConfigurator<CommandSettings>>(configurator, (c, name) => c.AddCommand<TCommand>(name));
    }

    /// <summary>
    /// Registers <typeparamref name="TBranch"/> under the name declared by its
    /// <see cref="CommandInfoAttribute"/>, applying its declared aliases and letting it configure
    /// its own nested commands and sub-branches.
    /// </summary>
    /// <typeparam name="TBranch">The branch type to register.</typeparam>
    /// <param name="configurator">The app configurator to register the branch on.</param>
    public static void ConfigureBranch<TBranch>(this IConfigurator configurator)
        where TBranch : ICommandBranch
    {
        ConfigureBranchInternal<TBranch, IConfigurator>(configurator, (c, name, action) => c.AddBranch(name, action));
    }

    /// <summary>
    /// Registers <typeparamref name="TBranch"/> under the name declared by its
    /// <see cref="CommandInfoAttribute"/>, applying its declared aliases and letting it configure
    /// its own nested commands and sub-branches.
    /// </summary>
    /// <typeparam name="TBranch">The branch type to register.</typeparam>
    /// <param name="configurator">The branch configurator to register the branch on.</param>
    public static void ConfigureBranch<TBranch>(this IConfigurator<CommandSettings> configurator)
        where TBranch : ICommandBranch
    {
        ConfigureBranchInternal<TBranch, IConfigurator<CommandSettings>>(configurator, (c, name, action) => c.AddBranch(name, action));
    }

    private static ICommandConfigurator ConfigureCommandInternal<TCommand, TConfigurator>(
        TConfigurator configurator,
        Func<TConfigurator, string, ICommandConfigurator> addCommand)
        where TCommand : class, ICommandLimiter<CommandSettings>
    {
        var meta = typeof(TCommand).GetCommandMetadata();

        var examples = typeof(TCommand).GetCommandExamples();

        var aliases = typeof(TCommand).GetCommandAliases();

        var cmd = addCommand(configurator, meta.Name)
            .WithDescription(meta.Description);

        foreach (var example in examples)
            cmd.WithExample(example.Args);

        foreach (var alias in aliases)
            cmd.WithAlias(alias);

        return cmd;
    }

    private static void ConfigureBranchInternal<TBranch, TConfigurator>(
        TConfigurator configurator,
        Func<TConfigurator, string, Action<IConfigurator<CommandSettings>>, IBranchConfigurator> addBranch)
        where TBranch : ICommandBranch
    {
        var meta = typeof(TBranch).GetCommandMetadata();

        var aliases = typeof(TBranch).GetCommandAliases();

        var branch = addBranch(configurator, meta.Name, settings =>
        {
            settings.SetDescription(meta.Description);

            TBranch.Configure(settings);
        });

        foreach (var alias in aliases)
            branch.WithAlias(alias);
    }
}
