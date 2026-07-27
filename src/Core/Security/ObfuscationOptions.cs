using System.Diagnostics.CodeAnalysis;

namespace Aurore.Foundation.Core.Security;

/// <summary>
/// Configures the identifier obfuscation scheme used to encode/decode integer IDs into opaque strings.
/// </summary>
/// <param name="MinimumLength">The minimum length of an encoded identifier.</param>
/// <param name="Alphabet">The set of characters used to build encoded identifiers.</param>
[ExcludeFromCodeCoverage]
public sealed record ObfuscationOptions(int MinimumLength, string Alphabet);
