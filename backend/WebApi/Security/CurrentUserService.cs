using System.Security.Claims;
using Shared.Security;

namespace WebApi.Security;

public sealed class CurrentUserService : ICurrentUserService
{
    public Guid? UserId { get; }
    public Guid? TenantId { get; }
    public Guid? BranchId { get; }
    public CurrentUserService(IHttpContextAccessor accessor)
    {
        var id = accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                 ?? accessor.HttpContext?.User?.FindFirst("sub")?.Value;
        if (Guid.TryParse(id, out var parsed)) UserId = parsed;
        var tenant = accessor.HttpContext?.User?.FindFirst("tenant")?.Value
                     ?? accessor.HttpContext?.User?.FindFirst("tenant_id")?.Value;
        if(Guid.TryParse(tenant, out var t)) TenantId = t;
        var branch = accessor.HttpContext?.User?.FindFirst("branch")?.Value
                     ?? accessor.HttpContext?.User?.FindFirst("branch_id")?.Value;
        if(Guid.TryParse(branch, out var b)) BranchId = b;
    }
}






