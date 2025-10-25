namespace Shared.Tenant;

public interface ITenantProvider
{
    Guid TenantId { get; }
    Guid? BranchId { get; }
}


