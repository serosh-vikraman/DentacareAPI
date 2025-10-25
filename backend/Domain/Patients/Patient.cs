using Shared.Domain;

namespace Domain.Patients;

public class Patient : TenantEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public DateOnly? DateOfBirth { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }

    private Patient() { }

    public Patient(Guid tenantId, Guid? branchId, string firstName, string lastName)
    {
        TenantId = tenantId;
        BranchId = branchId;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    public void UpdateBasicInfo(string firstName, string lastName)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        UpdatedUtc = DateTime.UtcNow;
    }

    public void UpdateContact(string? email, string? phone)
    {
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        UpdatedUtc = DateTime.UtcNow;
    }
}


