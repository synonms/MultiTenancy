using Microsoft.Extensions.Options;
using Synonms.MultiTenancy.Configuration;
using Synonms.MultiTenancy.Persistence;
using Xunit;

namespace Synonms.MultiTenancy.Tests.Unit.Persistence
{
    public class TenantConnectionStringProviderTests
    {
        // TODO: Test decryption
        
        [Fact]
        public void Get_ValidConfig_ReturnsConnectionString()
        {
            MultiTenancyOptions multiTenancyOptions = new()
            {
                TenantDbConnectionString = "connection"
            };

            TenantsConnectionStringProvider tenantsConnectionStringProvider = new(Options.Create(multiTenancyOptions));

            string actualResult = tenantsConnectionStringProvider.Get();
            
            Assert.Equal(multiTenancyOptions.TenantDbConnectionString, actualResult);
        }
    }
}