using Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Commands;

public sealed record UpdateServiceCommand(Guid Id, string Name, decimal Amount, string? Category, string? Description, bool? IsActive, bool? IsTaxable, decimal? TaxRate) : IRequest<bool>;

public sealed class UpdateServiceHandler : IRequestHandler<UpdateServiceCommand, bool>
{
    private readonly IApplicationDbContext _db;
    private readonly Shared.Security.ICurrentUserService _current;
    public UpdateServiceHandler(IApplicationDbContext db, Shared.Security.ICurrentUserService current)
    { _db = db; _current = current; }

    public async Task<bool> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _current.TenantId ?? Guid.Empty;
        var entity = await _db.Services.FirstOrDefaultAsync(s => s.Id == request.Id && s.TenantId == tenantId && !s.IsDeleted, cancellationToken);
        if (entity == null) return false;
        if (!string.IsNullOrWhiteSpace(request.Name)) entity.Name = request.Name.Trim();
        entity.Amount = request.Amount;
        entity.Category = string.IsNullOrWhiteSpace(request.Category) ? null : request.Category!.Trim();
        entity.UpdatedUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}



