using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Synonms.MultiTenancy.Abstractions.Context;
using Synonms.MultiTenancy.Abstractions.Resolution;
using Synonms.MultiTenancy.AspNetCore;
using Synonms.MultiTenancy.Configuration;
using Synonms.MultiTenancy.Context;
using Synonms.MultiTenancy.Resolution;

namespace Synonms.MultiTenancy.IoC;

public static class ServiceCollectionExtensions
{
    public static MultiTenancyServiceBuilder AddMultiTenancy(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.Configure<MultiTenancyOptions>(configuration);

        serviceCollection.AddScoped<MultiTenancyMiddleware>();
        serviceCollection.AddScoped<IMultiTenancyContextAccessor, MultiTenancyContextAccessor>();
        serviceCollection.AddScoped<IMultiTenancyContextFactory, MultiTenancyContextFactory>();
        serviceCollection.AddScoped<ITenantResolver, TenantResolver>();
            
        return new MultiTenancyServiceBuilder(serviceCollection);
    }
}