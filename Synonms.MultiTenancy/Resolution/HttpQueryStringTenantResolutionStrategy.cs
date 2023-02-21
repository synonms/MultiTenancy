using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Synonms.Functional;
using Synonms.MultiTenancy.Abstractions.Resolution;

namespace Synonms.MultiTenancy.Resolution;

public class HttpQueryStringTenantResolutionStrategy : ITenantResolutionStrategy
{
    private static readonly Func<KeyValuePair<string, StringValues>, bool> QueryPredicate =
        query => query.Key.Equals(Constants.QueryStrings.TenantId, StringComparison.OrdinalIgnoreCase);

    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpQueryStringTenantResolutionStrategy(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
        
    public Maybe<Guid> GetId()
    {
        if (_httpContextAccessor?.HttpContext is null)
        {
            return Maybe<Guid>.None;
        }
            
        if (CountApplicableQueries(_httpContextAccessor.HttpContext.Request) != 1)
        {
            return Maybe<Guid>.None;
        }

        KeyValuePair<string, StringValues> tenantQueryStrings = _httpContextAccessor.HttpContext.Request.Query.Single(QueryPredicate);

        if (tenantQueryStrings.Value.Count != 1)
        {
            return Maybe<Guid>.None;
        }

        string id = tenantQueryStrings.Value.Single();

        return Guid.TryParse(id, out Guid guid) 
            ? guid 
            : Maybe<Guid>.None;
    }
        
    private static int CountApplicableQueries(HttpRequest httpRequest) =>
        httpRequest.Query.Count(QueryPredicate);
}