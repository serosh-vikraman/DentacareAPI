using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? FullName { get; set; }
    public Guid TenantId { get; set; }
    public Guid? BranchId { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Specialty { get; set; }
    public string? Designation { get; set; }
}






