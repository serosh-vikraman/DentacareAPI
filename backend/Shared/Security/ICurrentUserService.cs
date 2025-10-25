namespace Shared.Security;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? TenantId { get; }
    Guid? BranchId { get; }
}






