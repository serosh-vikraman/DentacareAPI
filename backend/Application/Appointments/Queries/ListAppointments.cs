using Application.Abstractions;
using Application.Appointments.Dtos;
using Domain.Appointments;
using Domain.Doctors;
using Shared.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Appointments.Queries;

public sealed record ListAppointmentsQuery(string? Q) : IRequest<IReadOnlyList<AppointmentListItem>>;
public sealed record GetAppointmentQuery(Guid Id) : IRequest<AppointmentDetailDto?>;

public sealed class ListAppointmentsHandler : IRequestHandler<ListAppointmentsQuery, IReadOnlyList<AppointmentListItem>>
{
    private readonly IApplicationDbContext _db;
    private readonly IEncryptionService _enc;

    public ListAppointmentsHandler(IApplicationDbContext db, IEncryptionService enc)
    {
        _db = db;
        _enc = enc;
    }

    public async Task<IReadOnlyList<AppointmentListItem>> Handle(ListAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var q = request.Q?.Trim().ToLowerInvariant();
        var query = _db.Appointments.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(a => a.PatientName.ToLower().Contains(q) || a.DoctorName.ToLower().Contains(q) || (a.PatientMRNumber != null && a.PatientMRNumber.ToLower().Contains(q)));
        }

        var raw = await query
            .OrderByDescending(a => a.Date).ThenByDescending(a => a.StartTime)
            .Select(a => new {
                a.Id,
                a.PatientName,
                a.PatientMRNumber,
                a.PatientProfileId,
                a.DoctorName,
                a.Department,
                a.Date,
                a.StartTime,
                a.Status,
                a.Diagnosis
            })
            .ToListAsync(cancellationToken);

        // Optional: enrich with age by looking up PatientProfiles in-memory
        var profileIds = raw.Where(a => a.PatientProfileId.HasValue).Select(a => a.PatientProfileId!.Value).Distinct().ToList();
        var profiles = await _db.PatientProfiles.AsNoTracking()
            .Where(p => profileIds.Contains(p.Id))
            .Select(p => new { p.Id, p.DateOfBirth })
            .ToListAsync(cancellationToken);

        string? AgeFromDob(DateOnly? dob)
        {
            if (dob == null) return null;
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            int years = today.Year - dob.Value.Year;
            int months = today.Month - dob.Value.Month;
            if (today.Day < dob.Value.Day) months -= 1;
            if (months < 0) { months += 12; years -= 1; }
            if (years < 0) return null;
            return months > 0 ? $"{years}y {months}m" : $"{years}y";
        }

        var now = DateTime.Now;
        var list = raw.Select(a => {
            var appointmentDateTime = a.Date.ToDateTime(a.StartTime);
            var status = a.Status;
            var hasDiagnosis = !string.IsNullOrWhiteSpace(a.Diagnosis);
            
			// Respect explicit Cancelled status from DB
			if (!string.IsNullOrWhiteSpace(status) && string.Equals(status, "Cancelled", StringComparison.OrdinalIgnoreCase))
			{
				status = "Cancelled";
			}
			// If appointment has diagnosis, it's Completed
			else if (hasDiagnosis)
            {
                status = "Completed";
            }
            // If appointment is in the past and no diagnosis, mark as Missed
            else if (appointmentDateTime < now)
            {
                status = "Missed";
            }
            // If appointment is in the future, mark as Scheduled
            else if (appointmentDateTime > now)
            {
                status = "Scheduled";
            }
            // Fallback: If status is empty or null, determine based on date
            else if (string.IsNullOrWhiteSpace(status))
            {
                status = appointmentDateTime > now ? "Scheduled" : "Missed";
            }
            
            return new AppointmentListItem
            {
                Id = a.Id,
                PatientName = _enc.Decrypt(a.PatientName) ?? a.PatientName,
                PatientMRNumber = a.PatientMRNumber,
                Age = AgeFromDob(profiles.FirstOrDefault(p => p.Id == a.PatientProfileId)?.DateOfBirth),
                DoctorName = _enc.Decrypt(a.DoctorName) ?? a.DoctorName,
                Department = a.Department,
                Date = a.Date,
                StartTime = a.StartTime,
                Status = status
            };
        }).ToList();
        return list;
    }
}

public sealed class GetAppointmentHandler : IRequestHandler<GetAppointmentQuery, AppointmentDetailDto?>
{
    private readonly IApplicationDbContext _db;
    private readonly IEncryptionService _enc;
    public GetAppointmentHandler(IApplicationDbContext db, IEncryptionService enc)
    {
        _db = db; _enc = enc;
    }
    public async Task<AppointmentDetailDto?> Handle(GetAppointmentQuery request, CancellationToken cancellationToken)
    {
        var a = await _db.Appointments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (a == null) return null;
        
        var now = DateTime.Now;
        var appointmentDateTime = a.Date.ToDateTime(a.StartTime);
        var status = a.Status;
        var hasDiagnosis = !string.IsNullOrWhiteSpace(a.Diagnosis);
        
		// Respect explicit Cancelled status from DB
		if (!string.IsNullOrWhiteSpace(status) && string.Equals(status, "Cancelled", StringComparison.OrdinalIgnoreCase))
		{
			status = "Cancelled";
		}
		// If appointment has diagnosis, it's Completed
		else if (hasDiagnosis)
        {
            status = "Completed";
        }
        // If appointment is in the past and no diagnosis, mark as Missed
        else if (appointmentDateTime < now)
        {
            status = "Missed";
        }
        // If appointment is in the future, mark as Scheduled
        else if (appointmentDateTime > now)
        {
            status = "Scheduled";
        }
        // Fallback: If status is empty or null, determine based on date
        else if (string.IsNullOrWhiteSpace(status))
        {
            status = appointmentDateTime > now ? "Scheduled" : "Missed";
        }
        
        return new AppointmentDetailDto
        {
            Id = a.Id,
            PatientName = _enc.Decrypt(a.PatientName) ?? a.PatientName,
            PatientMRNumber = a.PatientMRNumber,
            PatientProfileId = a.PatientProfileId,
            Department = a.Department,
            DoctorName = _enc.Decrypt(a.DoctorName) ?? a.DoctorName,
            DoctorProfileId = a.DoctorProfileId,
            ConsultMode = a.ConsultMode,
            PaymentMode = a.PaymentMode,
            Date = a.Date,
            StartTime = a.StartTime,
            EndTime = a.EndTime,
            Reason = a.Reason,
            Notes = a.Notes,
            Status = status,
            InvestigationRvg = a.InvestigationRvg,
            InvestigationOpg = a.InvestigationOpg,
            InvestigationCeph = a.InvestigationCeph,
            InvestigationOcclusal = a.InvestigationOcclusal,
            InvestigationCbct = a.InvestigationCbct,
            InvestigationBlood = a.InvestigationBlood,
            InvestigationOthers = a.InvestigationOthers,
            DifferentialDiagnosis = a.DifferentialDiagnosis,
            Diagnosis = a.Diagnosis,
            TreatmentPlan = a.TreatmentPlan
        };
    }
}


