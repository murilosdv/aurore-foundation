using System.Threading;
using System.Threading.Tasks;
using Aurore.Foundation.Core.Abstractions;
using Aurore.Foundation.Core.Configuration;
using Aurore.Foundation.Core.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Aurore.Foundation.Tests.Core.Configuration;

public class FeatureConfigurationTests
{
    private sealed class ConcreteResult;

    private sealed class RequestFeature : IFeature<ConcreteResult>
    {
        public ValueTask<Result<ConcreteResult>> HandleAsync(CancellationToken cancellationToken)
        {
            return ValueTask.FromResult<Result<ConcreteResult>>(new ConcreteResult());
        }
    }

    private sealed class RequestResponseFeature : IFeature<string, ConcreteResult>
    {
        public ValueTask<Result<ConcreteResult>> HandleAsync(string request, CancellationToken cancellationToken)
        {
            return ValueTask.FromResult<Result<ConcreteResult>>(new ConcreteResult());
        }
    }

    private abstract class AbstractFeature : IFeature<ConcreteResult>
    {
        public abstract ValueTask<Result<ConcreteResult>> HandleAsync(CancellationToken cancellationToken);
    }

    private sealed class NotAFeature;

    [Fact(DisplayName = "AddFeaturesFromAssembly registers every concrete IFeature<TResult> and IFeature<TRequest, TResult> implementation")]
    public void RegistersConcreteFeatureImplementations()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddFeaturesFromAssembly<FeatureConfigurationTests>();

        // Assert
        Assert.Contains(services, d => d.ServiceType == typeof(RequestFeature));
        Assert.Contains(services, d => d.ServiceType == typeof(RequestResponseFeature));
    }

    [Fact(DisplayName = "AddFeaturesFromAssembly does not register abstract types or classes that do not implement IFeature")]
    public void SkipsAbstractTypesAndNonFeatureClasses()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddFeaturesFromAssembly<FeatureConfigurationTests>();

        // Assert
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(AbstractFeature));
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(NotAFeature));
    }

    [Fact(DisplayName = "AddFeaturesFromAssembly registers features with the requested service lifetime")]
    public void RegistersFeaturesWithRequestedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddFeaturesFromAssembly<FeatureConfigurationTests>(ServiceLifetime.Singleton);

        // Assert
        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(RequestFeature));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }
}
