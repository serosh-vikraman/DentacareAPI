using Application.Services.Queries;
using MediatR;
using Application.Services.Commands;

namespace WebApi.Services;

public static class ServiceEndpoints
{
    public static IEndpointRouteBuilder MapServiceEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/services", async (string? q, IMediator mediator) =>
        {
            var list = await mediator.Send(new ListServicesQuery(q));
            return Results.Ok(list);
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Dentist","Receptionist","Accountant"));

        app.MapPost("/api/services", async (CreateServiceRequest req, IMediator mediator) =>
        {
            var id = await mediator.Send(new CreateServiceCommand(
                req.Name ?? string.Empty,
                req.Amount ?? req.Price ?? 0,
                req.Category,
                req.Description,
                req.IsActive,
                req.IsTaxable,
                req.TaxRate
            ));
            return Results.Created($"/api/services/{id}", new { id });
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Accountant"));

        app.MapPut("/api/services/{id:guid}", async (Guid id, UpdateServiceRequest req, IMediator mediator) =>
        {
            var ok = await mediator.Send(new UpdateServiceCommand(
                id,
                req.Name ?? string.Empty,
                req.Amount ?? req.Price ?? 0,
                req.Category,
                req.Description,
                req.IsActive,
                req.IsTaxable,
                req.TaxRate
            ));
            return ok ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Accountant"));

        app.MapDelete("/api/services/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var ok = await mediator.Send(new DeleteServiceCommand(id));
            return ok ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Accountant"));

        return app;
    }
}

public sealed record CreateServiceRequest
{
    public string? Name { get; set; }
    public decimal? Amount { get; set; }
    public decimal? Price { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsTaxable { get; set; }
    public decimal? TaxRate { get; set; }
}

public sealed record UpdateServiceRequest
{
    public string? Name { get; set; }
    public decimal? Amount { get; set; }
    public decimal? Price { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsTaxable { get; set; }
    public decimal? TaxRate { get; set; }
}




