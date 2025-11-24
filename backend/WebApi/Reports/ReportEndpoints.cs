using Microsoft.EntityFrameworkCore;
using Shared.Security;

namespace WebApi.Reports;

public static class ReportEndpoints
{
	public static IEndpointRouteBuilder MapReportEndpoints(this IEndpointRouteBuilder app)
	{
		app.MapGet("/api/reports/service-income", async (string? start, string? end, Application.Abstractions.IApplicationDbContext db, ICurrentUserService current) =>
		{
			var tenantId = current.TenantId ?? Guid.Empty;
			DateOnly s;
			DateOnly e;
			// robust parsing for yyyy-MM-dd
			if (!DateOnly.TryParse(start ?? "", out s))
			{
				try { s = DateOnly.ParseExact(start ?? "", "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture); }
				catch { s = DateOnly.FromDateTime(DateTime.UtcNow); }
			}
			if (!DateOnly.TryParse(end ?? "", out e))
			{
				try { e = DateOnly.ParseExact(end ?? "", "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture); }
				catch { e = s; }
			}
			if (e < s) e = s;

			// interpret requested dates in server's local timezone and convert to UTC range
			var tz = TimeZoneInfo.Local;
			var startLocal = new DateTime(s.Year, s.Month, s.Day, 0, 0, 0, DateTimeKind.Unspecified);
			var endLocal = new DateTime(e.Year, e.Month, e.Day, 23, 59, 59, 999, DateTimeKind.Unspecified);
			var startUtc = TimeZoneInfo.ConvertTimeToUtc(startLocal, tz);
			var endUtc = TimeZoneInfo.ConvertTimeToUtc(endLocal, tz);

			// fetch raw rows within UTC range, then group by local date in-memory
			var raw = await (from pay in db.AppointmentPayments
							 join item in db.AppointmentPaymentItems on pay.Id equals item.AppointmentPaymentId
							 where (tenantId == Guid.Empty || pay.TenantId == tenantId)
							   && !pay.IsDeleted
							   && pay.CreatedUtc >= startUtc
							   && pay.CreatedUtc <= endUtc
							 select new
							 {
								 pay.CreatedUtc,
								 item.ServiceName,
								 item.Amount
							 }).ToListAsync();

			var grouped = raw
				.GroupBy(r => DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(r.CreatedUtc, tz)))
				.Select(g => new
				{
					date = g.Key.ToString("yyyy-MM-dd"),
					services = g.GroupBy(x => x.ServiceName)
								.Select(gg => new { name = gg.Key, amount = gg.Sum(x => x.Amount) })
								.OrderByDescending(x => x.amount)
								.ToList(),
					total = g.Sum(x => x.Amount)
				})
				.OrderBy(x => x.date)
				.ToList();

			return Results.Ok(grouped);
		}).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Accountant","Receptionist","Dentist"));

		return app;
	}
}



