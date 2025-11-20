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
			if (!DateOnly.TryParse(start ?? "", out var s)) s = DateOnly.FromDateTime(DateTime.UtcNow);
			if (!DateOnly.TryParse(end ?? "", out var e)) e = s;
			if (e < s) e = s;

			var rows = await (from pay in db.AppointmentPayments
							  join item in db.AppointmentPaymentItems on pay.Id equals item.AppointmentPaymentId
							  where pay.TenantId == tenantId
								&& !pay.IsDeleted
								&& DateOnly.FromDateTime(pay.CreatedUtc) >= s
								&& DateOnly.FromDateTime(pay.CreatedUtc) <= e
							  group item by new { D = DateOnly.FromDateTime(pay.CreatedUtc), item.ServiceName } into g
							  orderby g.Key.D
							  select new
							  {
								  Date = g.Key.D,
								  Service = g.Key.ServiceName,
								  Amount = g.Sum(x => (decimal?)x.Amount) ?? 0m
							  }).ToListAsync();

			var grouped = rows
				.GroupBy(r => r.Date)
				.Select(g => new
				{
					date = g.Key.ToString("yyyy-MM-dd"),
					services = g.Select(x => new { name = x.Service, amount = x.Amount }).OrderByDescending(x => x.amount).ToList(),
					total = g.Sum(x => x.Amount)
				})
				.OrderBy(x => x.date)
				.ToList();

			return Results.Ok(grouped);
		}).RequireAuthorization(policy => policy.RequireRole("Admin","Owner","Accountant","Receptionist"));

		return app;
	}
}



