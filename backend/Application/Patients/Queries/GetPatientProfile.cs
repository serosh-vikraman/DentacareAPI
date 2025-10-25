using Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Patients.Queries;

public sealed record GetPatientProfileQuery(Guid Id) : IRequest<PatientProfileDetailDto?>;

public sealed class PatientProfileDetailDto
{
	public Guid Id { get; init; }
	public string MRNumber { get; init; } = string.Empty;
	public string PatientName { get; init; } = string.Empty;
	public DateOnly? DateOfBirth { get; init; }
	public string? BloodGroup { get; init; }
	public string? Gender { get; init; }
	public string? PatientType { get; init; }
	public string? MaritalStatus { get; init; }
	public string? Mobile { get; init; }
	public string? AltPhone { get; init; }
	public string? Email { get; init; }
	public string? Address1 { get; init; }
	public string? Address2 { get; init; }
	public string? City { get; init; }
	public string? State { get; init; }
	public string? Zip { get; init; }
	public string? PhotoUrl { get; init; }
	public string? BP { get; init; }
	public int? Pulse { get; init; }
	public int? RespiratoryRate { get; init; }
	public double? TemperatureF { get; init; }
	public int? SpO2 { get; init; }
	public double? BloodSugar { get; init; }
	public int? PainLevel { get; init; }
	public string? Hygiene { get; init; }
	public string? Gingival { get; init; }
	public string? Pockets { get; init; }
	public string? Mobility { get; init; }
	public string? Caries { get; init; }
	public string? MucosalNotes { get; init; }
	public string? TMJNotes { get; init; }
	public string? OcclusionNotes { get; init; }
	public string? IntraExtraNotes { get; init; }
	public string? ContinuousSpO2 { get; init; }
	public string? HeartRateECG { get; init; }
	public string? SalivaPHFlow { get; init; }
	// Medical history
	public bool OnTreatment { get; init; }
	public string? AllergicMedicines { get; init; }
	public bool Diabetes { get; init; }
	public string? Cardiac { get; init; }
	public string? Neuro { get; init; }
	public string? Pregnancy { get; init; }
	public string? OtherConditions { get; init; }
	// Dental history flags
	public bool DentalHxSyncope { get; init; }
	public bool DentalHxAllergyLA { get; init; }
	public bool DentalHxEndo { get; init; }
	public bool DentalHxOrtho { get; init; }
	public bool DentalHxPerio { get; init; }
	public bool DentalHxSurgical { get; init; }
	public bool DentalHxExtraction { get; init; }
	public bool DentalHxRPD { get; init; }
	public bool DentalHxFPD { get; init; }
	public string? DentalHxOther { get; init; }
	// Complaints
	public string? ChiefComplaint { get; init; }
	public string? OE { get; init; }
	public string? EO { get; init; }
	public string? IO { get; init; }
    public string? EmergencyContactName { get; init; }
    public string? EmergencyContactPhone { get; init; }
}

public sealed class GetPatientProfileHandler : IRequestHandler<GetPatientProfileQuery, PatientProfileDetailDto?>
{
	private readonly IApplicationDbContext _db;
	private readonly Shared.Security.IEncryptionService _enc;
	private readonly Shared.Security.ICurrentUserService _current;
	public GetPatientProfileHandler(IApplicationDbContext db, Shared.Security.IEncryptionService enc, Shared.Security.ICurrentUserService current)
	{ _db = db; _enc = enc; _current = current; }

	public async Task<PatientProfileDetailDto?> Handle(GetPatientProfileQuery request, CancellationToken cancellationToken)
	{
		var tenantId = _current.TenantId ?? Guid.Empty;
		var p = await _db.PatientProfiles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id && x.TenantId == tenantId && !x.IsDeleted, cancellationToken)
			?? await _db.PatientProfiles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
		if (p == null) return null;
		return new PatientProfileDetailDto
		{
			Id = p.Id,
			MRNumber = p.MRNumber,
			PatientName = p.PatientName,
			DateOfBirth = p.DateOfBirth,
			BloodGroup = p.BloodGroup,
			Gender = p.Gender,
			PatientType = p.PatientType,
			MaritalStatus = p.MaritalStatus,
			Mobile = _enc.Decrypt(p.Mobile),
			AltPhone = _enc.Decrypt(p.AltPhone),
			Email = _enc.Decrypt(p.Email),
			Address1 = p.Address1,
			Address2 = p.Address2,
			City = p.City,
			State = p.State,
			Zip = p.Zip,
			PhotoUrl = p.PhotoUrl,
			BP = p.BP,
			Pulse = p.Pulse,
			RespiratoryRate = p.RespiratoryRate,
			TemperatureF = p.TemperatureF,
			SpO2 = p.SpO2,
			BloodSugar = p.BloodSugar,
			PainLevel = p.PainLevel,
			Hygiene = p.Hygiene,
			Gingival = p.Gingival,
			Pockets = p.Pockets,
			Mobility = p.Mobility,
			Caries = p.Caries,
			MucosalNotes = p.MucosalNotes,
			TMJNotes = p.TMJNotes,
			OcclusionNotes = p.OcclusionNotes,
			IntraExtraNotes = p.IntraExtraNotes,
			ContinuousSpO2 = p.ContinuousSpO2,
			HeartRateECG = p.HeartRateECG,
			SalivaPHFlow = p.SalivaPHFlow,
			OnTreatment = p.OnTreatment,
			AllergicMedicines = p.AllergicMedicines,
			Diabetes = p.Diabetes,
			Cardiac = p.Cardiac,
			Neuro = p.Neuro,
			Pregnancy = p.Pregnancy,
			OtherConditions = p.OtherConditions,
			DentalHxSyncope = p.HxSyncope,
			DentalHxAllergyLA = p.HxAllergyLA,
			DentalHxEndo = p.HxEndo,
			DentalHxOrtho = p.HxOrtho,
			DentalHxPerio = p.HxPerio,
			DentalHxSurgical = p.HxSurgical,
			DentalHxExtraction = p.HxExtraction,
			DentalHxRPD = p.HxRPD,
			DentalHxFPD = p.HxFPD,
			DentalHxOther = p.HxOther,
			ChiefComplaint = p.ChiefComplaint,
			OE = p.OE,
			EO = p.EO,
			IO = p.IO
            ,EmergencyContactName = p.EmergencyContactName
            ,EmergencyContactPhone = p.EmergencyContactPhone
		};
	}
}


