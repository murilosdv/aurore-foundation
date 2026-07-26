using System;
using Aurore.Foundation.Core.Security;

namespace Tests.Core.Security;

[Collection(nameof(ObfuscatorCollection))]
public class ObfuscatorTests
{
    [Fact(DisplayName = "Encode then Decode round-trips back to the original identifier")]
    public void EncodeThenDecodeRoundTrips()
    {
        // Arrange
        var id = 12345;

        // Act
        var encoded = id.Encode();
        var decoded = encoded.Decode();

        // Assert
        Assert.Equal(id, decoded);
    }

    [Fact(DisplayName = "Encode<T> then Decode<T> round-trips for a registered type")]
    public void EncodeAndDecodeWithTypeSaltRoundTrips()
    {
        // Arrange
        var id = 42;

        // Act
        var encoded = id.Encode<RegisteredEntity>();
        var decoded = encoded.Decode<RegisteredEntity>();

        // Assert
        Assert.Equal(id, decoded);
    }

    [Fact(DisplayName = "A type-salted identifier cannot be decoded as a plain (non-salted) identifier")]
    public void TypeSaltedIdentifierIsNotAPlainIdentifier()
    {
        // Arrange
        var encoded = 42.Encode<RegisteredEntity>();

        // Act
        var decoded = encoded.Decode();

        // Assert
        Assert.Equal(-1, decoded);
    }

    [Fact(DisplayName = "Encode<T> throws for a type that has not been registered")]
    public void EncodeThrowsForUnregisteredType()
    {
        // Arrange & Act & Assert
        Assert.Throws<InvalidOperationException>(() => 1.Encode<UnregisteredEntity>());
    }

    [Fact(DisplayName = "TryDecode returns false for a string that is not a valid obfuscated identifier")]
    public void TryDecodeReturnsFalseForInvalidInput()
    {
        // Arrange
        var invalid = "not-a-valid-id!";

        // Act
        var succeeded = invalid.TryDecode(out var id);

        // Assert
        Assert.False(succeeded);
        Assert.Equal(-1, id);
    }

    [Fact(DisplayName = "DecodeOrThrow throws ArgumentException for a string that is not a valid obfuscated identifier")]
    public void DecodeOrThrowThrowsForInvalidInput()
    {
        // Arrange
        var invalid = "not-a-valid-id!";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => invalid.DecodeOrThrow());
    }

    [Fact(DisplayName = "Configure throws when called a second time")]
    public void ConfigureThrowsOnSecondCall()
    {
        // Arrange & Act
        var act = () => Obfuscator.Configure(_ => { });

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }
}

[CollectionDefinition(nameof(ObfuscatorCollection))]
public class ObfuscatorCollection : ICollectionFixture<ObfuscatorFixture>;
