namespace Veveve.Domain.Models.Options;

public class AuthorizationOptions
{
    public static string SectionName = "Authorization"; // Name of object in appsettings

    public string JwtKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationInSeconds { get; set; }
}
