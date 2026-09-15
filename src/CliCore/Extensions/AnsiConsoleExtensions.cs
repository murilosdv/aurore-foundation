using System;
using Spectre.Console;

namespace Aurore.Foundation.CliCore.Extensions;

/// <summary>
/// Provides consistent, severity-colored console output for CLI applications, safely escaping
/// dynamic content so it can never be misinterpreted as Spectre markup.
/// </summary>
public static class AnsiConsoleExtensions
{
    extension(AnsiConsole)
    {
        /// <summary>
        /// Writes a message in the informational color.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public static void WriteInfo(string message)
        {
            WriteColored(message, Color.Blue);
        }

        /// <summary>
        /// Writes a label followed by an emphasized value, in the informational color.
        /// </summary>
        /// <param name="label">The label describing the value.</param>
        /// <param name="value">The value to emphasize.</param>
        public static void WriteInfo(string label, object value)
        {
            WriteLabel(Color.Blue, label, value);
        }

        /// <summary>
        /// Writes a message in the success color.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public static void WriteSuccess(string message)
        {
            WriteColored(message, Color.Green);
        }

        /// <summary>
        /// Writes a label followed by an emphasized value, in the success color.
        /// </summary>
        /// <param name="label">The label describing the value.</param>
        /// <param name="value">The value to emphasize.</param>
        public static void WriteSuccess(string label, object value)
        {
            WriteLabel(Color.Green, label, value);
        }

        /// <summary>
        /// Writes a message in the warning color.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public static void WriteWarning(string message)
        {
            WriteColored(message, Color.Yellow);
        }

        /// <summary>
        /// Writes a label followed by an emphasized value, in the warning color.
        /// </summary>
        /// <param name="label">The label describing the value.</param>
        /// <param name="value">The value to emphasize.</param>
        public static void WriteWarning(string label, object value)
        {
            WriteLabel(Color.Yellow, label, value);
        }

        /// <summary>
        /// Writes a message in the error color.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public static void WriteError(string message)
        {
            WriteColored(message, Color.Red);
        }

        /// <summary>
        /// Writes a label followed by an emphasized value, in the error color.
        /// </summary>
        /// <param name="label">The label describing the value.</param>
        /// <param name="value">The value to emphasize.</param>
        public static void WriteError(string label, object value)
        {
            WriteLabel(Color.Red, label, value);
        }

        /// <summary>
        /// Writes an exception's message in the error color.
        /// </summary>
        /// <param name="ex">The exception whose message should be written.</param>
        public static void WriteError(Exception ex)
        {
            WriteColored(ex.Message, Color.Red);
        }

        /// <summary>
        /// Writes a label followed by an exception's message, in the error color.
        /// </summary>
        /// <param name="label">The label describing the exception.</param>
        /// <param name="ex">The exception whose message should be written.</param>
        public static void WriteError(string label, Exception ex)
        {
            WriteLabel(Color.Red, label, ex.Message);
        }

        /// <summary>
        /// Writes a label followed by an emphasized value, in the default emphasis color.
        /// </summary>
        /// <param name="label">The label describing the value.</param>
        /// <param name="value">The value to emphasize.</param>
        public static void WriteEmphasized(string label, object value)
        {
            WriteLabel(Color.Yellow, label, value);
        }

        /// <summary>
        /// Writes a message in the given color, safely escaping it so it can never be
        /// misinterpreted as Spectre markup.
        /// </summary>
        /// <param name="text">The message to write.</param>
        /// <param name="color">The color to write it in.</param>
        public static void WriteColored(string text, Color color)
        {
            AnsiConsole.MarkupLine($"[bold {color}]{Markup.Escape(text)}[/]");
        }

        /// <summary>
        /// Writes the given title as large Figlet text, in a randomly chosen color.
        /// </summary>
        /// <param name="title">The title to render.</param>
        public static void WriteAppTitle(string title)
        {
            var colors = new[] { Color.Red, Color.Green, Color.Yellow, Color.Blue, Color.Magenta, Color.Cyan, Color.Aqua, Color.Pink1 };

            var color = colors[Random.Shared.Next(colors.Length)];

            AnsiConsole.Write(new FigletText(title).Color(color));
        }

        /// <summary>
        /// Writes an empty line.
        /// </summary>
        public static void LineBreak()
        {
            Console.WriteLine(string.Empty);
        }

        private static void WriteLabel(Color color, string label, object? value)
        {
            var text = value?.ToString() ?? string.Empty;

            AnsiConsole.MarkupLine($"[bold {color}]{Markup.Escape(label)}:[/] [bold]{Markup.Escape(text)}[/]");
        }
    }
}
