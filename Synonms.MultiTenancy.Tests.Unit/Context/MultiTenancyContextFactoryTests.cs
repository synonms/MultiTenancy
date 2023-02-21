using System;
using System.Threading.Tasks;
using Moq;
using Synonms.Functional;
using Synonms.MultiTenancy.Abstractions.Context;
using Synonms.MultiTenancy.Abstractions.Models;
using Synonms.MultiTenancy.Abstractions.Persistence;
using Synonms.MultiTenancy.Context;
using Synonms.MultiTenancy.Faults;
using Xunit;

namespace Synonms.MultiTenancy.Tests.Unit.Context;

public class MultiTenancyContextFactoryTests
{
    private static readonly Tenant Tenant = new()
    {
        Id = Guid.Parse("11111111-2222-3333-4444-555555555555"),
        Name = "Test Tenant"
    };

    private readonly Mock<ITenantRepository> _mockTenantRepository = new();

    public MultiTenancyContextFactoryTests()
    {
        _mockTenantRepository
            .Setup(x => x.GetAsync(Tenant.Id))
            .ReturnsAsync(Maybe<Tenant>.Some(Tenant));
    }
        
    [Fact]
    public async Task CreateAsync_ValidTenant_ThenReturnsSuccess()
    {
        MultiTenancyContextFactory factory = new(_mockTenantRepository.Object);

        Result<MultiTenancyContext> result = await factory.CreateAsync(Tenant.Id);

        result.Match(
            multiTenancyContext =>
            {
                Assert.Equal(Tenant.Id, multiTenancyContext.Tenant.Id);
                Assert.Equal(Tenant.Name, multiTenancyContext.Tenant.Name);
            }, 
            fault => Assert.True(false, "Expected Success"));
    }
        
    [Fact]
    public async Task CreateAsync_InvalidTenant_ThenReturnsTenantResolutionFault()
    {
        Guid invalidTenantId = Guid.NewGuid();

        _mockTenantRepository
            .Setup(x => x.GetAsync(invalidTenantId))
            .ReturnsAsync(Maybe<Tenant>.None);

        MultiTenancyContextFactory factory = new(_mockTenantRepository.Object);

        Result<MultiTenancyContext> result = await factory.CreateAsync(invalidTenantId);

        result.Match(
            _ => Assert.True(false, "Expected Failure"), 
            fault => Assert.IsType<TenantResolutionFault>(fault));
    }
}