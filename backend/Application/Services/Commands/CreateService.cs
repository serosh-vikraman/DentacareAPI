using Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Commands;

public sealed record CreateServiceCommand(string Name, decimal Amount, string? Category, string? Description, bool? IsActive, bool? IsTaxable, decimal? TaxRate) : IRequest<Guid>;

public sealed class CreateServiceHandler : IRequestHandler<CreateServiceCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    private readonly Shared.Security.ICurrentUserService _current;
    public CreateServiceHandler(IApplicationDbContext db, Shared.Security.ICurrentUserService current)
    { _db = db; _current = current; }

    public async Task<Guid> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Name is required", nameof(request.Name));

        var tenantId = _current.TenantId ?? Guid.Empty;
        var entity = new Domain.Services.Service
        {
            TenantId = tenantId,
            Name = request.Name.Trim(),
            Amount = request.Amount,
            Category = string.IsNullOrWhiteSpace(request.Category) ? null : request.Category!.Trim()
        };
        _db.Services.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}


