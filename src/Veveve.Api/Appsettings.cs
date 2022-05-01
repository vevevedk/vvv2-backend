namespace Veveve.Api;

#nullable disable
public class Appsettings
{
    public string ASPNETCORE_ENVIRONMENT { get; set; }
    public ConnectionStrings ConnectionStrings { get; set; }
    public SendGridSettings SendGrid { get; set; }
    public AuthorizationSettings Authorization { get; set; }
    public DefaultAdminAccount[] DefaultAdminAccounts { get; set; } = new DefaultAdminAccount[0];
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
}

public class AuthorizationSettings
{
    public string JwtKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpirationInSeconds { get; set; }
}

public class DefaultAdminAccount
{
    public string FullName { get; set; }
    public string Email { get; set; }
}