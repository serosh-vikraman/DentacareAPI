using Shared.Domain;

namespace Domain.Services;

public class Service : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Category { get; set; }
}




