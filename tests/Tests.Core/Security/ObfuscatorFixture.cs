using Aurore.Foundation.Core.Security;

namespace Aurore.Foundation.Tests.Core.Security;

/// <summary>
/// Calls the process-wide, call-once <see cref="Obfuscator.Configure"/> a single time for <see cref="ObfuscatorTests"/>,
/// since a second call anywhere in the process would throw.
/// </summary>
public sealed class ObfuscatorFixture
{
    public ObfuscatorFixture()
    {
        Obfuscator.Configure(options =>
        {
            options.MinimumLength = 5;
            options.Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            options.Register<RegisteredEntity>();
        });
    }
}

public sealed class RegisteredEntity;

public sealed class UnregisteredEntity;
