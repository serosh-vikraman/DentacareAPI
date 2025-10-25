using Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Security;

namespace Application.Patients.Queries;

public sealed class ListPatientProfilesQuery : IRequest<IReadOnlyList<PatientProfileListItem>>
{
    public string? Query { get; }
    public ListPatientProfilesQuery(string? query) { Query = query; }
}

public sealed record PatientProfileListItem(Guid Id, string MRNumber, string PatientName, string? Gender, string? Mobile, string? City, string? PhotoUrl);

public sealed class ListPatientProfilesHandler : IRequestHandler<ListPatientProfilesQuery, IReadOnlyList<PatientProfileListItem>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _current;
    private readonly IEncryptionService _enc;
    public ListPatientProfilesHandler(IApplicationDbContext db, ICurrentUserService current, IEncryptionService enc)
    {
        _db = db; _current = current; _enc = enc;
    }

    public async Task<IReadOnlyList<PatientProfileListItem>> Handle(ListPatientProfilesQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _current.TenantId ?? Guid.Empty;
        var q = await _db.PatientProfiles
            .Where(p => p.TenantId == tenantId && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedUtc)
            .Select(p => new { p.Id, p.MRNumber, p.PatientName, p.Gender, p.Mobile, p.City, p.PhotoUrl })
            .ToListAsync(cancellationToken);

        var items = q.Select(p => new PatientProfileListItem(
            p.Id,
            p.MRNumber,
            p.PatientName,
            p.Gender,
            _enc.Decrypt(p.Mobile),
            p.City,
            p.PhotoUrl
        ));

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var term = request.Query.Trim().ToLowerInvariant();
            items = items.Where(i =>
                (i.PatientName ?? string.Empty).ToLowerInvariant().Contains(term)
                || (i.Mobile ?? string.Empty).Contains(term)
                || (i.MRNumber ?? string.Empty).ToLowerInvariant().Contains(term));
        }

        return items.ToList();
    }
}


