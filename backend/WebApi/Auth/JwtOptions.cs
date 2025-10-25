namespace WebApi.Auth;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "DentaCare";
    public string Audience { get; set; } = "DentaCareUI";
    public string SigningKey { get; set; } = "change-this-in-production";
    public int AccessTokenMinutes { get; set; } = 60;
}






