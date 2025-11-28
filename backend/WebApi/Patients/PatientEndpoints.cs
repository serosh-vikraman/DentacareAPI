using Application.Patients.Commands;
using Application.Patients.Dtos;
using Application.Patients.Queries;
using MediatR;
using Shared.Security;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Patients;

public static class PatientEndpoints
{
    public static IEndpointRouteBuilder MapPatientEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/patients", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new ListPatientsQuery());
            return Results.Ok(result);
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Dentist","Receptionist","Accountant"));

        app.MapPost("/api/patients", async (CreatePatientRequest req, IMediator mediator) =>
        {
            var id = await mediator.Send(new CreatePatientCommand(req));
            return Results.Created($"/api/patients/{id}", new { id });
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Receptionist"));

        app.MapPut("/api/patients/{id:guid}", async (Guid id, UpdatePatientRequest req, IMediator mediator) =>
        {
            var ok = await mediator.Send(new UpdatePatientCommand(id, req));
            return ok ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Receptionist"));

        app.MapDelete("/api/patients/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var ok = await mediator.Send(new DeletePatientCommand(id));
            return ok ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner"));

        app.MapPost("/api/patients/profile", async (Guid? id, SavePatientProfileRequest req, IMediator mediator, ICurrentUserService current) =>
        {
            if (id.HasValue && id.Value != Guid.Empty) { req.Id = id; }
            var savedId = await mediator.Send(new SavePatientProfileCommand(req, current.TenantId ?? Guid.Empty, current.BranchId));
            var wasUpdate = req.Id.HasValue && req.Id.Value != Guid.Empty;
            return Results.Ok(new { id = savedId, message = (wasUpdate ? "Updated successfully" : "Saved successfully") });
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Receptionist","Dentist"));

        app.MapPut("/api/patients/profile/{id:guid}", async (Guid id, SavePatientProfileRequest req, IMediator mediator, ICurrentUserService current) =>
        {
            // force the request to be an update for the given id
            req.Id = id;
            var updatedId = await mediator.Send(new SavePatientProfileCommand(req, current.TenantId ?? Guid.Empty, current.BranchId));
            return Results.Ok(new { id = updatedId, message = "Updated successfully" });
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Receptionist","Dentist"));

        app.MapGet("/api/patient-profiles", async (string? q, int? page_number, int? page_size, IMediator mediator) =>
        {
            var list = await mediator.Send(new ListPatientProfilesQuery(q, page_number ?? 1, page_size ?? 20));
            return Results.Ok(list);
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Receptionist","Dentist","Accountant"));

        app.MapGet("/api/patient-profiles/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var one = await mediator.Send(new GetPatientProfileQuery(id));
            return one != null ? Results.Ok(one) : Results.NotFound();
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Receptionist","Dentist","Accountant"));

		// Stats: patients registered today (tenant-scoped)
		app.MapGet("/api/patient-profiles/stats/today", async (Application.Abstractions.IApplicationDbContext db, ICurrentUserService current) =>
		{
			var tenantId = current.TenantId ?? Guid.Empty;
			var startUtc = DateTime.UtcNow.Date;
			var endUtc = startUtc.AddDays(1);
			var count = await db.PatientProfiles
				.Where(p => p.TenantId == tenantId && !p.IsDeleted && p.CreatedUtc >= startUtc && p.CreatedUtc < endUtc)
				.CountAsync();
			return Results.Ok(new { count });
		}).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Receptionist","Dentist","Accountant"));

        app.MapDelete("/api/patient-profiles/{id:guid}", async (Guid id, Application.Abstractions.IApplicationDbContext db) =>
        {
            var p = await db.PatientProfiles.FirstOrDefaultAsync(x => x.Id == id);
            if (p == null) return Results.NotFound();
            var hasAppointments = await db.Appointments.AnyAsync(a => a.PatientProfileId == id && !a.IsDeleted);
            if (hasAppointments) return Results.Conflict(new { message = "Cannot delete: patient has appointments" });
            p.IsDeleted = true;
            await db.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner"));

        return app;
    }
}


