using Domain.Patients;
using Application.Abstractions;
using Domain.Appointments;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared.Tenant;
using Shared.Security;
using Infrastructure.Audit;

namespace Infrastructure.Data;

public class DentaCareDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IApplicationDbContext
{
    private readonly ITenantProvider _tenantProvider;
    private readonly IEncryptionService _encryptionService;
    private readonly Shared.Security.ICurrentUserService _currentUserService;

    public DentaCareDbContext(DbContextOptions<DentaCareDbContext> options, ITenantProvider tenantProvider, IEncryptionService encryptionService, Shared.Security.ICurrentUserService currentUserService)
        : base(options)
    {
        _tenantProvider = tenantProvider;
        _encryptionService = encryptionService;
        _currentUserService = currentUserService;
    }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Domain.Patients.PatientProfile> PatientProfiles => Set<Domain.Patients.PatientProfile>();
    public DbSet<Domain.Doctors.DoctorProfile> DoctorProfiles => Set<Domain.Doctors.DoctorProfile>();
    public DbSet<Domain.Staff.StaffProfile> StaffProfiles => Set<Domain.Staff.StaffProfile>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Domain.Services.Service> Services => Set<Domain.Services.Service>();
    public DbSet<AppointmentPayment> AppointmentPayments => Set<AppointmentPayment>();
    public DbSet<AppointmentPaymentItem> AppointmentPaymentItems => Set<AppointmentPaymentItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(320);
            entity.HasIndex(e => new { e.TenantId, e.BranchId });
            entity.HasIndex(e => new { e.TenantId, e.LastName, e.FirstName });
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.EntityName).HasMaxLength(200);
            entity.Property(x => x.Action).HasMaxLength(32);
            entity.HasIndex(x => x.TimestampUtc);
            entity.HasIndex(x => new { x.TenantId, x.EntityName });
        });

        modelBuilder.Entity<Domain.Patients.PatientProfile>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.PatientName).HasMaxLength(256).IsRequired();
            entity.Property(x => x.MRNumber).HasMaxLength(32).IsRequired();
            entity.Property(x => x.EmergencyContactName).HasMaxLength(128);
            entity.Property(x => x.EmergencyContactPhone).HasMaxLength(32);
            entity.HasIndex(x => new { x.TenantId, x.PatientName });
            entity.HasIndex(x => new { x.TenantId, x.MRNumber }).IsUnique();
            entity.Property(x => x.MucosalNotes).HasMaxLength(2048);
            entity.Property(x => x.TMJNotes).HasMaxLength(2048);
            entity.Property(x => x.OcclusionNotes).HasMaxLength(2048);
            entity.Property(x => x.IntraExtraNotes).HasMaxLength(2048);
            entity.Property(x => x.ContinuousSpO2).HasMaxLength(128);
            entity.Property(x => x.HeartRateECG).HasMaxLength(128);
            entity.Property(x => x.SalivaPHFlow).HasMaxLength(128);
        });

        modelBuilder.Entity<Domain.Doctors.DoctorProfile>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FullName).HasMaxLength(256).IsRequired();
            entity.Property(x => x.DoctorId).HasMaxLength(32).IsRequired();
            entity.HasIndex(x => new { x.TenantId, x.DoctorId }).IsUnique();
            entity.HasIndex(x => new { x.TenantId, x.FullName });
        });

        modelBuilder.Entity<Domain.Staff.StaffProfile>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FullName).HasMaxLength(256).IsRequired();
            entity.Property(x => x.StaffId).HasMaxLength(32).IsRequired();
            entity.HasIndex(x => new { x.TenantId, x.StaffId }).IsUnique();
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.PatientName).HasMaxLength(256).IsRequired();
            entity.Property(x => x.PatientMRNumber).HasMaxLength(32);
            entity.Property(x => x.Department).HasMaxLength(128).IsRequired();
            entity.Property(x => x.DoctorName).HasMaxLength(256).IsRequired();
            entity.Property(x => x.ConsultMode).HasMaxLength(32).IsRequired();
            entity.Property(x => x.PaymentMode).HasMaxLength(32);
            entity.Property(x => x.Status).HasMaxLength(32).IsRequired();
            entity.Property(x => x.InvestigationRvg).HasMaxLength(256);
            entity.Property(x => x.InvestigationBlood).HasMaxLength(256);
            entity.Property(x => x.InvestigationOthers).HasMaxLength(4096);
            entity.Property(x => x.DifferentialDiagnosis).HasMaxLength(4096);
            entity.Property(x => x.Diagnosis).HasMaxLength(4096);
            entity.Property(x => x.TreatmentPlan).HasMaxLength(4096);
            entity.HasIndex(x => new { x.TenantId, x.Date });
        });

        modelBuilder.Entity<Domain.Services.Service>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(256).IsRequired();
            entity.Property(x => x.Category).HasMaxLength(128);
            entity.HasIndex(x => new { x.TenantId, x.Name });
        });

        modelBuilder.Entity<AppointmentPayment>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Mode).HasMaxLength(32).IsRequired();
            entity.HasMany(x => x.Items).WithOne().HasForeignKey(i => i.AppointmentPaymentId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => new { x.TenantId, x.AppointmentId });
        });

        modelBuilder.Entity<AppointmentPaymentItem>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ServiceName).HasMaxLength(256).IsRequired();
        });

        // global tenant filter
        modelBuilder.Entity<Patient>().HasQueryFilter(p => p.TenantId == _tenantProvider.TenantId && !p.IsDeleted);
    }

    public override int SaveChanges()
    {
        ApplySensitiveEncryption();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplySensitiveEncryption();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplySensitiveEncryption()
    {
        foreach (var entry in ChangeTracker.Entries<Patient>())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                var encEmail = _encryptionService.Encrypt(entry.Entity.Email);
                var encPhone = _encryptionService.Encrypt(entry.Entity.Phone);
                typeof(Patient).GetProperty("Email")!.SetValue(entry.Entity, encEmail);
                typeof(Patient).GetProperty("Phone")!.SetValue(entry.Entity, encPhone);
            }
            if (entry.State == EntityState.Unchanged)
            {
                // noop
            }
        }
        foreach (var entry in ChangeTracker.Entries<Domain.Patients.PatientProfile>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.Email = _encryptionService.Encrypt(entry.Entity.Email);
                entry.Entity.Mobile = _encryptionService.Encrypt(entry.Entity.Mobile);
                entry.Entity.AltPhone = _encryptionService.Encrypt(entry.Entity.AltPhone);
            }
            else if (entry.State == EntityState.Modified)
            {
                if (entry.Property(nameof(Domain.Patients.PatientProfile.Email)).IsModified)
                    entry.Entity.Email = _encryptionService.Encrypt(entry.Entity.Email);
                if (entry.Property(nameof(Domain.Patients.PatientProfile.Mobile)).IsModified)
                    entry.Entity.Mobile = _encryptionService.Encrypt(entry.Entity.Mobile);
                if (entry.Property(nameof(Domain.Patients.PatientProfile.AltPhone)).IsModified)
                    entry.Entity.AltPhone = _encryptionService.Encrypt(entry.Entity.AltPhone);
            }
        }

        foreach (var entry in ChangeTracker.Entries<Domain.Doctors.DoctorProfile>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.FullName = _encryptionService.Encrypt(entry.Entity.FullName);
                entry.Entity.Email = _encryptionService.Encrypt(entry.Entity.Email);
                entry.Entity.ContactNumber = _encryptionService.Encrypt(entry.Entity.ContactNumber);
            }
            else if (entry.State == EntityState.Modified)
            {
                if (entry.Property(nameof(Domain.Doctors.DoctorProfile.FullName)).IsModified)
                    entry.Entity.FullName = _encryptionService.Encrypt(entry.Entity.FullName);
                if (entry.Property(nameof(Domain.Doctors.DoctorProfile.Email)).IsModified)
                    entry.Entity.Email = _encryptionService.Encrypt(entry.Entity.Email);
                if (entry.Property(nameof(Domain.Doctors.DoctorProfile.ContactNumber)).IsModified)
                    entry.Entity.ContactNumber = _encryptionService.Encrypt(entry.Entity.ContactNumber);
            }
        }

        foreach (var entry in ChangeTracker.Entries<Domain.Staff.StaffProfile>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.Email = _encryptionService.Encrypt(entry.Entity.Email);
                entry.Entity.ContactNumber = _encryptionService.Encrypt(entry.Entity.ContactNumber);
                entry.Entity.FullName = _encryptionService.Encrypt(entry.Entity.FullName);
            }
            else if (entry.State == EntityState.Modified)
            {
                if (entry.Property(nameof(Domain.Staff.StaffProfile.Email)).IsModified)
                    entry.Entity.Email = _encryptionService.Encrypt(entry.Entity.Email);
                if (entry.Property(nameof(Domain.Staff.StaffProfile.ContactNumber)).IsModified)
                    entry.Entity.ContactNumber = _encryptionService.Encrypt(entry.Entity.ContactNumber);
                if (entry.Property(nameof(Domain.Staff.StaffProfile.FullName)).IsModified)
                    entry.Entity.FullName = _encryptionService.Encrypt(entry.Entity.FullName);
            }
        }

        foreach (var entry in ChangeTracker.Entries<Appointment>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.PatientName = _encryptionService.Encrypt(entry.Entity.PatientName);
                entry.Entity.DoctorName = _encryptionService.Encrypt(entry.Entity.DoctorName);
            }
            else if (entry.State == EntityState.Modified)
            {
                if (entry.Property(nameof(Appointment.PatientName)).IsModified)
                    entry.Entity.PatientName = _encryptionService.Encrypt(entry.Entity.PatientName);
                if (entry.Property(nameof(Appointment.DoctorName)).IsModified)
                    entry.Entity.DoctorName = _encryptionService.Encrypt(entry.Entity.DoctorName);
            }
        }

        foreach (var entry in ChangeTracker.Entries<Infrastructure.Identity.ApplicationUser>())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                typeof(Infrastructure.Identity.ApplicationUser).GetProperty("FullName")?.SetValue(entry.Entity, _encryptionService.Encrypt(entry.Entity.FullName));
            }
        }

        var auditEntries = new List<AuditLog>();
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is Patient p)
            {
                var action = entry.State switch
                {
                    EntityState.Added => "Created",
                    EntityState.Modified => "Updated",
                    EntityState.Deleted => "Deleted",
                    _ => null
                };
                if (action != null)
                {
                    auditEntries.Add(new AuditLog
                    {
                        TenantId = p.TenantId,
                        EntityName = nameof(Patient),
                        EntityId = p.Id,
                        Action = action,
                        UserId = _currentUserService.UserId
                    });
                }
            }
        }
        if (auditEntries.Count > 0)
        {
            AuditLogs.AddRange(auditEntries);
        }
    }
}


