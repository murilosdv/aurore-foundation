using System;

namespace Aurore.Foundation.Tests.CliCore.Fixtures;

public interface IGreeter
{
    void Greet();
}

public sealed class Greeter(Action? onGreet = null) : IGreeter
{
    public void Greet()
    {
        onGreet?.Invoke();
    }
}
