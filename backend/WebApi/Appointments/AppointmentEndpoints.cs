using Application.Appointments.Commands;
using Application.Appointments.Dtos;
using Application.Appointments.Queries;
using MediatR;

namespace WebApi.Appointments;

public static class AppointmentEndpoints
{
    public static IEndpointRouteBuilder MapAppointmentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/appointments", async (string? q, IMediator mediator) =>
        {
            var list = await mediator.Send(new ListAppointmentsQuery(q));
            return Results.Ok(list);
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Dentist","Receptionist","Accountant"));

        app.MapGet("/api/appointments/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var one = await mediator.Send(new GetAppointmentQuery(id));
            return one != null ? Results.Ok(one) : Results.NotFound();
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Dentist","Receptionist","Accountant"));

        app.MapPost("/api/appointments", async (CreateAppointmentRequest req, IMediator mediator) =>
        {
            var id = await mediator.Send(new CreateAppointmentCommand(req));
            return Results.Created($"/api/appointments/{id}", new { id });
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

        return app;
    }
}





