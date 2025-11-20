using Microsoft.EntityFrameworkCore;
using Shared.Security;

namespace WebApi.Appointments;

public static class PaymentEndpoints
{
    public static IEndpointRouteBuilder MapPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/payments/today", async (Application.Abstractions.IApplicationDbContext db, ICurrentUserService current) =>
        {
            var tenantId = current.TenantId ?? Guid.Empty;
            var todayUtc = DateTime.UtcNow.Date;
            var sum = await db.AppointmentPayments
                .Where(p => p.TenantId == tenantId && !p.IsDeleted && p.CreatedUtc.Date == todayUtc)
                .SumAsync(p => (decimal?)p.TotalAmount) ?? 0m;
            return Results.Ok(sum);
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Accountant","Receptionist"));

        app.MapGet("/api/payments", async (string? date, Application.Abstractions.IApplicationDbContext db, ICurrentUserService current) =>
        {
            var tenantId = current.TenantId ?? Guid.Empty;
            if (!string.IsNullOrWhiteSpace(date) && DateOnly.TryParse(date, out var d))
            {
                // Compare by UTC date of creation
                var sum = await db.AppointmentPayments
                    .Where(p => p.TenantId == tenantId && !p.IsDeleted && DateOnly.FromDateTime(p.CreatedUtc) == d)
                    .SumAsync(p => (decimal?)p.TotalAmount) ?? 0m;
                return Results.Ok(sum);
            }
            // Fallback: return recent payments list (last 50)
            var list = await db.AppointmentPayments
                .Where(p => p.TenantId == tenantId && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedUtc)
                .Take(50)
                .Select(p => new { p.Id, p.Mode, p.ReferenceNumber, p.TotalAmount, p.CreatedUtc, p.AppointmentId })
                .ToListAsync();
            return Results.Ok(list);
        }).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Accountant","Receptionist"));

        return app;
    }
}



