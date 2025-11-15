namespace WebApi.Users;

public sealed class DoctorUpdateRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string? Specialty { get; set; }
    public string? Gender { get; set; }
    public string? Dob { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Address { get; set; }
    public string? EmergencyName { get; set; }
    public string? EmergencyRelation { get; set; }
    public string? EmergencyPhone { get; set; }
    public string? Qualifications { get; set; }
    public string? Regno { get; set; }
    public int? Experience { get; set; }
}


