using System;
using System.Threading.Tasks;
using Synonms.Functional;

namespace Synonms.MultiTenancy.Abstractions.Context;

public interface IMultiTenancyContextFactory
{
    Task<Result<MultiTenancyContext>> CreateAsync(Guid tenantId);
}