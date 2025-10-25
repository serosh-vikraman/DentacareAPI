using Application.Abstractions;
using Application.Patients.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Patients.Commands;

public sealed record UpdatePatientCommand(Guid Id, UpdatePatientRequest Request) : IRequest<bool>;

public sealed class UpdatePatientHandler : IRequestHandler<UpdatePatientCommand, bool>
{
    private readonly IApplicationDbContext _db;

    public UpdatePatientHandler(IApplicationDbContext db) { _db = db; }

    public async Task<bool> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var p = await _db.Patients.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (p == null) return false;
        p.UpdateBasicInfo(request.Request.FirstName, request.Request.LastName);
        p.UpdateContact(request.Request.Email, request.Request.Phone);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}






