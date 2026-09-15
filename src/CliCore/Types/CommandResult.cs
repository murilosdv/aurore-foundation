namespace Aurore.Foundation.CliCore.Types;

/// <summary>
/// The captured output of a completed external process.
/// </summary>
/// <param name="Stdout">The process's standard output.</param>
/// <param name="Stderr">The process's standard error output.</param>
/// <param name="ExitCode">The process's exit code.</param>
public sealed record CommandResult(string Stdout, string Stderr, int ExitCode)
{
    /// <summary>
    /// Whether the process exited with code 0.
    /// </summary>
    public bool IsSuccess => ExitCode is 0;
}
