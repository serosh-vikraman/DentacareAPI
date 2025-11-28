using Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Security;

namespace Application.Patients.Queries;

using Application.Common.Models;

public sealed class ListPatientProfilesQuery : IRequest<PaginatedList<PatientProfileListItem>>
{
    public string? Query { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public ListPatientProfilesQuery(string? query, int pageNumber = 1, int pageSize = 20) 
    { 
        Query = query; 
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}

public sealed record PatientProfileListItem(Guid Id, string MRNumber, string PatientName, string? Gender, string? Mobile, string? City, string? PhotoUrl);

public sealed class ListPatientProfilesHandler : IRequestHandler<ListPatientProfilesQuery, PaginatedList<PatientProfileListItem>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _current;
    private readonly IEncryptionService _enc;
    public ListPatientProfilesHandler(IApplicationDbContext db, ICurrentUserService current, IEncryptionService enc)
    {
        _db = db; _current = current; _enc = enc;
    }

    public async Task<PaginatedList<PatientProfileListItem>> Handle(ListPatientProfilesQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _current.TenantId ?? Guid.Empty;
        var query = _db.PatientProfiles
            .Where(p => p.TenantId == tenantId && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedUtc)
            .Select(p => new { p.Id, p.MRNumber, p.PatientName, p.Gender, p.Mobile, p.City, p.PhotoUrl });

        // If searching, we must fetch all (or filtered by unencrypted fields) to decrypt mobile
        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var term = request.Query.Trim().ToLowerInvariant();
            
            // Optimization: if term matches unencrypted fields, we can filter in DB? 
            // But we need OR logic with Mobile. So we can't easily filter in DB if we want to include Mobile matches.
            // Fetch all to be safe for Mobile search.
            var allItems = await query.ToListAsync(cancellationToken);
            
            var filtered = allItems.Select(p => new PatientProfileListItem(
                p.Id,
                p.MRNumber,
                p.PatientName,
                p.Gender,
                _enc.Decrypt(p.Mobile),
                p.City,
                p.PhotoUrl
            )).Where(i =>
                (i.PatientName ?? string.Empty).ToLowerInvariant().Contains(term)
                || (i.Mobile ?? string.Empty).Contains(term)
                || (i.MRNumber ?? string.Empty).ToLowerInvariant().Contains(term)
            ).ToList();

            var count = filtered.Count;
            var pageItems = filtered
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PaginatedList<PatientProfileListItem>(pageItems, count, request.PageNumber, request.PageSize);
        }
        else
        {
            // No search, use DB pagination
            var count = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var mapped = items.Select(p => new PatientProfileListItem(
                p.Id,
                p.MRNumber,
                p.PatientName,
                p.Gender,
                _enc.Decrypt(p.Mobile),
                p.City,
                p.PhotoUrl
            )).ToList();

            return new PaginatedList<PatientProfileListItem>(mapped, count, request.PageNumber, request.PageSize);
        }
    }
}


