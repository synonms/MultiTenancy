# MultiTenancy

*_Work In Progress_*

MultiTenancy is a lightweight library to help facilitate - you guessed it - Multi-Tenancy.

It resolves a unique tenant identifier (Guid) from an incoming HTTP request and passes it to a repository to load the related Tenant.  You implement `ITenantRepository` and resolve the Tenant object however you want.

A `MultiTenancyContext` is created via Middleware containing the resolved Tenant and this is accessible via the `IMultiTenancyContextAccessor`.  Simply inject this interface into your classes (much like you would with `IHttpContextAccessor`) and you can get the Tenant.

Requests where the Tenant can not be resolved are rejected with HTTP status code 400 (Bad Request).

Two identifier resolution strategies are available out of the box - via a 'X-Synonms-Tenant-ID' header or via a 'tenantId' query string parameter - but you can also provide your own.

*TODO*
 - Setup instructions/example
 - Make `Tenant` abstract and the various repos etc. generic with `<TTenant>` so clients can implement their own variations
