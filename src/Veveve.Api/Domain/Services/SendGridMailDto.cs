using System;
using SendGrid.Helpers.Mail;

namespace Veveve.Api.Domain.Services;

public class SendGridMailDto
{
    public SendGridMailDto(
        string templateId,
        object templateData,
        Guid reference,
        EmailAddress from,
        EmailAddress to,
        EmailAddress? replyTo = null)
    {
        TemplateId = templateId;
        TemplateData = templateData;
        Reference = reference;
        From = from;
        To = to;
        ReplyTo = replyTo;
    }

    public string TemplateId { get; set; }
    public object TemplateData { get; set; }
    public EmailAddress From { get; set; }
    public EmailAddress To { get; set; }

    public Guid Reference { get; set; }

    /// <summary>
    /// Optional
    /// </summary>
    public EmailAddress? ReplyTo { get; set; }
}