using Application.Abstractions;
using Application.Appointments.Dtos;
using Domain.Appointments;
using MediatR;

namespace Application.Appointments.Commands;

public sealed record SavePaymentCommand(SavePaymentRequest Request) : IRequest<Guid>;

public sealed class SavePaymentHandler : IRequestHandler<SavePaymentCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    private readonly Shared.Tenant.ITenantProvider _tenant;

    public SavePaymentHandler(IApplicationDbContext db, Shared.Tenant.ITenantProvider tenant)
    { _db = db; _tenant = tenant; }

    public async Task<Guid> Handle(SavePaymentCommand request, CancellationToken cancellationToken)
    {
        var r = request.Request;
        var pay = new AppointmentPayment
        {
            AppointmentId = r.AppointmentId,
            TenantId = _tenant.TenantId,
            BranchId = _tenant.BranchId,
            Mode = r.Mode,
            ReferenceNumber = r.ReferenceNumber,
            TotalAmount = r.TotalAmount
        };
        foreach (var it in r.Items)
        {
            pay.Items.Add(new AppointmentPaymentItem
            {
                ServiceId = it.ServiceId,
                ServiceName = it.ServiceName,
                Amount = it.Amount
            });
        }
        _db.AppointmentPayments.Add(pay);
        await _db.SaveChangesAsync(cancellationToken);
        return pay.Id;
    }
}



