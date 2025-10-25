using Shared.Domain;

namespace Domain.Doctors;

public sealed class DoctorProfile : TenantEntity
{
    public Guid UserId { get; set; }
    public string DoctorId { get; set; } = string.Empty; // 6-digit sequence per tenant

    // Personal Information
    public string FullName { get; set; } = string.Empty;
    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? PhotoUrl { get; set; }
    public string? ContactNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactRelation { get; set; }
    public string? EmergencyContactPhone { get; set; }

    // Professional Details
    public string? Specialization { get; set; }
    public string? Qualifications { get; set; }
    public string? MedicalRegistrationNumber { get; set; }
    public int? YearsOfExperience { get; set; }
}


