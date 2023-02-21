using Microsoft.Extensions.Options;
using Synonms.MultiTenancy.Abstractions.Persistence;
using Synonms.MultiTenancy.Configuration;

namespace Synonms.MultiTenancy.Persistence;

public class TenantsConnectionStringProvider : ITenantsConnectionStringProvider
{
    private readonly IOptions<MultiTenancyOptions> _options;

    public TenantsConnectionStringProvider(IOptions<MultiTenancyOptions> options)
    {
        _options = options;
    }

    public string Get() =>
        _options.Value.TenantDbConnectionString ?? string.Empty;
}