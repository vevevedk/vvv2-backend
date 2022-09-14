namespace Veveve.Domain.Models.Options;

public class SendGridOptions
{
    public static string SectionName = "SendGrid"; // Name of object in appsettings

    public string ApiKey { get; set; } = string.Empty;
    /// <summary>
    /// Retrieved from Sendgrid -> mailsettings -> signed webhooks
    /// </summary>
    public string VerificationKey { get; set; } = string.Empty;

    /// <summary>
    /// Will appear as the From name in an email sent to a customer
    /// </summary>
    public string SendFromName { get; set; } = string.Empty;

    /// <summary>
    /// Will appear as the From email in an email sent to a customer.
    /// Might also be used as reply-to
    /// </summary>
    public string SendFromEmail { get; set; } = string.Empty;

    /// <summary>
    /// Template id from a custom template in SendGrid
    /// </summary>
    public string ResetPasswordTemplateId { get; set; } = string.Empty;

    /// <summary>
    /// Link to the frontend for resetting a password
    /// </summary>
    public string ResetPasswordLink { get; set; } = string.Empty;
}
