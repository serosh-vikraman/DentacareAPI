using Application.Abstractions;
using Application.Patients.Dtos;
using Domain.Patients;
using MediatR;
using Shared.Tenant;

namespace Application.Patients.Commands;

public sealed record CreatePatientCommand(CreatePatientRequest Request) : IRequest<Guid>;

public sealed class CreatePatientHandler : IRequestHandler<CreatePatientCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    private readonly ITenantProvider _tenant;

    public CreatePatientHandler(IApplicationDbContext db, ITenantProvider tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    public async Task<Guid> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;
        var entity = new Patient(_tenant.TenantId, _tenant.BranchId, r.FirstName, r.LastName);
        if (!string.IsNullOrWhiteSpace(r.Email)) entity.GetType().GetProperty("Email")?.SetValue(entity, r.Email);
        if (!string.IsNullOrWhiteSpace(r.Phone)) entity.GetType().GetProperty("Phone")?.SetValue(entity, r.Phone);
        _db.Patients.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}






