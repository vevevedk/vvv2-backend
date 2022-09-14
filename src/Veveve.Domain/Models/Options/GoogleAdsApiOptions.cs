namespace Veveve.Domain.Models.Options;

public class GoogleAdsApiOptions
{
    public static string SectionName = "GoogleAdsApi"; // Name of object in appsettings

    public string DeveloperToken { get; set; } = string.Empty;
    public string OAuth2Mode { get; set; } = string.Empty;
    public string OAuth2ClientId { get; set; } = string.Empty;
    public string OAuth2ClientSecret { get; set; } = string.Empty;
    public string OAuth2RefreshToken { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
}