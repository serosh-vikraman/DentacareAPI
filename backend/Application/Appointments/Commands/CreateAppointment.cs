using Application.Abstractions;
using Application.Appointments.Dtos;
using Domain.Appointments;
using Domain.Patients;
using MediatR;
using Microsoft.EntityFrameworkCore;
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

		// Ensure we have a PatientProfile and MR number when creating an appointment
		Guid? patientProfileId = r.PatientProfileId;
		string? patientMrNumber = string.IsNullOrWhiteSpace(r.PatientMRNumber) ? null : r.PatientMRNumber.Trim();

		if (patientProfileId == null && string.IsNullOrWhiteSpace(patientMrNumber) && !string.IsNullOrWhiteSpace(r.PatientName))
		{
			// Create a minimal PatientProfile so the patient appears in Patients screen
			var newProfile = new PatientProfile
			{
				TenantId = _tenantProvider.TenantId,
				BranchId = _tenantProvider.BranchId,
				PatientName = r.PatientName.Trim(),
				MRNumber = await GenerateNextMrNumberAsync(_db, _tenantProvider.TenantId, cancellationToken)
			};
			_db.PatientProfiles.Add(newProfile);
			await _db.SaveChangesAsync(cancellationToken);
			patientProfileId = newProfile.Id;
			patientMrNumber = newProfile.MRNumber;
		}
		else if (patientProfileId != null && string.IsNullOrWhiteSpace(patientMrNumber))
		{
			// If a profile is provided, derive MR number for consistency
			patientMrNumber = await _db.PatientProfiles
				.Where(p => p.Id == patientProfileId.Value)
				.Select(p => p.MRNumber)
				.FirstOrDefaultAsync(cancellationToken);
		}
		else if (patientProfileId == null && !string.IsNullOrWhiteSpace(patientMrNumber))
		{
			// MR provided but profile id missing - try to resolve
			patientProfileId = await _db.PatientProfiles
				.Where(p => p.MRNumber == patientMrNumber && p.TenantId == _tenantProvider.TenantId && !p.IsDeleted)
				.Select(p => (Guid?)p.Id)
				.FirstOrDefaultAsync(cancellationToken);
		}

		var entity = new Appointment
        {
            TenantId = _tenantProvider.TenantId,
            BranchId = _tenantProvider.BranchId,
            PatientName = r.PatientName.Trim(),
			PatientMRNumber = patientMrNumber,
			PatientProfileId = patientProfileId,
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
            Status = DetermineInitialStatus(r.Date, r.StartTime)
        };
        _db.Appointments.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

	private static async Task<string> GenerateNextMrNumberAsync(IApplicationDbContext db, Guid tenantId, CancellationToken ct)
	{
		// Tenant-scoped naive sequence; consider a dedicated sequence table for concurrency
		var count = await db.PatientProfiles.Where(p => p.TenantId == tenantId).CountAsync(ct);
		var next = count + 1;
		return next.ToString("D6");
	}

    private static string DetermineInitialStatus(DateOnly date, TimeOnly startTime)
    {
        var appointmentDateTime = date.ToDateTime(startTime);
        var now = DateTime.Now;
        return appointmentDateTime > now ? "Scheduled" : "Missed";
    }
}







