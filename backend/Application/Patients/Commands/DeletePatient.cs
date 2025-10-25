using Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Patients.Commands;

public sealed record DeletePatientCommand(Guid Id) : IRequest<bool>;

public sealed class DeletePatientHandler : IRequestHandler<DeletePatientCommand, bool>
{
    private readonly IApplicationDbContext _db;

    public DeletePatientHandler(IApplicationDbContext db) { _db = db; }

    public async Task<bool> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
    {
        var p = await _db.Patients.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (p == null) return false;
        p.IsDeleted = true;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}






