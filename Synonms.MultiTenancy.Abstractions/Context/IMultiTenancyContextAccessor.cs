namespace Synonms.MultiTenancy.Abstractions.Context;

public interface IMultiTenancyContextAccessor
{
    MultiTenancyContext? MultiTenancyContext { get; set; } 
}