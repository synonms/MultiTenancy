using System;
using System.Threading.Tasks;
using Synonms.Functional;
using Synonms.MultiTenancy.Abstractions.Context;
using Synonms.MultiTenancy.Abstractions.Models;
using Synonms.MultiTenancy.Abstractions.Persistence;
using Synonms.MultiTenancy.Faults;

namespace Synonms.MultiTenancy.Context;

public class MultiTenancyContextFactory : IMultiTenancyContextFactory
{
    private readonly ITenantRepository _repository;

    public MultiTenancyContextFactory(ITenantRepository repository)
    {
        _repository = repository;
    }
        
    public async Task<Result<MultiTenancyContext>> CreateAsync(Guid tenantId)
    {
        Maybe<Tenant> maybeTenant = await _repository.GetAsync(tenantId);

        return maybeTenant.Match(
            tenant =>
            {
                MultiTenancyContext multiTenancyContext = MultiTenancyContext.Create(tenant);

                return Result<MultiTenancyContext>.Success(multiTenancyContext);
            },
            () =>
            {
                TenantResolutionFault fault = new("Unable to find tenant Id '{0}'.", new FaultSource(nameof(tenantId), tenantId.ToString()), tenantId);
                
                return Result<MultiTenancyContext>.Failure(fault);
            });
    }
}