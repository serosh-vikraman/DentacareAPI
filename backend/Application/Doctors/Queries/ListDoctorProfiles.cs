using Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Security;

namespace Application.Doctors.Queries;

public sealed record DoctorProfileListItem(Guid Id, string FullName, string? Specialization);

public sealed class ListDoctorProfilesQuery : IRequest<IReadOnlyList<DoctorProfileListItem>>
{
    public string? Query { get; }
    public ListDoctorProfilesQuery(string? query) { Query = query; }
}

public sealed class ListDoctorProfilesHandler : IRequestHandler<ListDoctorProfilesQuery, IReadOnlyList<DoctorProfileListItem>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _current;
    private readonly IEncryptionService _enc;

    public ListDoctorProfilesHandler(IApplicationDbContext db, ICurrentUserService current, IEncryptionService enc)
    {
        _db = db; _current = current; _enc = enc;
    }

    public async Task<IReadOnlyList<DoctorProfileListItem>> Handle(ListDoctorProfilesQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _current.TenantId ?? Guid.Empty;
        var list = await _db.DoctorProfiles
            .Where(d => d.TenantId == tenantId && !d.IsDeleted)
            .OrderBy(d => d.FullName)
            .Select(d => new { d.Id, d.FullName, d.Specialization })
            .ToListAsync(cancellationToken);
        var items = list.Select(d => new DoctorProfileListItem(
            d.Id,
            _enc.Decrypt(d.FullName) ?? d.FullName,
            d.Specialization
        ));
        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var q = request.Query.Trim().ToLowerInvariant();
            items = items.Where(x => x.FullName.ToLowerInvariant().Contains(q) || (x.Specialization ?? string.Empty).ToLowerInvariant().Contains(q));
        }
        return items.ToList();
    }
}







