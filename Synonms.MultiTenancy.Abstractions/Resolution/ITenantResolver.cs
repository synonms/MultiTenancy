using System;
using System.Threading.Tasks;
using Synonms.Functional;

namespace Synonms.MultiTenancy.Abstractions.Resolution;

public interface ITenantResolver
{
    Task<Maybe<Guid>> ResolveIdAsync();
}