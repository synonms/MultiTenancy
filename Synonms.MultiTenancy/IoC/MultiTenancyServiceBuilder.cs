using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Synonms.MultiTenancy.Abstractions.Resolution;

namespace Synonms.MultiTenancy.IoC;

public class MultiTenancyServiceBuilder
{
    public MultiTenancyServiceBuilder(IServiceCollection serviceCollection)
    {
        ServiceCollection = serviceCollection;
    }

    public IServiceCollection ServiceCollection { get; }

    public MultiTenancyServiceBuilder WithResolutionStrategiesFrom(Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Transient) 
    {
        foreach (Type type in assembly.GetTypes().Where(x => !x.IsInterface && !x.IsAbstract && x.GetInterfaces().Contains(typeof(ITenantResolutionStrategy))))
        {
            ServiceCollection.Add(ServiceDescriptor.Describe(typeof(ITenantResolutionStrategy), type, lifetime));
        }

        return this;
    }
}