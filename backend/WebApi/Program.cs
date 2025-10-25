using Infrastructure;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApi.Auth;
using WebApi.Patients;
using WebApi.Appointments;
using FluentValidation;
using Microsoft.OpenApi.Models;
using WebApi.Uploads;
using WebApi.Services;
using WebApi.Users;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Log.Logger);

// JWT configuration
var jwtOptions = new WebApi.Auth.JwtOptions();
builder.Configuration.Bind("Jwt", jwtOptions);
builder.Services.AddSingleton(jwtOptions);
builder.Services.AddSingleton(new WebApi.Auth.TokenService(jwtOptions));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
    };
});
builder.Services.AddAuthorization();
builder.Services.AddSingleton<Shared.Security.ICurrentUserService, WebApi.Security.CurrentUserService>();

// Serilog basic wiring can be added later if needed

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<Shared.Tenant.ITenantProvider, WebApi.Tenancy.TenantProvider>();
builder.Services.AddInfrastructure(builder.Configuration);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    // Map DateOnly/TimeOnly so Swagger can render schemas
    o.MapType<DateOnly>(() => new OpenApiSchema { Type = "string", Format = "date" });
    o.MapType<TimeOnly>(() => new OpenApiSchema { Type = "string", Format = "time" });
});
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Application.Abstractions.IApplicationDbContext).Assembly));
builder.Services.AddValidatorsFromAssemblyContaining<Application.Patients.Validators.CreatePatientValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<Application.Appointments.Validators.CreateAppointmentValidator>();

var app = builder.Build();

// Seed roles and admin
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<Infrastructure.Identity.IdentitySeeder>();
    await seeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Disabled in dev to avoid proxying to HTTPS without a dev cert
// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapAuthEndpoints();
app.MapPatientEndpoints();
app.MapAppointmentEndpoints();
app.MapUploadEndpoints();
app.MapUserEndpoints();
app.MapServiceEndpoints();

// duplicate legacy endpoint removed; use MapPatientEndpoints()

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
