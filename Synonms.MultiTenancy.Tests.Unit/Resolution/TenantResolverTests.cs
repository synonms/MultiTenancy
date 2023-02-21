using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Synonms.Functional;
using Synonms.MultiTenancy.Abstractions.Resolution;
using Synonms.MultiTenancy.Resolution;
using Xunit;

namespace Synonms.MultiTenancy.Tests.Unit.Resolution;

public class TenantResolverTests
{
    private readonly Guid _tenantId = Guid.Parse("11111111-2222-3333-4444-555555555555");

    private readonly Mock<ITenantResolutionStrategy> _strategy1 = new();
    private readonly Mock<ITenantResolutionStrategy> _strategy2 = new();
    private readonly Mock<ITenantResolutionStrategy> _strategy3 = new();

    [Fact]
    public async Task ResolveIdAsync_AllSuccessfulStrategies_ReturnsFirstCode()
    {
        _strategy1.Setup(x => x.GetId()).Returns(Maybe<Guid>.Some(_tenantId));
        _strategy2.Setup(x => x.GetId()).Returns(Maybe<Guid>.Some(Guid.NewGuid()));
        _strategy3.Setup(x => x.GetId()).Returns(Maybe<Guid>.Some(Guid.NewGuid()));
            
        List<ITenantResolutionStrategy> strategies = new()
        {
            _strategy1.Object,
            _strategy2.Object,
            _strategy3.Object
        };
            
        TenantResolver resolver = new(strategies);

        Maybe<Guid> result = await resolver.ResolveIdAsync();

        result.Match(
            guid => Assert.Equal(_tenantId, guid),
            () => Assert.True(false, "Expected Some")
        );
    }
        
    [Fact]
    public async Task ResolveIdAsync_SomeSuccessfulStrategies_ReturnsFirstCode()
    {
        _strategy1.Setup(x => x.GetId()).Returns(Maybe<Guid>.None);
        _strategy2.Setup(x => x.GetId()).Returns(Maybe<Guid>.Some(_tenantId));
        _strategy3.Setup(x => x.GetId()).Returns(Maybe<Guid>.Some(Guid.NewGuid()));
            
        List<ITenantResolutionStrategy> strategies = new()
        {
            _strategy1.Object,
            _strategy2.Object,
            _strategy3.Object
        };
            
        TenantResolver resolver = new(strategies);

        Maybe<Guid> result = await resolver.ResolveIdAsync();

        result.Match(
            tenantCode => Assert.Equal(_tenantId, tenantCode),
            () => Assert.True(false, "Expected Some")
        );
    }
        
    [Fact]
    public async Task ResolveIdAsync_NoSuccessfulStrategies_ReturnsNone()
    {
        _strategy1.Setup(x => x.GetId()).Returns(Maybe<Guid>.None);
        _strategy2.Setup(x => x.GetId()).Returns(Maybe<Guid>.None);
        _strategy3.Setup(x => x.GetId()).Returns(Maybe<Guid>.None);
            
        List<ITenantResolutionStrategy> strategies = new()
        {
            _strategy1.Object,
            _strategy2.Object,
            _strategy3.Object
        };
            
        TenantResolver resolver = new(strategies);

        Maybe<Guid> result = await resolver.ResolveIdAsync();

        Assert.True(result.IsNone);
    }
}