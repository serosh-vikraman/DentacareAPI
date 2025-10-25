using Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Appointments.Commands;

public sealed record DeleteAppointmentCommand(Guid Id) : IRequest<bool>;

public sealed class DeleteAppointmentHandler : IRequestHandler<DeleteAppointmentCommand, bool>
{
    private readonly IApplicationDbContext _db;

    public DeleteAppointmentHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<bool> Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Appointments.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return false;
        entity.IsDeleted = true;
        entity.UpdatedUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}







