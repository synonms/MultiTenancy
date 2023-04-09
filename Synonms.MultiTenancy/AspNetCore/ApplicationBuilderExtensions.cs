using Microsoft.AspNetCore.Builder;

namespace Synonms.MultiTenancy.AspNetCore;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseMultiTenancy(this IApplicationBuilder builder) => 
        builder.UseMiddleware<MultiTenancyMiddleware>();
}