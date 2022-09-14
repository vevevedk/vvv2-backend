namespace Veveve.Domain.Models.Options;

#nullable disable
public class Appsettings
{
    public string ASPNETCORE_ENVIRONMENT { get; set; }
    public ConnectionStrings ConnectionStrings { get; set; }
    public SendGridSettings SendGrid { get; set; }
    public AuthorizationSettings Authorization { get; set; }
    public DefaultAdminUser[] DefaultAdminUsers { get; set; } = new DefaultAdminUser[0];
    public string DefaultAdminClientName { get; set; }
    public GoogleAdsApi GoogleAdsApi { get; set; }
}

public class ConnectionStrings
{
    public string DbConnection { get; set; }
}

public class SendGridSettings
{
    public string ApiKey { get; set; }
    /// <summary>
    /// Retrieved from Sendgrid -> mailsettings -> signed webhooks
    /// </summary>
    public string VerificationKey { get; set; }
    
    /// <summary>
    /// Will appear as the From name in an email sent to a customer
    /// </summary>
    public string SendFromName { get; set; }

    /// <summary>
    /// Will appear as the From email in an email sent to a customer.
    /// Might also be used as reply-to
    /// </summary>
    public string SendFromEmail { get; set; }

    /// <summary>
    /// Template id from a custom template in SendGrid
    /// </summary>
    public string ResetPasswordTemplateId { get; set; }
    
    /// <summary>
    /// Link to the frontend for resetting a password
    /// </summary>
    public string ResetPasswordLink { get; set; }
}

public class AuthorizationSettings
{
    public string JwtKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpirationInSeconds { get; set; }
}

public class DefaultAdminUser
{
    public string FullName { get; set; }
    public string Email { get; set; }
}

public class GoogleAdsApi
{
    public string DeveloperToken { get; set; }
    public string OAuth2Mode { get; set; }
    public string OAuth2ClientId { get; set; }
    public string OAuth2ClientSecret { get; set; }
    public string OAuth2RefreshToken { get; set; }
    public string CustomerId { get; set; }
}