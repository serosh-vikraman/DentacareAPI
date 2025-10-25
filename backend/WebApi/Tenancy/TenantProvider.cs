using Shared.Tenant;

namespace WebApi.Tenancy;

public sealed class TenantProvider : ITenantProvider
{
    public Guid TenantId { get; }
    public Guid? BranchId { get; }

    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        // For now single-tenant: use a fixed TenantId until auth is wired.
        // Later, resolve from JWT claims or headers.
        TenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        var httpContext = httpContextAccessor.HttpContext;
        var branchHeader = httpContext?.Request.Headers["X-Branch-Id"].FirstOrDefault();
        if (Guid.TryParse(branchHeader, out var branchId))
        {
            BranchId = branchId;
        }
    }
}


