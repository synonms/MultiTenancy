using Synonms.MultiTenancy.Abstractions.Models;

namespace Synonms.MultiTenancy.Abstractions.Context;

public class MultiTenancyContext
{
    private MultiTenancyContext(Tenant tenant)
    {
        Tenant = tenant;
    }

    public Tenant Tenant { get; }

    public static MultiTenancyContext Create(Tenant tenant) =>
        new(tenant);
}