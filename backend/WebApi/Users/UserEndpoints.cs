using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Shared.Security;
using Domain.Doctors;
using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Application.Doctors.Queries;

namespace WebApi.Users;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").RequireAuthorization();

        group.MapGet("/doctors", [Authorize(Roles = "Admin,Owner,Dentist,Receptionist,Accountant")] async (IMediator mediator, string? q) =>
        {
            var list = await mediator.Send(new ListDoctorProfilesQuery(q));
            return Results.Ok(list);
        });

		group.MapGet("/staff", [Authorize(Roles = "Admin,Owner")] async (UserManager<ApplicationUser> users, RoleManager<ApplicationRole> roles, Shared.Security.IEncryptionService enc, IApplicationDbContext db) =>
        {
			var all = users.Users.ToList();
            var doctors = await users.GetUsersInRoleAsync("Dentist");
            var adminId = "admin@dentacare.local";
			// Build profile lookup for names (some historical records may have encrypted names on profile or user)
			var userIds = all.Select(u => u.Id).ToList();
			var profiles = await db.StaffProfiles
				.Where(p => userIds.Contains(p.UserId))
				.Select(p => new { p.UserId, p.FullName })
				.ToListAsync();
			var nameByUserId = profiles.ToDictionary(p => p.UserId, p => p.FullName);
			string ResolveName(ApplicationUser u)
			{
				// Prefer profile full name (decrypt if needed), then user full name (decrypt if needed)
				if (nameByUserId.TryGetValue(u.Id, out var pfName))
				{
					var dec = enc.Decrypt(pfName);
					if (!string.IsNullOrWhiteSpace(dec)) return dec!;
					if (!string.IsNullOrWhiteSpace(pfName)) return pfName!;
				}
				var uDec = enc.Decrypt(u.FullName);
				if (!string.IsNullOrWhiteSpace(uDec)) return uDec!;
				return u.FullName;
			}
			var list = all
				.Where(u => u.Email != adminId && !doctors.Any(d => d.Id == u.Id))
				.Select(u => new { u.Id, FullName = ResolveName(u), u.Email, u.PhoneNumber, u.PhotoUrl, u.Designation })
				.ToList();
            return Results.Ok(list);
        });

        group.MapPost("/staff", [Authorize(Roles = "Admin,Owner")] async (UserManager<ApplicationUser> users, RoleManager<ApplicationRole> roles, IApplicationDbContext db, ICurrentUserService current, 
            string fullName, string email, string phone, string password, string designation, string role) =>
        {
            var u = new ApplicationUser { Id = Guid.NewGuid(), UserName = email, Email = email, PhoneNumber = phone, FullName = fullName, Designation = designation, TenantId = current.TenantId ?? Guid.Empty };
            var result = await users.CreateAsync(u, password);
            if (!result.Succeeded) return Results.BadRequest(result.Errors);
            await users.AddToRoleAsync(u, role);
            var next = (await db.StaffProfiles.CountAsync()) + 1;
            db.StaffProfiles.Add(new Domain.Staff.StaffProfile
            {
                UserId = u.Id,
                TenantId = current.TenantId ?? Guid.Empty,
                StaffId = next.ToString("D6"),
                FullName = fullName,
                PhotoUrl = u.PhotoUrl,
                ContactNumber = phone,
                Email = email,
                Address = null,
                Role = role,
                Designation = designation
            });
            await db.SaveChangesAsync();
            return Results.Ok(new { id = u.Id });
        });

        group.MapPut("/staff/{id:guid}", [Authorize(Roles = "Admin,Owner")] async (Guid id, UserManager<ApplicationUser> users, IApplicationDbContext db, 
            string fullName, string email, string phone, string designation, string role) =>
        {
            var u = await users.FindByIdAsync(id.ToString());
            if (u == null) return Results.NotFound();
            u.FullName = fullName; u.Email = email; u.UserName = email; u.PhoneNumber = phone; u.Designation = designation;
            var r = await users.UpdateAsync(u);
            if (!r.Succeeded) return Results.BadRequest(r.Errors);
            var p = await db.StaffProfiles.FirstOrDefaultAsync(s => s.UserId == u.Id);
            if (p != null)
            {
                p.FullName = fullName; p.ContactNumber = phone; p.Email = email; p.Role = role; p.Designation = designation;
                await db.SaveChangesAsync();
            }
            return Results.NoContent();
        });

        group.MapDelete("/staff/{id:guid}", [Authorize(Roles = "Admin,Owner")] async (Guid id, UserManager<ApplicationUser> users, IApplicationDbContext db) =>
        {
            var u = await users.FindByIdAsync(id.ToString());
            if (u == null) return Results.NotFound();
            var p = await db.StaffProfiles.FirstOrDefaultAsync(s => s.UserId == u.Id);
            if (p != null) { db.StaffProfiles.Remove(p); await db.SaveChangesAsync(); }
            var r = await users.DeleteAsync(u);
            if (!r.Succeeded) return Results.BadRequest(r.Errors);
            return Results.NoContent();
        });

        group.MapPost("/doctors", [Authorize(Roles = "Admin,Owner")] async (UserManager<ApplicationUser> users, RoleManager<ApplicationRole> roles, IApplicationDbContext db, ICurrentUserService current, DoctorCreateRequest req) =>
        {
            var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = req.Email, Email = req.Email, PhoneNumber = req.Phone, FullName = req.FullName, Specialty = req.Specialty, PhotoUrl = req.PhotoUrl, TenantId = current.TenantId ?? Guid.Empty };
            var result = await users.CreateAsync(user, req.Password);
            if(!result.Succeeded) return Results.BadRequest(result.Errors);
            await users.AddToRoleAsync(user, "Dentist");
            var next = (await db.DoctorProfiles.CountAsync()) + 1;
            var profile = new DoctorProfile
            {
                UserId = user.Id,
                TenantId = current.TenantId ?? Guid.Empty,
                DoctorId = next.ToString("D6"),
                FullName = req.FullName,
                Gender = req.Gender,
                DateOfBirth = ParseDate(req.Dob),
                PhotoUrl = req.PhotoUrl,
                ContactNumber = req.Phone,
                Email = req.Email,
                Address = req.Address,
                EmergencyContactName = req.EmergencyName,
                EmergencyContactRelation = req.EmergencyRelation,
                EmergencyContactPhone = req.EmergencyPhone,
                Specialization = req.Specialty,
                Qualifications = req.Qualifications,
                MedicalRegistrationNumber = req.Regno,
                YearsOfExperience = req.Experience
            };
            db.DoctorProfiles.Add(profile);
            await db.SaveChangesAsync();
            return Results.Ok(new { id = profile.Id });
        });

        // Get doctor detail by User Id
        group.MapGet("/doctors/{id:guid}", [Authorize(Roles = "Admin,Owner,Dentist,Receptionist,Accountant")] async (Guid id, UserManager<ApplicationUser> users, IApplicationDbContext db, IEncryptionService enc) =>
        {
            var user = await users.FindByIdAsync(id.ToString());
            if (user == null) return Results.NotFound();
            var profile = await db.DoctorProfiles.AsNoTracking().FirstOrDefaultAsync(d => d.UserId == user.Id);
            return Results.Ok(new {
                id = profile?.Id,
                userId = user.Id,
                fullName = enc.Decrypt(profile?.FullName) ?? user.FullName,
                email = user.Email,
                phone = user.PhoneNumber,
                specialty = profile?.Specialization ?? user.Specialty,
                gender = profile?.Gender,
                dob = profile?.DateOfBirth,
                photoUrl = user.PhotoUrl ?? profile?.PhotoUrl,
                address = profile?.Address,
                emergencyName = profile?.EmergencyContactName,
                emergencyRelation = profile?.EmergencyContactRelation,
                emergencyPhone = profile?.EmergencyContactPhone,
                qualifications = profile?.Qualifications,
                regno = profile?.MedicalRegistrationNumber,
                experience = profile?.YearsOfExperience
            });
        });

        group.MapPut("/doctors/{id:guid}", [Authorize(Roles = "Admin,Owner")] async (Guid id, UserManager<ApplicationUser> users, IApplicationDbContext db, DoctorUpdateRequest req) =>
        {
            var user = await users.FindByIdAsync(id.ToString());
            if(user == null) return Results.NotFound();
            user.FullName = req.FullName; user.Email = req.Email; user.UserName = req.Email; user.PhoneNumber = req.Phone; user.Specialty = req.Specialty; user.PhotoUrl = req.PhotoUrl;
            var ur = await users.UpdateAsync(user);
            if(!ur.Succeeded) return Results.BadRequest(ur.Errors);
            if (!string.IsNullOrWhiteSpace(req.Password))
            {
                var hasPwd = await users.HasPasswordAsync(user);
                IdentityResult pr;
                if (hasPwd)
                {
                    // Reset password (admin flow)
                    await users.RemovePasswordAsync(user);
                    pr = await users.AddPasswordAsync(user, req.Password);
                }
                else
                {
                    pr = await users.AddPasswordAsync(user, req.Password);
                }
                if (!pr.Succeeded) return Results.BadRequest(pr.Errors);
            }
            var profile = await db.DoctorProfiles.FirstOrDefaultAsync(d => d.UserId == user.Id);
            if(profile != null){
                profile.FullName = req.FullName; profile.Gender = req.Gender; profile.DateOfBirth = ParseDate(req.Dob); profile.PhotoUrl = req.PhotoUrl; profile.ContactNumber = req.Phone; profile.Email = req.Email; profile.Address = req.Address; profile.EmergencyContactName = req.EmergencyName; profile.EmergencyContactRelation = req.EmergencyRelation; profile.EmergencyContactPhone = req.EmergencyPhone; profile.Specialization = req.Specialty; profile.Qualifications = req.Qualifications; profile.MedicalRegistrationNumber = req.Regno; profile.YearsOfExperience = req.Experience;
                await db.SaveChangesAsync();
            }
            return Results.NoContent();
        });

        group.MapDelete("/doctors/{id:guid}", [Authorize(Roles = "Admin,Owner")] async (Guid id, UserManager<ApplicationUser> users, IApplicationDbContext db) =>
        {
            var user = await users.FindByIdAsync(id.ToString());
            if(user == null) return Results.NotFound();
            var profile = await db.DoctorProfiles.FirstOrDefaultAsync(d => d.UserId == user.Id);
            if(profile != null){ db.DoctorProfiles.Remove(profile); await db.SaveChangesAsync(); }
            var r = await users.DeleteAsync(user);
            if(!r.Succeeded) return Results.BadRequest(r.Errors);
            return Results.NoContent();
        });

        return app;
    }

    private static DateOnly? ParseDate(string? v)
    {
        if (string.IsNullOrWhiteSpace(v)) return null;
        if (DateOnly.TryParse(v, out var d1)) return d1;
        try
        {
            var parts = v.Split('/', '-', '.');
            if (parts.Length == 3)
            {
                var dd = int.Parse(parts[0]);
                var mm = int.Parse(parts[1]);
                var yy = int.Parse(parts[2]);
                if (yy < 100) yy += 1900;
                return new DateOnly(yy, mm, dd);
            }
        }
        catch { }
        return null;
    }
}


