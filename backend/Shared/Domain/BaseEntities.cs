namespace Shared.Domain;

public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
}

public abstract class TenantEntity : Entity
{
    public Guid TenantId { get; set; }
    public Guid? BranchId { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedUtc { get; set; }
    public bool IsDeleted { get; set; }
}


