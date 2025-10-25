using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Identity;

public sealed class IdentitySeeder
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<IdentitySeeder> _logger;

    public static readonly string[] DefaultRoles = new[]
    {
        "Owner","Admin","Dentist","Receptionist","Accountant","FrontDeskAdmin","NursingAssistant"
    };

    public IdentitySeeder(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, ILogger<IdentitySeeder> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        foreach (var role in DefaultRoles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = role });
            }
        }

        var adminEmail = "admin@dentacare.local";
        var admin = await _userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new ApplicationUser
            {
                Id = Guid.Parse("00000000-0000-0000-0000-0000000000AA"),
                Email = adminEmail,
                UserName = adminEmail,
                EmailConfirmed = true,
                FullName = "System Admin",
                TenantId = Guid.Parse("00000000-0000-0000-0000-000000000001")
            };
            var result = await _userManager.CreateAsync(admin, "Admin@12345");
            if (!result.Succeeded)
            {
                _logger.LogWarning("Admin user creation failed: {Errors}", string.Join(",", result.Errors.Select(e => e.Description)));
                return;
            }
        }

        foreach (var role in DefaultRoles)
        {
            if (!await _userManager.IsInRoleAsync(admin, role))
            {
                await _userManager.AddToRoleAsync(admin, role);
            }
        }
    }
}






