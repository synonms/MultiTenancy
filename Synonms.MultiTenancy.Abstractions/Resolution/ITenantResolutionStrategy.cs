using System;
using Synonms.Functional;

namespace Synonms.MultiTenancy.Abstractions.Resolution;

public interface ITenantResolutionStrategy
{
    Maybe<Guid> GetId();
}