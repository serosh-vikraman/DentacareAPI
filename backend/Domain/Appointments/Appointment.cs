using Shared.Domain;

namespace Domain.Appointments;

public class Appointment : TenantEntity
{
    public string PatientName { get; set; } = string.Empty;
    public string? PatientMRNumber { get; set; }
    public Guid? PatientProfileId { get; set; }
    public string Department { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public Guid? DoctorProfileId { get; set; }
    public string ConsultMode { get; set; } = "In-person";
    public string? PaymentMode { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "Upcoming";
	// Diagnosis / Investigation
	public string? InvestigationRvg { get; set; } // IOPA/RVG notes
	public bool? InvestigationOpg { get; set; }
	public bool? InvestigationCeph { get; set; }
	public bool? InvestigationOcclusal { get; set; }
	public bool? InvestigationCbct { get; set; }
	public string? InvestigationBlood { get; set; }
	public string? InvestigationOthers { get; set; }
	public string? DifferentialDiagnosis { get; set; } // DD
	public string? Diagnosis { get; set; }
	public string? TreatmentPlan { get; set; }
}





