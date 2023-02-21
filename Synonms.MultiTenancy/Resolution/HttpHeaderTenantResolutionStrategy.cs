using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Synonms.Functional;
using Synonms.MultiTenancy.Abstractions.Resolution;

namespace Synonms.MultiTenancy.Resolution;

public class HttpHeaderTenantResolutionStrategy : ITenantResolutionStrategy
{
    private static readonly Func<KeyValuePair<string, StringValues>, bool> HeaderPredicate =
        header => header.Key.Equals(Constants.Headers.TenantId, StringComparison.OrdinalIgnoreCase);

    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpHeaderTenantResolutionStrategy(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
        
    public Maybe<Guid> GetId()
    {
        if (_httpContextAccessor?.HttpContext is null)
        {
            return Maybe<Guid>.None;
        }

        if (CountApplicableHeaders(_httpContextAccessor.HttpContext.Request) != 1)
        {
            return Maybe<Guid>.None;
        }
            
        KeyValuePair<string, StringValues> tenantIdHeader = _httpContextAccessor.HttpContext.Request.Headers.Single(HeaderPredicate);

        if (tenantIdHeader.Value.Count != 1)
        {
            return Maybe<Guid>.None;
        }

        string id = tenantIdHeader.Value.Single();

        return Guid.TryParse(id, out Guid guid) 
            ? guid 
            : Maybe<Guid>.None;
    }
        
    private static int CountApplicableHeaders(HttpRequest httpRequest) =>
        httpRequest.Headers.Count(HeaderPredicate);
}