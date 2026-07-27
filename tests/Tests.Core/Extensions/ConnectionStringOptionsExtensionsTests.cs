using Aurore.Foundation.Core.Extensions;
using Aurore.Foundation.Core.Options;

namespace Aurore.Foundation.Tests.Core.Extensions;

public class ConnectionStringOptionsExtensionsTests
{
    [Fact(DisplayName = "BuildConnectionString prepends the server segment and formats each key with the given function")]
    public void BuildConnectionStringPrependsServerAndFormatsKeys()
    {
        // Arrange
        var settings = new DatabaseConnectionStringOptions
        {
            Server = "db.internal",
            Options = new()
            {
                ["port_number"] = 5432,
                ["timeout"] = 30
            }
        };

        // Act
        var result = settings.BuildConnectionString(';', '=', "Host=db.internal", key => key.ToUpperInvariant());

        // Assert
        Assert.Equal("Host=db.internal;PORT_NUMBER=5432;TIMEOUT=30", result);
    }

    [Fact(DisplayName = "BuildPostgresConnectionString Pascal-cases keys and separates segments with a semicolon")]
    public void BuildPostgresConnectionStringPascalizesKeys()
    {
        // Arrange
        var settings = new DatabaseConnectionStringOptions
        {
            Server = "db.internal",
            Options = new()
            {
                ["port_number"] = 5432,
                ["ssl_mode"] = "Require"
            }
        };

        // Act
        var result = settings.BuildPostgresConnectionString();

        // Assert
        Assert.Equal("Host=db.internal;PortNumber=5432;SslMode=Require", result);
    }
}
