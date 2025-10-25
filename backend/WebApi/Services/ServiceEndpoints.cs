using Application.Services.Queries;
using MediatR;

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

        return app;
    }
}




