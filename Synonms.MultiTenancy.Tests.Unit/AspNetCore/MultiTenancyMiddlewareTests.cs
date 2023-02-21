using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Synonms.Functional;
using Synonms.MultiTenancy.Abstractions.Context;
using Synonms.MultiTenancy.Abstractions.Models;
using Synonms.MultiTenancy.Abstractions.Resolution;
using Synonms.MultiTenancy.AspNetCore;
using Synonms.MultiTenancy.Faults;
using Xunit;

namespace Synonms.MultiTenancy.Tests.Unit.AspNetCore;

public class MultiTenancyMiddlewareTests
{
    private static readonly Tenant Tenant = new()
    {
        Id = Guid.Parse("11111111-2222-3333-4444-555555555555"),
        Name = "Test Tenant"
    };

    private readonly Mock<ILogger<MultiTenancyMiddleware>> _mockLogger = new(); 
    private readonly Mock<ITenantResolver> _mockTenantResolver = new();
    private readonly Mock<IMultiTenancyContextFactory> _mockMultiTenancyContextFactory = new();
    private readonly Mock<IMultiTenancyContextAccessor> _mockTenantEnvironmentContextAccessor = new();

    private readonly MultiTenancyContext _multiTenancyContext = MultiTenancyContext.Create(Tenant);

    public MultiTenancyMiddlewareTests()
    {
        _mockTenantResolver
            .Setup(x => x.ResolveIdAsync())
            .ReturnsAsync(Maybe<Guid>.Some(Tenant.Id));
            
        _mockMultiTenancyContextFactory
            .Setup(x => x.CreateAsync(Tenant.Id))
            .ReturnsAsync(Result<MultiTenancyContext>.Success(_multiTenancyContext));
    }
        
    [Fact]
    public async Task InvokeAsync_MultiTenancyContextExists_PerformsNoActionAndCallsNext()
    {
        _mockTenantEnvironmentContextAccessor
            .Setup(x => x.MultiTenancyContext)
            .Returns(_multiTenancyContext);

        MultiTenancyMiddleware multiTenancyMiddleware = new(_mockLogger.Object, _mockTenantResolver.Object, _mockMultiTenancyContextFactory.Object, _mockTenantEnvironmentContextAccessor.Object);

        bool isNextCalled = false;

        DefaultHttpContext httpContext = new();
            
        await multiTenancyMiddleware.InvokeAsync(httpContext, _ =>
        {
            isNextCalled = true;
            return Task.CompletedTask;
        });
            
        Assert.True(isNextCalled);
            
        _mockTenantResolver.VerifyNoOtherCalls();
        _mockMultiTenancyContextFactory.VerifyNoOtherCalls();
        _mockTenantEnvironmentContextAccessor.VerifySet(x => x.MultiTenancyContext = _multiTenancyContext, Times.Never);
    }
        
    [Fact]
    public async Task InvokeAsync_MultiTenancyContextIsNullAndResolutionSucceeds_CallsNext()
    {
        _mockTenantEnvironmentContextAccessor
            .SetupSequence(x => x.MultiTenancyContext)
            .Returns(null as MultiTenancyContext)
            .Returns(_multiTenancyContext)
            .Returns(_multiTenancyContext)
            .Returns(_multiTenancyContext)
            .Returns(_multiTenancyContext);

        MultiTenancyMiddleware multiTenancyMiddleware = new(_mockLogger.Object, _mockTenantResolver.Object, _mockMultiTenancyContextFactory.Object, _mockTenantEnvironmentContextAccessor.Object);

        bool isNextCalled = false;
            
        DefaultHttpContext httpContext = new();

        await multiTenancyMiddleware.InvokeAsync(httpContext, _ =>
        {
            isNextCalled = true;
            return Task.CompletedTask;
        });
            
        Assert.True(isNextCalled);

        _mockTenantEnvironmentContextAccessor.VerifySet(x => x.MultiTenancyContext = _multiTenancyContext, Times.Once);
    }
        
    [Fact]
    public async Task InvokeAsync_MultiTenancyContextIsNullAndTenantResolutionFails_SetsBadRequestAndDoesNotCallNext()
    {
        _mockTenantEnvironmentContextAccessor
            .Setup(x => x.MultiTenancyContext)
            .Returns(null as MultiTenancyContext);

        _mockTenantResolver
            .Setup(x => x.ResolveIdAsync())
            .ReturnsAsync(Maybe<Guid>.None);

        MultiTenancyMiddleware multiTenancyMiddleware = new(_mockLogger.Object, _mockTenantResolver.Object, _mockMultiTenancyContextFactory.Object, _mockTenantEnvironmentContextAccessor.Object);

        bool isNextCalled = false;
        DefaultHttpContext httpContext = new();
            
        await multiTenancyMiddleware.InvokeAsync(httpContext, _ =>
        {
            isNextCalled = true;
            return Task.CompletedTask;
        });
            
        Assert.False(isNextCalled);
        Assert.Equal((int)HttpStatusCode.BadRequest, httpContext.Response.StatusCode);
    }
        
    [Fact]
    public async Task InvokeAsync_MultiTenancyContextIsNullAndContextCreationFails_SetsBadRequestAndDoesNotCallNext()
    {
        _mockTenantEnvironmentContextAccessor
            .Setup(x => x.MultiTenancyContext)
            .Returns(null as MultiTenancyContext);

        _mockMultiTenancyContextFactory
            .Setup(x => x.CreateAsync(Tenant.Id))
            .ReturnsAsync(Result<MultiTenancyContext>.Failure(new TenantResolutionFault("test")));

        MultiTenancyMiddleware multiTenancyMiddleware = new(_mockLogger.Object, _mockTenantResolver.Object, _mockMultiTenancyContextFactory.Object, _mockTenantEnvironmentContextAccessor.Object);

        bool isNextCalled = false;
        DefaultHttpContext httpContext = new();
            
        await multiTenancyMiddleware.InvokeAsync(httpContext, _ =>
        {
            isNextCalled = true;
            return Task.CompletedTask;
        });
            
        Assert.False(isNextCalled);
        Assert.Equal((int)HttpStatusCode.BadRequest, httpContext.Response.StatusCode);
    }
}