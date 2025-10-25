using Application.Abstractions;
using Application.Patients.Dtos;
using Domain.Patients;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Patients.Commands;

public sealed class SavePatientProfileCommand : IRequest<Guid>
{
    public SavePatientProfileRequest Request { get; }
    public Guid TenantId { get; }
    public Guid? BranchId { get; }
    public SavePatientProfileCommand(SavePatientProfileRequest request, Guid tenantId, Guid? branchId)
    {
        Request = request;
        TenantId = tenantId;
        BranchId = branchId;
    }
}

public sealed class SavePatientProfileHandler : IRequestHandler<SavePatientProfileCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    public SavePatientProfileHandler(IApplicationDbContext db) { _db = db; }

    public async Task<Guid> Handle(SavePatientProfileCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;
        Console.WriteLine($"[SavePatientProfile] Incoming Id: {r.Id}");
        PatientProfile entity;
        if (r.Id.HasValue && r.Id.Value != Guid.Empty)
        {
            entity = await _db.PatientProfiles.FirstOrDefaultAsync(x => x.Id == r.Id.Value && x.TenantId == request.TenantId && !x.IsDeleted, cancellationToken)
                ?? await _db.PatientProfiles.FirstOrDefaultAsync(x => x.Id == r.Id.Value && !x.IsDeleted, cancellationToken)
                ?? throw new Exception("Patient profile not found for update.");
            Console.WriteLine($"[SavePatientProfile] Updating existing profile {entity.Id}");
            entity.UpdatedUtc = DateTime.UtcNow;
        }
        else
        {
            entity = new PatientProfile
            {
                TenantId = request.TenantId,
                BranchId = request.BranchId,
                MRNumber = await GenerateNextMrNumber(_db, request.TenantId, cancellationToken)
            };
            _db.PatientProfiles.Add(entity);
            Console.WriteLine("[SavePatientProfile] Creating NEW profile");
        }

        entity.PatientName = r.PatientName;
        entity.DateOfBirth = ParseDate(r.Dob);
        entity.BloodGroup = r.BloodGroup;
        entity.Gender = r.Gender;
        entity.PatientType = r.PatientType;
        entity.MaritalStatus = r.MaritalStatus;
        entity.Mobile = r.Mobile;
        entity.AltPhone = r.AltPhone;
        entity.Email = r.Email;
        entity.Address1 = r.Address1;
        entity.Address2 = r.Address2;
        entity.City = r.City;
        entity.State = r.State;
        entity.Zip = r.Zip;
        entity.PhotoUrl = r.PhotoUrl;
        entity.EmergencyContactName = r.EmergencyContactName;
        entity.EmergencyContactPhone = r.EmergencyContactPhone;
        entity.ChiefComplaint = r.ChiefComplaint;
        entity.OE = r.OE;
        entity.EO = r.EO;
        entity.IO = r.IO;
        entity.BP = r.BP;
        entity.Pulse = r.Pulse;
        entity.RespiratoryRate = r.RespiratoryRate;
        entity.TemperatureF = r.TemperatureF;
        entity.SpO2 = r.SpO2;
        entity.BloodSugar = r.BloodSugar;
        entity.PainLevel = r.PainLevel;
        entity.Hygiene = r.Hygiene;
        entity.Gingival = r.Gingival;
        entity.Pockets = r.Pockets;
        entity.Mobility = r.Mobility;
        entity.Caries = r.Caries;
        entity.MucosalNotes = r.MucosalNotes;
        entity.TMJNotes = r.TMJNotes;
        entity.OcclusionNotes = r.OcclusionNotes;
        entity.IntraExtraNotes = r.IntraExtraNotes;
        entity.ContinuousSpO2 = r.ContinuousSpO2;
        entity.HeartRateECG = r.HeartRateECG;
        entity.SalivaPHFlow = r.SalivaPHFlow;
        entity.OnTreatment = (r.OnTreatment ?? "No").Equals("Yes", StringComparison.OrdinalIgnoreCase);
        entity.AllergicMedicines = r.AllergicMedicines;
        entity.Diabetes = (r.Diabetes ?? "No").Equals("Yes", StringComparison.OrdinalIgnoreCase);
        entity.Cardiac = r.Cardiac;
        entity.Neuro = r.Neuro;
        entity.Pregnancy = r.Pregnancy;
        entity.OtherConditions = r.OtherConditions;
        entity.HxSyncope = r.DentalHxSyncope;
        entity.HxAllergyLA = r.DentalHxAllergyLA;
        entity.HxEndo = r.DentalHxEndo;
        entity.HxOrtho = r.DentalHxOrtho;
        entity.HxPerio = r.DentalHxPerio;
        entity.HxSurgical = r.DentalHxSurgical;
        entity.HxExtraction = r.DentalHxExtraction;
        entity.HxRPD = r.DentalHxRPD;
        entity.HxFPD = r.DentalHxFPD;
        entity.HxOther = r.DentalHxOther;

        await _db.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    private static string? CombineNotes(params string?[] notes)
    {
        var parts = notes.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        return parts.Length == 0 ? null : string.Join(" | ", parts!);
    }

    private static async Task<string> GenerateNextMrNumber(IApplicationDbContext db, Guid tenantId, CancellationToken ct)
    {
        // naive approach: count existing; for production use a separate sequence table with concurrency control
        var count = await db.PatientProfiles.CountAsync(ct);
        var next = count + 1;
        return next.ToString("D6");
    }

    private static DateOnly? ParseDate(string? v)
    {
        if (string.IsNullOrWhiteSpace(v)) return null;
        if (DateOnly.TryParse(v, out var d1)) return d1;
        try
        {
            var parts = v.Split('/', '-', '.');
            if (parts.Length == 3)
            {
                var dd = int.Parse(parts[0]);
                var mm = int.Parse(parts[1]);
                var yy2 = int.Parse(parts[2]);
                int yyyy;
                if (yy2 < 100)
                {
                    yyyy = yy2 <= 49 ? 2000 + yy2 : 1900 + yy2; // 00-49 => 2000-2049, 50-99 => 1950-1999
                }
                else
                {
                    yyyy = yy2;
                }
                return new DateOnly(yyyy, mm, dd);
            }
        }
        catch { }
        return null;
    }
}


