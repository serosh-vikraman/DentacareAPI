using Microsoft.AspNetCore.Authorization;

namespace WebApi.Uploads;

public static class UploadEndpoints
{
    public static IEndpointRouteBuilder MapUploadEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/upload/photo", [Authorize] async (HttpRequest request, IWebHostEnvironment env) =>
        {
            if (!request.HasFormContentType) return Results.BadRequest("Form-data expected");
            var form = await request.ReadFormAsync();
            var file = form.Files.FirstOrDefault();
            if (file == null || file.Length == 0) return Results.BadRequest("No file");
            var uploadsRoot = Path.Combine(env.ContentRootPath, "Uploads");
            Directory.CreateDirectory(uploadsRoot);
            var safeName = Path.GetFileNameWithoutExtension(file.FileName);
            var ext = Path.GetExtension(file.FileName);
            var name = safeName + "-" + Guid.NewGuid().ToString("N").Substring(0,8) + ext;
            var path = Path.Combine(uploadsRoot, name);
            await using (var stream = File.Create(path))
            {
                await file.CopyToAsync(stream);
            }
            var publicUrl = $"/Uploads/{name}";
            return Results.Ok(new { url = publicUrl, name });
        });

        return app;
    }
}


