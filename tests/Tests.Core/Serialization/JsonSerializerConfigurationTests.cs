using System;
using System.IO;
using Aurore.Foundation.Core.Serialization;

namespace Tests.Core.Serialization;

public class JsonSerializerConfigurationTests
{
    private enum Status
    {
        Active,
        Inactive
    }

    private sealed record Sample(string DisplayName, Status Status);

    [Fact(DisplayName = "Serialize writes camelCase property names and enums as strings")]
    public void SerializeUsesCamelCasePropertiesAndStringEnums()
    {
        // Arrange
        var sample = new Sample("Aurore", Status.Active);

        // Act
        var json = sample.Serialize();

        // Assert
        Assert.Equal("""{"displayName":"Aurore","status":"Active"}""", json);
    }

    [Fact(DisplayName = "Deserialize reads property names case-insensitively and parses string enums")]
    public void DeserializeIsCaseInsensitiveAndParsesStringEnums()
    {
        // Arrange
        var json = """{"DISPLAYNAME":"Aurore","STATUS":"Inactive"}""";

        // Act
        var sample = json.Deserialize<Sample>();

        // Assert
        Assert.Equal(new Sample("Aurore", Status.Inactive), sample);
    }

    [Fact(DisplayName = "Serializing then deserializing a value round-trips it back to an equal instance")]
    public void SerializeThenDeserializeRoundTrips()
    {
        // Arrange
        var sample = new Sample("Aurore Foundation", Status.Active);

        // Act
        var json = sample.Serialize();
        var result = json.Deserialize<Sample>();

        // Assert
        Assert.Equal(sample, result);
    }

    [Fact(DisplayName = "SerializeToFile then DeserializeFromFile round-trips a value through disk")]
    public void SerializeToFileThenDeserializeFromFileRoundTrips()
    {
        // Arrange
        var sample = new Sample("Aurore Foundation", Status.Inactive);
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

        try
        {
            // Act
            sample.SerializeToFile(path);
            var result = path.DeserializeFromFile<Sample>();

            // Assert
            Assert.Equal(sample, result);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact(DisplayName = "DeserializeFromFile throws FileNotFoundException when the file does not exist")]
    public void DeserializeFromFileThrowsWhenFileMissing()
    {
        // Arrange
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

        // Act
        var act = () => path.DeserializeFromFile<Sample>();

        // Assert
        Assert.Throws<FileNotFoundException>(act);
    }
}
