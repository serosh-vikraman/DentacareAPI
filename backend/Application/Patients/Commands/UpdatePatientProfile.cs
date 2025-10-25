using Application.Abstractions;
using Application.Patients.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Patients.Commands;

public sealed record UpdatePatientProfileCommand(Guid Id, SavePatientProfileRequest Request) : IRequest<bool>;

public sealed class UpdatePatientProfileHandler : IRequestHandler<UpdatePatientProfileCommand, bool>
{
    private readonly IApplicationDbContext _db;
    public UpdatePatientProfileHandler(IApplicationDbContext db) { _db = db; }

    public async Task<bool> Handle(UpdatePatientProfileCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;
        var p = await _db.PatientProfiles.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        if (p == null) return false;

        p.PatientName = r.PatientName;
        p.DateOfBirth = ParseDate(r.Dob);
        p.BloodGroup = r.BloodGroup;
        p.Gender = r.Gender;
        p.PatientType = r.PatientType;
        p.MaritalStatus = r.MaritalStatus;
        p.Mobile = r.Mobile;
        p.AltPhone = r.AltPhone;
        p.Email = r.Email;
        p.Address1 = r.Address1;
        p.Address2 = r.Address2;
        p.City = r.City;
        p.State = r.State;
        p.Zip = r.Zip;
        p.PhotoUrl = r.PhotoUrl;
        p.EmergencyContactName = r.EmergencyContactName;
        p.EmergencyContactPhone = r.EmergencyContactPhone;

        p.ChiefComplaint = r.ChiefComplaint;
        p.OE = r.OE; p.EO = r.EO; p.IO = r.IO;

        p.BP = r.BP; p.Pulse = r.Pulse; p.RespiratoryRate = r.RespiratoryRate; p.TemperatureF = r.TemperatureF; p.SpO2 = r.SpO2; p.BloodSugar = r.BloodSugar;
        p.PainLevel = r.PainLevel; p.Hygiene = r.Hygiene; p.Gingival = r.Gingival; p.Pockets = r.Pockets; p.Mobility = r.Mobility; p.Caries = r.Caries;
        p.MucosalNotes = r.MucosalNotes; p.TMJNotes = r.TMJNotes; p.OcclusionNotes = r.OcclusionNotes; p.IntraExtraNotes = r.IntraExtraNotes;
        p.ContinuousSpO2 = r.ContinuousSpO2; p.HeartRateECG = r.HeartRateECG; p.SalivaPHFlow = r.SalivaPHFlow;

        p.OnTreatment = (r.OnTreatment ?? "No").Equals("Yes", StringComparison.OrdinalIgnoreCase);
        p.AllergicMedicines = r.AllergicMedicines;
        p.Diabetes = (r.Diabetes ?? "No").Equals("Yes", StringComparison.OrdinalIgnoreCase);
        p.Cardiac = r.Cardiac; p.Neuro = r.Neuro; p.Pregnancy = r.Pregnancy; p.OtherConditions = r.OtherConditions;

        p.HxSyncope = r.DentalHxSyncope; p.HxAllergyLA = r.DentalHxAllergyLA; p.HxEndo = r.DentalHxEndo; p.HxOrtho = r.DentalHxOrtho; p.HxPerio = r.DentalHxPerio; p.HxSurgical = r.DentalHxSurgical; p.HxExtraction = r.DentalHxExtraction; p.HxRPD = r.DentalHxRPD; p.HxFPD = r.DentalHxFPD; p.HxOther = r.DentalHxOther;

        p.UpdatedUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
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
                var yy = int.Parse(parts[2]);
                if (yy < 100) yy += 1900;
                return new DateOnly(yy, mm, dd);
            }
        }
        catch { }
        return null;
    }
}



