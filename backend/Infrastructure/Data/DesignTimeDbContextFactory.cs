using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DentaCareDbContext>
{
    public DentaCareDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DentaCare") ??
                                "server=127.0.0.1;port=3306;database=DentaCare;user id=root;password=root";

        var optionsBuilder = new DbContextOptionsBuilder<DentaCareDbContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        // Use a dummy tenant provider at design-time
        var tenantProvider = new DesignTimeTenantProvider();
        var encryption = new DesignTimeEncryptionService();
        var currentUser = new DesignTimeCurrentUserService();
        return new DentaCareDbContext(optionsBuilder.Options, tenantProvider, encryption, currentUser);
    }

    private sealed class DesignTimeTenantProvider : Shared.Tenant.ITenantProvider
    {
        public Guid TenantId => Guid.Parse("00000000-0000-0000-0000-000000000001");
        public Guid? BranchId => null;
    }
}

internal sealed class DesignTimeEncryptionService : Shared.Security.IEncryptionService
{
    public string? Encrypt(string? plaintext) => plaintext;
    public string? Decrypt(string? ciphertext) => ciphertext;
}

internal sealed class DesignTimeCurrentUserService : Shared.Security.ICurrentUserService
{
    public Guid? UserId => null;
    public Guid? TenantId => Guid.Parse("00000000-0000-0000-0000-000000000001");
    public Guid? BranchId => null;
}


