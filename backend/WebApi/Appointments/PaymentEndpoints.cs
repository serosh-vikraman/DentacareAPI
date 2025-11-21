using Microsoft.EntityFrameworkCore;
using Shared.Security;

namespace WebApi.Appointments;

public sealed class PaymentSummaryRequest
{
	public string? Start { get; set; }
	public string? End { get; set; }
}

public static class PaymentEndpoints
{
    public static IEndpointRouteBuilder MapPaymentEndpoints(this IEndpointRouteBuilder app)
    {
		app.MapPost("/api/payments/summary", async (PaymentSummaryRequest req, Application.Abstractions.IApplicationDbContext db, ICurrentUserService current) =>
		{
			var tenantId = current.TenantId ?? Guid.Empty;
			DateOnly start;
			DateOnly end;
			if (!TryParseYmd(req?.Start, out start)) start = DateOnly.FromDateTime(DateTime.UtcNow);
			if (!TryParseYmd(req?.End, out end)) end = start;
			if (end < start) end = start;

			var q = db.AppointmentPayments.Where(p => !p.IsDeleted);
			if (tenantId != Guid.Empty) q = q.Where(p => p.TenantId == tenantId);

			var rows = await q
				.Where(p => DateOnly.FromDateTime(p.CreatedUtc) >= start && DateOnly.FromDateTime(p.CreatedUtc) <= end)
				.GroupBy(p => DateOnly.FromDateTime(p.CreatedUtc))
				.Select(g => new { D = g.Key, Total = g.Sum(x => (decimal?)x.TotalAmount) ?? 0m })
				.ToListAsync();

			var map = rows.ToDictionary(r => r.D, r => r.Total);
			var list = new List<object>();
			for (var d = start; d <= end; d = d.AddDays(1))
			{
				map.TryGetValue(d, out var total);
				list.Add(new { date = d.ToString("yyyy-MM-dd"), amount = total });
			}
			return Results.Ok(list);
		}).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Accountant","Receptionist","Dentist"));

		app.MapGet("/api/payments/today", async (Application.Abstractions.IApplicationDbContext db, ICurrentUserService current) =>
        {
            var tenantId = current.TenantId ?? Guid.Empty;
            var todayUtc = DateTime.UtcNow.Date;
			IQueryable<Domain.Appointments.AppointmentPayment> q = db.AppointmentPayments.Where(p => !p.IsDeleted && p.CreatedUtc.Date == todayUtc);
			if (tenantId != Guid.Empty) q = q.Where(p => p.TenantId == tenantId);
			var sum = await q.SumAsync(p => (decimal?)p.TotalAmount) ?? 0m;
            return Results.Ok(sum);
		}).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Accountant","Receptionist","Dentist"));

		app.MapGet("/api/payments", async (string? date, Application.Abstractions.IApplicationDbContext db, ICurrentUserService current) =>
        {
			var tenantId = current.TenantId ?? Guid.Empty;
			if (!string.IsNullOrWhiteSpace(date))
            {
				DateOnly d;
				if (!DateOnly.TryParse(date, out d))
				{
					try
					{
						d = DateOnly.ParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
					}
					catch
					{
						return Results.BadRequest(new { message = "Invalid date. Expected format yyyy-MM-dd" });
					}
				}
				IQueryable<Domain.Appointments.AppointmentPayment> q = db.AppointmentPayments
					.Where(p => !p.IsDeleted && DateOnly.FromDateTime(p.CreatedUtc) == d);
				if (tenantId != Guid.Empty) q = q.Where(p => p.TenantId == tenantId);
				var sum = await q.SumAsync(p => (decimal?)p.TotalAmount) ?? 0m;
				return Results.Ok(sum);
            }
            // Fallback: return recent payments list (last 50)
			IQueryable<Domain.Appointments.AppointmentPayment> q2 = db.AppointmentPayments.Where(p => !p.IsDeleted);
			if (tenantId != Guid.Empty) q2 = q2.Where(p => p.TenantId == tenantId);
			var list = await q2
                .OrderByDescending(p => p.CreatedUtc)
                .Take(50)
                .Select(p => new { p.Id, p.Mode, p.ReferenceNumber, p.TotalAmount, p.CreatedUtc, p.AppointmentId })
                .ToListAsync();
            return Results.Ok(list);
		}).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Accountant","Receptionist","Dentist"));

		return app;
    }

	private static bool TryParseYmd(string? s, out DateOnly d)
	{
		if (DateOnly.TryParse(s ?? "", out d)) return true;
		try
		{
			d = DateOnly.ParseExact(s ?? "", "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
			return true;
		}
		catch
		{
			d = default;
			return false;
		}
	}
}



