using Microsoft.Extensions.Logging;

namespace Aurore.Foundation.Core.Logging;

/// <summary>
/// Provides extension members for <see cref="ILoggerFactory"/> to simplify console logging setup.
/// </summary>
public static class LoggingExtensions
{
    extension(ILoggerFactory factory)
    {
        /// <summary>
        /// Creates a new <see cref="ILoggerFactory"/> preconfigured with a single-line, UTC-timestamped console logger
        /// at <see cref="LogLevel.Debug"/>, suitable for early application bootstrap.
        /// </summary>
        /// <returns>A new, console-configured <see cref="ILoggerFactory"/>.</returns>
        public static ILoggerFactory BootstrapForConsole()
        {
            return LoggerFactory.Create(log =>
            {
                log.SetMinimumLevel(LogLevel.Debug);
                log.AddSimpleConsole(o =>
                {
                    o.SingleLine = true;
                    o.UseUtcTimestamp = true;
                    o.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
                });
            });
        }

        /// <summary>
        /// Creates a logger named <c>"ConsoleLogger"</c> from this factory.
        /// </summary>
        /// <returns>The created <see cref="ILogger"/>.</returns>
        public ILogger CreateSimpleConsoleLogger()
        {
            return factory.CreateLogger("ConsoleLogger");
        }
    }
}
