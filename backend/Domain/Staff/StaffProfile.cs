using Shared.Domain;

namespace Domain.Staff;

public sealed class StaffProfile : TenantEntity
{
    public Guid UserId { get; set; }
    public string StaffId { get; set; } = string.Empty; // 6-digit sequence per tenant

    public string FullName { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public string? ContactNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Role { get; set; } // FrontDeskAdmin, NursingAssistant, Receptionist, etc.
    public string? Designation { get; set; }
}


