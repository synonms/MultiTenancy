using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using Synonms.Functional;
using Synonms.MultiTenancy.Resolution;
using Xunit;

namespace Synonms.MultiTenancy.Tests.Unit.Resolution;

public class HttpHeaderTenantResolutionStrategyTests
{
    private readonly Guid _tenantId = Guid.Parse("11111111-2222-3333-4444-555555555555");

    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor = new();
        
    [Fact]
    public void GetId_HeaderExistsWithSingleValue_ReturnsId()
    {
        DefaultHttpContext httpContext = new();
        httpContext.Request.Headers.Add(Constants.Headers.TenantId, _tenantId.ToString());
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        HttpHeaderTenantResolutionStrategy strategy = new(_mockHttpContextAccessor.Object);
            
        Maybe<Guid> outcome = strategy.GetId();

        outcome.Match(
            guid => Assert.Equal(_tenantId, guid),
            () => Assert.True(false, "Expected Some"));
    }
        
    [Fact]
    public void GetId_HeaderExistsWithMultipleValues_ReturnsNone()
    {
        DefaultHttpContext httpContext = new();
        httpContext.Request.Headers.Add(Constants.Headers.TenantId, new StringValues(new[] 
        {
            _tenantId.ToString(),
            Guid.NewGuid().ToString()
        }));
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        HttpHeaderTenantResolutionStrategy strategy = new(_mockHttpContextAccessor.Object);
            
        Maybe<Guid> outcome = strategy.GetId();

        Assert.True(outcome.IsNone);
    }

    [Fact]
    public void GetId_HeaderExistsWithInvalidCharacters_ReturnsNone()
    {
        DefaultHttpContext httpContext = new();
        httpContext.Request.Headers.Add(Constants.Headers.TenantId, "some.invalid: code");
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        HttpHeaderTenantResolutionStrategy strategy = new(_mockHttpContextAccessor.Object);
            
        Maybe<Guid> outcome = strategy.GetId();

        Assert.True(outcome.IsNone);
    }

    [Fact]
    public void GetId_HeaderDoesNotExist_ReturnsNone()
    {
        DefaultHttpContext httpContext = new();
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        HttpHeaderTenantResolutionStrategy strategy = new(_mockHttpContextAccessor.Object);
            
        Maybe<Guid> outcome = strategy.GetId();

        Assert.True(outcome.IsNone);
    }
}