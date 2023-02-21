using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Synonms.Functional;
using Synonms.MultiTenancy.Abstractions.Context;
using Synonms.MultiTenancy.Abstractions.Resolution;
using Synonms.MultiTenancy.Faults;

namespace Synonms.MultiTenancy.AspNetCore;

public class MultiTenancyMiddleware : IMiddleware
{
    private readonly ILogger<MultiTenancyMiddleware> _logger;
    private readonly ITenantResolver _tenantResolver;
    private readonly IMultiTenancyContextFactory _multiTenancyContextFactory;
    private readonly IMultiTenancyContextAccessor _multiTenancyContextAccessor;

    public MultiTenancyMiddleware(ILogger<MultiTenancyMiddleware> logger, ITenantResolver tenantResolver, IMultiTenancyContextFactory multiTenancyContextFactory, IMultiTenancyContextAccessor multiTenancyContextAccessor)
    {
        _logger = logger;
        _tenantResolver = tenantResolver;
        _multiTenancyContextFactory = multiTenancyContextFactory;
        _multiTenancyContextAccessor = multiTenancyContextAccessor;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (_multiTenancyContextAccessor.MultiTenancyContext is null)
        {
            Maybe<Guid> tenantIdResolutionOutcome = await _tenantResolver.ResolveIdAsync();
               
            Maybe<Fault> maybeFault = await tenantIdResolutionOutcome.Match(
                async tenantCode =>
                {
                    Result<MultiTenancyContext> maybeTenant = await _multiTenancyContextFactory.CreateAsync(tenantCode);

                    return maybeTenant.Bind(
                        multiTenancyContext =>
                        {
                            _multiTenancyContextAccessor.MultiTenancyContext = multiTenancyContext;
                            return Maybe<Fault>.None;
                        });
                },
                () => Maybe<Fault>.SomeAsync(new TenantResolutionFault("Unable to resolve tenant Id.")));

            if (maybeFault.IsSome)
            {
                maybeFault.IfSome(fault => _logger.LogError("MultiTenancy fault: {0}", fault));
                    
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return;
            }
        }

        await next(context);
    }
}