using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Synonms.Functional;
using Synonms.MultiTenancy.Abstractions.Models;

namespace Synonms.MultiTenancy.Abstractions.Persistence;

public interface ITenantRepository
{
    Task<IEnumerable<Tenant>> GetAllAsync();
        
    Task<Maybe<Tenant>> GetAsync(Guid id);
}