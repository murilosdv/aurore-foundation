using Aurore.Foundation.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;

namespace Tests.AspNetCore.Extensions;

public class HttpContextExtensionsTests
{
    [Fact(DisplayName = "GetRequestHeader returns the header value when the header is present")]
    public void GetRequestHeaderReturnsValueWhenPresent()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Test-Header"] = "some-value";

        // Act
        var result = context.GetRequestHeader("X-Test-Header");

        // Assert
        Assert.Equal("some-value", result);
    }

    [Fact(DisplayName = "GetRequestHeader returns an empty string when the header is absent and no fallback is given")]
    public void GetRequestHeaderReturnsEmptyWhenAbsentWithNoFallback()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act
        var result = context.GetRequestHeader("X-Missing-Header");

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact(DisplayName = "GetRequestHeader returns the fallback value when the header is absent")]
    public void GetRequestHeaderReturnsFallbackWhenAbsent()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act
        var result = context.GetRequestHeader("X-Missing-Header", "fallback-value");

        // Assert
        Assert.Equal("fallback-value", result);
    }
}
