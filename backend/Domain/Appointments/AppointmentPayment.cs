using Shared.Domain;

namespace Domain.Appointments;

public class AppointmentPayment : TenantEntity
{
    public Guid AppointmentId { get; set; }
    public string Mode { get; set; } = string.Empty; // Cash, Card, UPI, Insurance
    public string? ReferenceNumber { get; set; }
    public decimal TotalAmount { get; set; }
    public ICollection<AppointmentPaymentItem> Items { get; set; } = new List<AppointmentPaymentItem>();
}

public class AppointmentPaymentItem : Entity
{
    public Guid AppointmentPaymentId { get; set; }
    public Guid? ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}



