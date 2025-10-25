namespace Application.Appointments.Dtos;

public sealed class AppointmentListItem
{
    public Guid Id { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string? PatientMRNumber { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Age { get; set; }
}

public sealed class CreateAppointmentRequest
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
}

public sealed class UpdateAppointmentRequest
{
    public string? Status { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public string? InvestigationRvg { get; set; }
    public bool? InvestigationOpg { get; set; }
    public bool? InvestigationCeph { get; set; }
    public bool? InvestigationOcclusal { get; set; }
    public bool? InvestigationCbct { get; set; }
    public string? InvestigationBlood { get; set; }
    public string? InvestigationOthers { get; set; }
    public string? DifferentialDiagnosis { get; set; }
    public string? Diagnosis { get; set; }
    public string? TreatmentPlan { get; set; }
}

public sealed class SavePaymentRequest
{
    public Guid AppointmentId { get; set; }
    public string Mode { get; set; } = string.Empty;
    public string? ReferenceNumber { get; set; }
    public decimal TotalAmount { get; set; }
    public List<SavePaymentItem> Items { get; set; } = new();
}
public sealed class SavePaymentItem
{
    public Guid? ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public sealed class AppointmentDetailDto
{
    public Guid Id { get; set; }
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
    public string Status { get; set; } = string.Empty;
    public string? InvestigationRvg { get; set; }
    public bool? InvestigationOpg { get; set; }
    public bool? InvestigationCeph { get; set; }
    public bool? InvestigationOcclusal { get; set; }
    public bool? InvestigationCbct { get; set; }
    public string? InvestigationBlood { get; set; }
    public string? InvestigationOthers { get; set; }
    public string? DifferentialDiagnosis { get; set; }
    public string? Diagnosis { get; set; }
    public string? TreatmentPlan { get; set; }
}





