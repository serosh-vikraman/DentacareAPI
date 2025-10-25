using Shared.Domain;

namespace Domain.Patients;

public class PatientProfile : TenantEntity
{
    public string MRNumber { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string? BloodGroup { get; set; }
    public string? Gender { get; set; }
    public string? PatientType { get; set; }
    public string? MaritalStatus { get; set; }

    public string? Mobile { get; set; }
    public string? AltPhone { get; set; }
    public string? Email { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public string? PhotoUrl { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }

    // Vitals (snapshot)
    public string? BP { get; set; }
    public int? Pulse { get; set; }
    public int? RespiratoryRate { get; set; }
    public double? TemperatureF { get; set; }
    public int? SpO2 { get; set; }
    public double? BloodSugar { get; set; }

    // Dental/Oral
    public int? PainLevel { get; set; }
    public string? Hygiene { get; set; }
    public string? Gingival { get; set; }
    public string? Pockets { get; set; }
    public string? Mobility { get; set; }
    public string? Caries { get; set; }
    public string? MucosalNotes { get; set; }
    public string? TMJNotes { get; set; }
    public string? OcclusionNotes { get; set; }
    public string? IntraExtraNotes { get; set; }
    public string? ContinuousSpO2 { get; set; }
    public string? HeartRateECG { get; set; }
    public string? SalivaPHFlow { get; set; }

    // Medical history
    public bool OnTreatment { get; set; }
    public string? AllergicMedicines { get; set; }
    public bool Diabetes { get; set; }
    public string? Cardiac { get; set; }
    public string? Neuro { get; set; }
    public string? Pregnancy { get; set; }
    public string? OtherConditions { get; set; }

    // Dental history flags
    public bool HxSyncope { get; set; }
    public bool HxAllergyLA { get; set; }
    public bool HxEndo { get; set; }
    public bool HxOrtho { get; set; }
    public bool HxPerio { get; set; }
    public bool HxSurgical { get; set; }
    public bool HxExtraction { get; set; }
    public bool HxRPD { get; set; }
    public bool HxFPD { get; set; }
    public string? HxOther { get; set; }

    // Complaints
    public string? ChiefComplaint { get; set; }
    public string? OE { get; set; }
    public string? EO { get; set; }
    public string? IO { get; set; }
}


