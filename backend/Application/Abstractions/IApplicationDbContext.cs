using Domain.Patients;
using Domain.Appointments;
using Domain.Doctors;
using Domain.Staff;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Patient> Patients { get; }
    DbSet<PatientProfile> PatientProfiles { get; }
    DbSet<DoctorProfile> DoctorProfiles { get; }
    DbSet<StaffProfile> StaffProfiles { get; }
    DbSet<Appointment> Appointments { get; }
    DbSet<Domain.Services.Service> Services { get; }
    DbSet<AppointmentPayment> AppointmentPayments { get; }
    DbSet<AppointmentPaymentItem> AppointmentPaymentItems { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}






