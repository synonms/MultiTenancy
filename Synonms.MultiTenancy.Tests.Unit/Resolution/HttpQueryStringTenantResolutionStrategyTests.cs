using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using Synonms.Functional;
using Synonms.MultiTenancy.Resolution;
using Xunit;

namespace Synonms.MultiTenancy.Tests.Unit.Resolution;

public class HttpQueryStringTenantResolutionStrategyTests
{
    private readonly Guid _tenantId = Guid.Parse("11111111-2222-3333-4444-555555555555");
        
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor = new();
        
    [Fact]
    public void GetId_HeaderExistsWithSingleValue_ReturnsCode()
    {
        DefaultHttpContext httpContext = new()
        {
            Request =
            {
                QueryString = new QueryString($"?{Constants.QueryStrings.TenantId}={_tenantId}")
            }
        };
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        HttpQueryStringTenantResolutionStrategy strategy = new(_mockHttpContextAccessor.Object);
            
        Maybe<Guid> result = strategy.GetId();

        result.Match(
            guid => Assert.Equal(_tenantId, guid),
            () => Assert.True(false, "Expected Some"));
    }
        
    [Fact]
    public void GetId_HeaderExistsWithMultipleValues_ReturnsNone()
    {
        StringValues stringValues = new(new[]
        {
            _tenantId.ToString(),
            Guid.NewGuid().ToString()
        });
            
        DefaultHttpContext httpContext = new()
        {
            Request =
            {
                QueryString = new QueryString($"?{Constants.QueryStrings.TenantId}={stringValues}")
            }
        };
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        HttpQueryStringTenantResolutionStrategy strategy = new(_mockHttpContextAccessor.Object);
            
        Maybe<Guid> result = strategy.GetId();

        Assert.True(result.IsNone);
    }

    [Fact]
    public void GetId_HeaderExistsWithInvalidCharacters_ReturnsNone()
    {
        DefaultHttpContext httpContext = new()
        {
            Request =
            {
                QueryString = new QueryString($"?{Constants.QueryStrings.TenantId}=some.invalid:%20code")
            }
        };
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        HttpQueryStringTenantResolutionStrategy strategy = new(_mockHttpContextAccessor.Object);
            
        Maybe<Guid> result = strategy.GetId();

        Assert.True(result.IsNone);
    }

    [Fact]
    public void GetId_HeaderDoesNotExist_ReturnsNone()
    {
        DefaultHttpContext httpContext = new();
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        HttpQueryStringTenantResolutionStrategy strategy = new(_mockHttpContextAccessor.Object);
            
        Maybe<Guid> result = strategy.GetId();

        Assert.True(result.IsNone);
    }
}