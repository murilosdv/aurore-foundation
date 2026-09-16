using System;
using Aurore.Foundation.Core.Security;

namespace Aurore.Foundation.Tests.Core.Security;

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

    [Fact(DisplayName = "Encode<T> produces a different string per type for the same underlying identifier")]
    public void EncodeProducesDifferentStringsPerType()
    {
        // Arrange
        var id = 1;

        // Act
        var encodedForRegisteredEntity = id.Encode<RegisteredEntity>();
        var encodedForAnotherRegisteredEntity = id.Encode<AnotherRegisteredEntity>();

        // Assert
        Assert.NotEqual(encodedForRegisteredEntity, encodedForAnotherRegisteredEntity);
    }

    [Fact(DisplayName = "Encode produces a fixed-width string regardless of the identifier's value")]
    public void EncodeProducesFixedWidthOutput()
    {
        // Arrange
        var small = 1;
        var large = int.MaxValue;

        // Act
        var encodedSmall = small.Encode();
        var encodedLarge = large.Encode();

        // Assert
        Assert.Equal(encodedSmall.Length, encodedLarge.Length);
    }

    [Fact(DisplayName = "Decode returns -1 for a string of the wrong length")]
    public void DecodeReturnsNegativeOneForWrongLength()
    {
        // Arrange
        var tooShort = 1.Encode()[..^1];

        // Act
        var decoded = tooShort.Decode();

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

    [Fact(DisplayName = "TryDecode<T> returns true and the original identifier for a validly type-salted string")]
    public void TryDecodeWithTypeSaltReturnsTrueForValidInput()
    {
        // Arrange
        var encoded = 42.Encode<RegisteredEntity>();

        // Act
        var succeeded = encoded.TryDecode<RegisteredEntity>(out var id);

        // Assert
        Assert.True(succeeded);
        Assert.Equal(42, id);
    }

    [Fact(DisplayName = "DecodeOrThrow throws ArgumentException for a string that is not a valid obfuscated identifier")]
    public void DecodeOrThrowThrowsForInvalidInput()
    {
        // Arrange
        var invalid = "not-a-valid-id!";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => invalid.DecodeOrThrow());
    }

    [Fact(DisplayName = "DecodeOrThrow<T> returns the original identifier for a validly type-salted string")]
    public void DecodeOrThrowWithTypeSaltReturnsIdForValidInput()
    {
        // Arrange
        var encoded = 7.Encode<RegisteredEntity>();

        // Act
        var id = encoded.DecodeOrThrow<RegisteredEntity>();

        // Assert
        Assert.Equal(7, id);
    }

    [Fact(DisplayName = "Encode then DecodeLong round-trips back to the original identifier")]
    public void EncodeThenDecodeLongRoundTrips()
    {
        // Arrange
        var id = 12345L;

        // Act
        var encoded = id.Encode();
        var decoded = encoded.DecodeLong();

        // Assert
        Assert.Equal(id, decoded);
    }

    [Fact(DisplayName = "Encode then DecodeLong round-trips for a value beyond int.MaxValue")]
    public void EncodeThenDecodeLongRoundTripsBeyondIntRange()
    {
        // Arrange
        var id = (long)int.MaxValue + 1;

        // Act
        var encoded = id.Encode();
        var decoded = encoded.DecodeLong();

        // Assert
        Assert.Equal(id, decoded);
    }

    [Fact(DisplayName = "Encode<T> then DecodeLong<T> round-trips for a registered type")]
    public void EncodeAndDecodeLongWithTypeSaltRoundTrips()
    {
        // Arrange
        var id = 42L;

        // Act
        var encoded = id.Encode<RegisteredEntity>();
        var decoded = encoded.DecodeLong<RegisteredEntity>();

        // Assert
        Assert.Equal(id, decoded);
    }

    [Fact(DisplayName = "TryDecodeLong returns false for a string that is not a valid obfuscated identifier")]
    public void TryDecodeLongReturnsFalseForInvalidInput()
    {
        // Arrange
        var invalid = "not-a-valid-id!";

        // Act
        var succeeded = invalid.TryDecodeLong(out var id);

        // Assert
        Assert.False(succeeded);
        Assert.Equal(-1, id);
    }

    [Fact(DisplayName = "TryDecodeLong<T> returns true and the original identifier for a validly type-salted string")]
    public void TryDecodeLongWithTypeSaltReturnsTrueForValidInput()
    {
        // Arrange
        var encoded = 42L.Encode<RegisteredEntity>();

        // Act
        var succeeded = encoded.TryDecodeLong<RegisteredEntity>(out var id);

        // Assert
        Assert.True(succeeded);
        Assert.Equal(42L, id);
    }

    [Fact(DisplayName = "DecodeLongOrThrow throws ArgumentException for a string that is not a valid obfuscated identifier")]
    public void DecodeLongOrThrowThrowsForInvalidInput()
    {
        // Arrange
        var invalid = "not-a-valid-id!";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => invalid.DecodeLongOrThrow());
    }

    [Fact(DisplayName = "DecodeLongOrThrow<T> returns the original identifier for a validly type-salted string")]
    public void DecodeLongOrThrowWithTypeSaltReturnsIdForValidInput()
    {
        // Arrange
        var encoded = 7L.Encode<RegisteredEntity>();

        // Act
        var id = encoded.DecodeLongOrThrow<RegisteredEntity>();

        // Assert
        Assert.Equal(7L, id);
    }

    [Fact(DisplayName = "Configure throws when called a second time")]
    public void ConfigureThrowsOnSecondCall()
    {
        // Arrange & Act
        var act = () => Obfuscator.Configure();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }
}

[CollectionDefinition(nameof(ObfuscatorCollection))]
public class ObfuscatorCollection : ICollectionFixture<ObfuscatorFixture>;
