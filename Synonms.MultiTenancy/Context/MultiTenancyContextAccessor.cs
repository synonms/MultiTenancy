using Synonms.MultiTenancy.Abstractions.Context;

namespace Synonms.MultiTenancy.Context;

public class MultiTenancyContextAccessor : IMultiTenancyContextAccessor
{
    public MultiTenancyContext? MultiTenancyContext { get; set; }
}