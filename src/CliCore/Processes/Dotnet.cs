using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Aurore.Foundation.CLI.Core.Processes;

/// <summary>
/// Wraps common <c>dotnet</c> CLI invocations (tool management, EF Core migrations/database
/// commands, and packing) as typed, awaitable methods.
/// </summary>
public static class Dotnet
{
    /// <summary>
    /// Wraps <c>dotnet tool</c> invocations.
    /// </summary>
    public static class Tool
    {
        /// <summary>
        /// Installs a .NET global tool.
        /// </summary>
        /// <param name="packageId">The tool's NuGet package ID.</param>
        /// <param name="cancellationToken">A token to cancel waiting for the process to exit.</param>
        public static async Task InstallAsync(string packageId, CancellationToken cancellationToken = default)
        {
            await RunAsync(["tool", "install --global", packageId], cancellationToken);
        }

        /// <summary>
        /// Installs a .NET global tool from a specific source and version.
        /// </summary>
        /// <param name="source">The NuGet source to install from.</param>
        /// <param name="packageId">The tool's NuGet package ID.</param>
        /// <param name="version">The exact version to install.</param>
        /// <param name="cancellationToken">A token to cancel waiting for the process to exit.</param>
        public static async Task InstallAsync(string source, string packageId, string version, CancellationToken cancellationToken = default)
        {
            await RunAsync(["tool", "install --global", "--add-source", source, packageId, "--version", version], cancellationToken);
        }

        /// <summary>
        /// Updates a .NET global tool to its latest version.
        /// </summary>
        /// <param name="packageId">The tool's NuGet package ID.</param>
        /// <param name="cancellationToken">A token to cancel waiting for the process to exit.</param>
        public static async Task UpdateAsync(string packageId, CancellationToken cancellationToken = default)
        {
            await RunAsync(["tool", "update --global", packageId], cancellationToken);
        }

        /// <summary>
        /// Uninstalls a .NET global tool.
        /// </summary>
        /// <param name="packageId">The tool's NuGet package ID.</param>
        /// <param name="cancellationToken">A token to cancel waiting for the process to exit.</param>
        public static async Task UninstallAsync(string packageId, CancellationToken cancellationToken = default)
        {
            await RunAsync(["tool", "uninstall --global", packageId], cancellationToken);
        }
    }

    /// <summary>
    /// Wraps <c>dotnet ef migrations</c> invocations.
    /// </summary>
    public static class Migrations
    {
        /// <summary>
        /// Adds a new EF Core migration.
        /// </summary>
        /// <param name="migrationName">The name of the migration to add.</param>
        /// <param name="projectPath">The path to the project containing the DbContext.</param>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="cancellationToken">A token to cancel waiting for the process to exit.</param>
        public static async Task AddAsync(string migrationName, string projectPath, string connectionString, CancellationToken cancellationToken = default)
        {
            await RunAsync(["ef migrations add", migrationName, "--project", projectPath, "-o History", "--", connectionString], cancellationToken);
        }

        /// <summary>
        /// Removes the most recent EF Core migration.
        /// </summary>
        /// <param name="projectPath">The path to the project containing the DbContext.</param>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="cancellationToken">A token to cancel waiting for the process to exit.</param>
        public static async Task RemoveAsync(string projectPath, string connectionString, CancellationToken cancellationToken = default)
        {
            await RunAsync(["ef migrations remove", "--project", projectPath, "--", connectionString], cancellationToken);
        }

        /// <summary>
        /// Bundles all EF Core migrations into a standalone executable.
        /// </summary>
        /// <param name="projectPath">The path to the project containing the DbContext.</param>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="cancellationToken">A token to cancel waiting for the process to exit.</param>
        public static async Task BundleAsync(string projectPath, string connectionString, CancellationToken cancellationToken = default)
        {
            await RunAsync(["ef migrations bundle", "--project", projectPath, "--", connectionString], cancellationToken);
        }
    }

    /// <summary>
    /// Wraps <c>dotnet ef database</c> invocations.
    /// </summary>
    public static class Database
    {
        /// <summary>
        /// Applies pending EF Core migrations to the database.
        /// </summary>
        /// <param name="projectPath">The path to the project containing the DbContext.</param>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="cancellationToken">A token to cancel waiting for the process to exit.</param>
        public static async Task UpdateAsync(string projectPath, string connectionString, CancellationToken cancellationToken = default)
        {
            await RunAsync(["ef database update", "--project", projectPath, "--", connectionString], cancellationToken);
        }

        /// <summary>
        /// Drops the database.
        /// </summary>
        /// <param name="projectPath">The path to the project containing the DbContext.</param>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="cancellationToken">A token to cancel waiting for the process to exit.</param>
        public static async Task DropAsync(string projectPath, string connectionString, CancellationToken cancellationToken = default)
        {
            await RunAsync(["ef database drop", "--project", projectPath, "--force", "--", connectionString], cancellationToken);
        }
    }

    /// <summary>
    /// Packs a project into a NuGet package.
    /// </summary>
    /// <param name="projectPath">The path to the project to pack.</param>
    /// <param name="version">The package version to stamp the package with.</param>
    /// <param name="output">The directory to write the resulting package to.</param>
    /// <param name="cancellationToken">A token to cancel waiting for the process to exit.</param>
    public static async Task PackAsync(string projectPath, string version, string output, CancellationToken cancellationToken = default)
    {
        await RunAsync(["pack", projectPath, "-c Release", $"-p:Version={version}", "-o", output], cancellationToken);
    }

    private static async Task RunAsync(string[] args, CancellationToken cancellationToken = default)
    {
        var process = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = string.Join(' ', args),
            UseShellExecute = false
        });

        await process!.WaitForExitAsync(cancellationToken);
    }
}
