namespace Application.Patients.Dtos;

public sealed class PatientDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
}

public sealed class CreatePatientRequest
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
}

public sealed class UpdatePatientRequest
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
}

public sealed class SavePatientProfileRequest
{
    public Guid? Id { get; set; }
    public string PatientName { get; init; } = string.Empty;
    public string? Dob { get; init; }
    public string? BloodGroup { get; init; }
    public string? Gender { get; init; }
    public string? PatientType { get; init; }
    public string? MaritalStatus { get; init; }
    public string? Mobile { get; init; }
    public string? AltPhone { get; init; }
    public string? Email { get; init; }
    public string? Address1 { get; init; }
    public string? Address2 { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? Zip { get; init; }
    public string? PhotoUrl { get; init; }
    public string? EmergencyContactName { get; init; }
    public string? EmergencyContactPhone { get; init; }
    public string? ChiefComplaint { get; init; }
    public string? OE { get; init; }
    public string? EO { get; init; }
    public string? IO { get; init; }
    // vitals - general
    public string? BP { get; init; }
    public int? Pulse { get; init; }
    public int? RespiratoryRate { get; init; }
    public double? TemperatureF { get; init; }
    public int? SpO2 { get; init; }
    public double? BloodSugar { get; init; }
    // dental vitals
    public int? PainLevel { get; init; }
    public string? Hygiene { get; init; }
    public string? Gingival { get; init; }
    public string? Pockets { get; init; }
    public string? Mobility { get; init; }
    public string? Caries { get; init; }
    public string? MucosalNotes { get; init; }
    public string? TMJNotes { get; init; }
    public string? OcclusionNotes { get; init; }
    public string? IntraExtraNotes { get; init; }
    public string? ContinuousSpO2 { get; init; }
    public string? HeartRateECG { get; init; }
    public string? SalivaPHFlow { get; init; }
    // medical history
    public string? OnTreatment { get; init; }
    public string? AllergicMedicines { get; init; }
    public string? Diabetes { get; init; }
    public string? Cardiac { get; init; }
    public string? Neuro { get; init; }
    public string? Pregnancy { get; init; }
    public string? OtherConditions { get; init; }
    public bool DentalHxSyncope { get; init; }
    public bool DentalHxAllergyLA { get; init; }
    public bool DentalHxEndo { get; init; }
    public bool DentalHxOrtho { get; init; }
    public bool DentalHxPerio { get; init; }
    public bool DentalHxSurgical { get; init; }
    public bool DentalHxExtraction { get; init; }
    public bool DentalHxRPD { get; init; }
    public bool DentalHxFPD { get; init; }
    public string? DentalHxOther { get; init; }
}


