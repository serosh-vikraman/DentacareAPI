using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Auth;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (
            LoginRequest req,
            UserManager<ApplicationUser> userManager,
            TokenService tokenService) =>
        {
            var user = await userManager.FindByEmailAsync(req.Email);
            if (user == null) return Results.Unauthorized();
            var valid = await userManager.CheckPasswordAsync(user, req.Password);
            if (!valid) return Results.Unauthorized();
            var roles = await userManager.GetRolesAsync(user);
            var token = tokenService.CreateAccessToken(user, roles);
            return Results.Ok(new { accessToken = token });
        });

        return app;
    }
}


