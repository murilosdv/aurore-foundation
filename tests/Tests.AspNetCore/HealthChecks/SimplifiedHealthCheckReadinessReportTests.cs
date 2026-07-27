using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Aurore.Foundation.AspNetCore.HealthChecks;
using Aurore.Foundation.Core.Constants;
using Aurore.Foundation.TestBed.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Aurore.Foundation.Tests.AspNetCore.HealthChecks;

public class SimplifiedHealthCheckReadinessReportTests
{
    private sealed record TestableReport : SimplifiedHealthCheckReadinessReport
    {
        public static HealthStatus ExposedGetStatus(HealthReport report)
        {
            return GetStatus(report);
        }
    }

    private static HealthReportEntry CreateEntry(HealthStatus status, params string[] tags)
    {
        return new HealthReportEntry(status, "description", TimeSpan.FromMilliseconds(5), null, null, tags);
    }

    [Fact(DisplayName = "GetStatus stays Unhealthy when a critical-tagged entry is unhealthy")]
    public void StaysUnhealthyWhenCriticalEntryIsUnhealthy()
    {
        // Arrange
        var entries = new Dictionary<string, HealthReportEntry>
        {
            ["critical-dependency"] = CreateEntry(HealthStatus.Unhealthy, HealthCheckProperties.Tags.Critical),
            ["other-dependency"] = CreateEntry(HealthStatus.Healthy)
        };
        var report = new HealthReport(entries, HealthStatus.Unhealthy, TimeSpan.FromSeconds(1));

        // Act
        var status = TestableReport.ExposedGetStatus(report);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, status);
    }

    [Fact(DisplayName = "GetStatus degrades to Degraded when unhealthy entries are not tagged critical")]
    public void DegradesToDegradedWhenNoCriticalEntryIsUnhealthy()
    {
        // Arrange
        var entries = new Dictionary<string, HealthReportEntry>
        {
            ["non-critical-dependency"] = CreateEntry(HealthStatus.Unhealthy, HealthCheckProperties.Tags.NonCritical)
        };
        var report = new HealthReport(entries, HealthStatus.Unhealthy, TimeSpan.FromSeconds(1));

        // Act
        var status = TestableReport.ExposedGetStatus(report);

        // Assert
        Assert.Equal(HealthStatus.Degraded, status);
    }

    [Fact(DisplayName = "GetStatus passes Healthy through unchanged")]
    public void PassesHealthyThroughUnchanged()
    {
        // Arrange
        var entries = new Dictionary<string, HealthReportEntry>
        {
            ["dependency"] = CreateEntry(HealthStatus.Healthy)
        };
        var report = new HealthReport(entries, HealthStatus.Healthy, TimeSpan.FromSeconds(1));

        // Act
        var status = TestableReport.ExposedGetStatus(report);

        // Assert
        Assert.Equal(HealthStatus.Healthy, status);
    }

    [Fact(DisplayName = "GetStatus passes Degraded through unchanged")]
    public void PassesDegradedThroughUnchanged()
    {
        // Arrange
        var entries = new Dictionary<string, HealthReportEntry>
        {
            ["dependency"] = CreateEntry(HealthStatus.Degraded)
        };
        var report = new HealthReport(entries, HealthStatus.Degraded, TimeSpan.FromSeconds(1));

        // Act
        var status = TestableReport.ExposedGetStatus(report);

        // Assert
        Assert.Equal(HealthStatus.Degraded, status);
    }

    [Fact(DisplayName = "HealthCheckReadinessReport.WriteResponseAsync writes a dependencies array with one entry per health report entry")]
    public async Task WriteResponseAsyncWritesDependenciesArray()
    {
        // Arrange
        var entries = new Dictionary<string, HealthReportEntry>
        {
            ["database"] = CreateEntry(HealthStatus.Healthy, HealthCheckProperties.Tags.Critical, HealthCheckProperties.Tags.TypeDatabase)
        };
        var report = new HealthReport(entries, HealthStatus.Healthy, TimeSpan.FromSeconds(2));
        var body = new MemoryStream();
        var context = HttpContextSetup.Create().WithBody(body).Build();

        // Act
        await HealthCheckReadinessReport.WriteResponseAsync(context, report);

        // Assert
        body.Seek(0, SeekOrigin.Begin);
        using var document = await JsonDocument.ParseAsync(body, cancellationToken: TestContext.Current.CancellationToken);
        var dependencies = document.RootElement.GetProperty("dependencies");
        Assert.Equal(1, dependencies.GetArrayLength());

        var dependency = dependencies[0];
        Assert.Equal("database", dependency.GetProperty("name").GetString());
        Assert.Equal((int)HealthStatus.Healthy, dependency.GetProperty("status").GetInt32());
        Assert.True(dependency.TryGetProperty("duration", out _));

        var tags = dependency.GetProperty("tags");
        var tagValues = new List<string?>();
        foreach (var tag in tags.EnumerateArray())
            tagValues.Add(tag.GetString());

        Assert.Contains(HealthCheckProperties.Tags.Critical, tagValues);
        Assert.Contains(HealthCheckProperties.Tags.TypeDatabase, tagValues);
    }
}
