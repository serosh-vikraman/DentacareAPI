using Application.Appointments.Commands;
using Application.Appointments.Dtos;
using Application.Appointments.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Security;

namespace WebApi.Appointments;

public static class AppointmentEndpoints
{
    public static IEndpointRouteBuilder MapAppointmentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/appointments", async (string? q, int? page_number, int? page_size, string? date, IMediator mediator) =>
        {
            DateOnly? d = null;
            if (DateOnly.TryParse(date, out var parsed)) d = parsed;
            var list = await mediator.Send(new ListAppointmentsQuery(q, page_number ?? 1, page_size ?? 20, d));
            return Results.Ok(list);
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Dentist","Receptionist","Accountant"));

        app.MapGet("/api/appointments/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var one = await mediator.Send(new GetAppointmentQuery(id));
            return one != null ? Results.Ok(one) : Results.NotFound();
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Dentist","Receptionist","Accountant"));

		app.MapPost("/api/appointments", async (CreateAppointmentRequest req, IMediator mediator, Application.Abstractions.IApplicationDbContext db) =>
        {
            var id = await mediator.Send(new CreateAppointmentCommand(req));
			// include patient info for immediate UI updates
			var info = await db.Appointments.AsNoTracking()
				.Where(a => a.Id == id)
				.Select(a => new { a.PatientProfileId, a.PatientMRNumber })
				.FirstOrDefaultAsync();

            string? age = null;
            if (info?.PatientProfileId != null)
            {
                var dob = await db.PatientProfiles.AsNoTracking()
                    .Where(p => p.Id == info.PatientProfileId)
                    .Select(p => p.DateOfBirth)
                    .FirstOrDefaultAsync();
                
                if (dob.HasValue)
                {
                    var today = DateOnly.FromDateTime(DateTime.UtcNow);
                    int years = today.Year - dob.Value.Year;
                    int months = today.Month - dob.Value.Month;
                    if (today.Day < dob.Value.Day) months -= 1;
                    if (months < 0) { months += 12; years -= 1; }
                    if (years >= 0) age = months > 0 ? $"{years}y {months}m" : $"{years}y";
                }
            }

			return Results.Created($"/api/appointments/{id}", new { id, patientProfileId = info?.PatientProfileId, patientMRNumber = info?.PatientMRNumber, age });
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Receptionist","Dentist"));

        app.MapPut("/api/appointments/{id:guid}", async (Guid id, UpdateAppointmentRequest req, IMediator mediator) =>
        {
            var ok = await mediator.Send(new UpdateAppointmentCommand(id, req));
            return ok ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Receptionist","Dentist"));

        app.MapDelete("/api/appointments/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var ok = await mediator.Send(new DeleteAppointmentCommand(id));
            return ok ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner"));

        app.MapPost("/api/appointments/{id:guid}/payment", async (Guid id, Application.Appointments.Dtos.SavePaymentRequest req, IMediator mediator) =>
        {
            if (id != req.AppointmentId) return Results.BadRequest(new { message = "Appointment id mismatch" });
            var payId = await mediator.Send(new Application.Appointments.Commands.SavePaymentCommand(req));
            return Results.Ok(new { id = payId });
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Receptionist","Dentist"));

        app.MapGet("/api/appointments/{id:guid}/payments", async (Guid id, Application.Abstractions.IApplicationDbContext db) =>
        {
            var list = await db.AppointmentPayments
                .Where(p => p.AppointmentId == id && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedUtc)
                .Select(p => new {
                    p.Id,
                    p.Mode,
                    p.ReferenceNumber,
                    p.TotalAmount,
                    p.CreatedUtc,
                    Items = db.AppointmentPaymentItems.Where(i => i.AppointmentPaymentId == p.Id)
                        .Select(i => new { i.Id, i.ServiceId, i.ServiceName, i.Amount })
                        .ToList()
                }).ToListAsync();
            return Results.Ok(list);
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Receptionist","Dentist","Accountant"));

        app.MapGet("/api/patients/{patientId:guid}/payments", async (Guid patientId, Application.Abstractions.IApplicationDbContext db) =>
        {
            var query = from pay in db.AppointmentPayments
                        join appt in db.Appointments on pay.AppointmentId equals appt.Id
                        where appt.PatientProfileId == patientId && !pay.IsDeleted && !appt.IsDeleted
                        orderby pay.CreatedUtc descending
                        select new {
                            pay.Id,
                            pay.Mode,
                            pay.ReferenceNumber,
                            pay.TotalAmount,
                            pay.CreatedUtc,
                            pay.AppointmentId,
                            appt.Date,
                            appt.StartTime,
                            Items = db.AppointmentPaymentItems.Where(i => i.AppointmentPaymentId == pay.Id)
                                .Select(i => new { i.Id, i.ServiceId, i.ServiceName, i.Amount })
                                .ToList()
                        };
            var list = await query.ToListAsync();
            return Results.Ok(list);
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Receptionist","Dentist","Accountant"));

        app.MapGet("/api/patients/{patientId:guid}/appointments/history", async (Guid patientId, Application.Abstractions.IApplicationDbContext db, IEncryptionService enc) =>
        {
            var appointments = await db.Appointments
                .Where(a => a.PatientProfileId == patientId && !a.IsDeleted)
                .OrderByDescending(a => a.Date).ThenByDescending(a => a.StartTime)
                .Select(a => new {
                    a.Id,
                    a.Date,
                    a.StartTime,
                    a.DoctorName,
                    a.Department,
                    a.Reason,
                    a.Notes,
                    a.InvestigationRvg,
                    a.InvestigationOpg,
                    a.InvestigationCeph,
                    a.InvestigationOcclusal,
                    a.InvestigationCbct,
                    a.InvestigationBlood,
                    a.InvestigationOthers,
                    a.DifferentialDiagnosis,
                    a.Diagnosis,
                    a.TreatmentPlan
                })
                .ToListAsync();
            var decrypted = appointments.Select(a => new {
                a.Id,
                a.Date,
                a.StartTime,
                DoctorName = enc.Decrypt(a.DoctorName) ?? a.DoctorName,
                a.Department,
                a.Reason,
                a.Notes,
                a.InvestigationRvg,
                a.InvestigationOpg,
                a.InvestigationCeph,
                a.InvestigationOcclusal,
                a.InvestigationCbct,
                a.InvestigationBlood,
                a.InvestigationOthers,
                a.DifferentialDiagnosis,
                a.Diagnosis,
                a.TreatmentPlan
            }).ToList();
            return Results.Ok(decrypted);
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Receptionist","Dentist","Accountant"));

        return app;
    }
}





