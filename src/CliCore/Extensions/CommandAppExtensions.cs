using System;
using Aurore.Foundation.CLI.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Aurore.Foundation.CLI.Core.Extensions;

/// <summary>
/// Provides factory helpers for creating a Spectre.Console.Cli <see cref="CommandApp"/>.
/// </summary>
public static class CommandAppExtensions
{
    extension(CommandApp)
    {
        /// <summary>
        /// Creates a <see cref="CommandApp"/> wired to a dependency-injection container, so
        /// commands can receive constructor-injected dependencies.
        /// </summary>
        /// <param name="configure">A callback that registers the app's services.</param>
        public static CommandApp CreateWithServices(Action<IServiceCollection> configure)
        {
            var services = new ServiceCollection();

            configure.Invoke(services);

            return new CommandApp(new DependencyRegistrar(services));
        }
    }
}
