using Application.Abstractions;
using Application.Patients.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Patients.Queries;

public sealed record ListPatientsQuery(int Limit = 100) : IRequest<IReadOnlyList<PatientDto>>;

public sealed class ListPatientsHandler : IRequestHandler<ListPatientsQuery, IReadOnlyList<PatientDto>>
{
    private readonly IApplicationDbContext _db;

    public ListPatientsHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<PatientDto>> Handle(ListPatientsQuery request, CancellationToken cancellationToken)
    {
        var q = _db.Patients
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Take(request.Limit)
            .Select(p => new PatientDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                Phone = p.Phone
            });
        return await q.ToListAsync(cancellationToken);
    }
}






