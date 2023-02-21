using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Synonms.Functional;
using Synonms.Functional.Extensions;
using Synonms.MultiTenancy.Abstractions.Resolution;

namespace Synonms.MultiTenancy.Resolution;

public class TenantResolver : ITenantResolver
{
    private readonly IEnumerable<ITenantResolutionStrategy> _resolutionStrategies;

    public TenantResolver(IEnumerable<ITenantResolutionStrategy> resolutionStrategies)
    {
        _resolutionStrategies = resolutionStrategies;
    }
        
    public Task<Maybe<Guid>> ResolveIdAsync()
    {
        Maybe<Guid> resolutionOutcome = _resolutionStrategies.Coalesce(strategy => strategy.GetId(), Maybe<Guid>.None);
        
        return Task.FromResult(resolutionOutcome);
    }
}