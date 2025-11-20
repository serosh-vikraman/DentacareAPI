using Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Commands;

public sealed record DeleteServiceCommand(Guid Id) : IRequest<bool>;

public sealed class DeleteServiceHandler : IRequestHandler<DeleteServiceCommand, bool>
{
    private readonly IApplicationDbContext _db;
    private readonly Shared.Security.ICurrentUserService _current;
    public DeleteServiceHandler(IApplicationDbContext db, Shared.Security.ICurrentUserService current)
    { _db = db; _current = current; }

    public async Task<bool> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _current.TenantId ?? Guid.Empty;
        var entity = await _db.Services.FirstOrDefaultAsync(s => s.Id == request.Id && s.TenantId == tenantId && !s.IsDeleted, cancellationToken);
        if (entity == null) return false;
        entity.IsDeleted = true;
        entity.UpdatedUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}



