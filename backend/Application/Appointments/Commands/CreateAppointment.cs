using Application.Abstractions;
using Application.Appointments.Dtos;
using Domain.Appointments;
using MediatR;
using Shared.Tenant;

namespace Application.Appointments.Commands;

public sealed record CreateAppointmentCommand(CreateAppointmentRequest Request) : IRequest<Guid>;

public sealed class CreateAppointmentHandler : IRequestHandler<CreateAppointmentCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    private readonly ITenantProvider _tenantProvider;

    public CreateAppointmentHandler(IApplicationDbContext db, ITenantProvider tenantProvider)
    {
        _db = db;
        _tenantProvider = tenantProvider;
    }

    public async Task<Guid> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;
        var entity = new Appointment
        {
            TenantId = _tenantProvider.TenantId,
            BranchId = _tenantProvider.BranchId,
            PatientName = r.PatientName.Trim(),
            PatientMRNumber = string.IsNullOrWhiteSpace(r.PatientMRNumber) ? null : r.PatientMRNumber.Trim(),
            PatientProfileId = r.PatientProfileId,
            Department = r.Department.Trim(),
            DoctorName = r.DoctorName.Trim(),
            DoctorProfileId = r.DoctorProfileId,
            ConsultMode = string.IsNullOrWhiteSpace(r.ConsultMode) ? "In-person" : r.ConsultMode.Trim(),
            PaymentMode = string.IsNullOrWhiteSpace(r.PaymentMode) ? null : r.PaymentMode.Trim(),
            Date = r.Date,
            StartTime = r.StartTime,
            EndTime = r.EndTime,
            Reason = string.IsNullOrWhiteSpace(r.Reason) ? null : r.Reason.Trim(),
            Notes = string.IsNullOrWhiteSpace(r.Notes) ? null : r.Notes.Trim(),
            Status = "Upcoming"
        };
        _db.Appointments.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}







