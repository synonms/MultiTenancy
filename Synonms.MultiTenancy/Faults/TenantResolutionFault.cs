using Synonms.Functional;
using Synonms.MultiTenancy.Abstractions.Faults;

namespace Synonms.MultiTenancy.Faults;

public class TenantResolutionFault : MultiTenancyFault
{
    public TenantResolutionFault(string detail, params object?[] arguments) 
        : this(detail, new FaultSource(), arguments)
    {
    }

    public TenantResolutionFault(string detail, FaultSource source, params object?[] arguments) 
        : base(nameof(TenantResolutionFault), detail, source, arguments)
    {
    }
}