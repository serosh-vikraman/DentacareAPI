using Application.Abstractions;
using Application.Appointments.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Appointments.Commands;

public sealed record UpdateAppointmentCommand(Guid Id, UpdateAppointmentRequest Request) : IRequest<bool>;

public sealed class UpdateAppointmentHandler : IRequestHandler<UpdateAppointmentCommand, bool>
{
    private readonly IApplicationDbContext _db;

    public UpdateAppointmentHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<bool> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Appointments.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return false;
        var r = request.Request;
        if (!string.IsNullOrWhiteSpace(r.Status)) entity.Status = r.Status.Trim();
        if (r.Reason != null) entity.Reason = r.Reason.Trim();
        if (r.Notes != null) entity.Notes = r.Notes.Trim();
        if (r.InvestigationRvg != null) entity.InvestigationRvg = r.InvestigationRvg.Trim();
        if (r.InvestigationOpg.HasValue) entity.InvestigationOpg = r.InvestigationOpg;
        if (r.InvestigationCeph.HasValue) entity.InvestigationCeph = r.InvestigationCeph;
        if (r.InvestigationOcclusal.HasValue) entity.InvestigationOcclusal = r.InvestigationOcclusal;
        if (r.InvestigationCbct.HasValue) entity.InvestigationCbct = r.InvestigationCbct;
        if (r.InvestigationBlood != null) entity.InvestigationBlood = r.InvestigationBlood.Trim();
        if (r.InvestigationOthers != null) entity.InvestigationOthers = r.InvestigationOthers.Trim();
        if (r.DifferentialDiagnosis != null) entity.DifferentialDiagnosis = r.DifferentialDiagnosis.Trim();
        if (r.Diagnosis != null) entity.Diagnosis = r.Diagnosis.Trim();
        if (r.TreatmentPlan != null) entity.TreatmentPlan = r.TreatmentPlan.Trim();
        if (r.InvestigationRvg != null) entity.InvestigationRvg = r.InvestigationRvg.Trim();
        if (r.InvestigationOpg.HasValue) entity.InvestigationOpg = r.InvestigationOpg;
        if (r.InvestigationCeph.HasValue) entity.InvestigationCeph = r.InvestigationCeph;
        if (r.InvestigationOcclusal.HasValue) entity.InvestigationOcclusal = r.InvestigationOcclusal;
        if (r.InvestigationCbct.HasValue) entity.InvestigationCbct = r.InvestigationCbct;
        if (r.InvestigationBlood != null) entity.InvestigationBlood = r.InvestigationBlood.Trim();
        if (r.InvestigationOthers != null) entity.InvestigationOthers = r.InvestigationOthers.Trim();
        if (r.DifferentialDiagnosis != null) entity.DifferentialDiagnosis = r.DifferentialDiagnosis.Trim();
        if (r.Diagnosis != null) entity.Diagnosis = r.Diagnosis.Trim();
        if (r.TreatmentPlan != null) entity.TreatmentPlan = r.TreatmentPlan.Trim();
        entity.UpdatedUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}





