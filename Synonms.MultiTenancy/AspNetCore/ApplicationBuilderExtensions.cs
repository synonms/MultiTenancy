using Microsoft.AspNetCore.Builder;

namespace Synonms.MultiTenancy.AspNetCore;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSynonmsMultiTenancy(this IApplicationBuilder builder) => 
        builder.UseMiddleware<MultiTenancyMiddleware>();
}