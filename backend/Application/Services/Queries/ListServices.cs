using Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Queries;

public sealed record ListServicesQuery(string? Q) : IRequest<IReadOnlyList<ServiceListItem>>;
public sealed record ServiceListItem(Guid Id, string Name, decimal Amount, string? Category);

public sealed class ListServicesHandler : IRequestHandler<ListServicesQuery, IReadOnlyList<ServiceListItem>>
{
    private readonly IApplicationDbContext _db;
    private readonly Shared.Security.ICurrentUserService _current;

    public ListServicesHandler(IApplicationDbContext db, Shared.Security.ICurrentUserService current)
    { _db = db; _current = current; }

    public async Task<IReadOnlyList<ServiceListItem>> Handle(ListServicesQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _current.TenantId ?? Guid.Empty;
        var q = _db.Services
            .Where(s => s.TenantId == tenantId && !s.IsDeleted);
        if (!string.IsNullOrWhiteSpace(request.Q))
        {
            var str = request.Q.Trim().ToLower();
            q = q.Where(s => s.Name.ToLower().Contains(str));
        }
        return await q.OrderBy(s => s.Name).Select(s => new ServiceListItem(s.Id, s.Name, s.Amount, s.Category)).ToListAsync(cancellationToken);
    }
}




