using System;
using System.ComponentModel.DataAnnotations;

namespace Veveve.Api.Infrastructure.Database.Entities;

public class EmailLogEntity : BaseEntity
{
    public EmailLogEntity()
    {
        Event = EmailEventEnum.Registered;
    }

    [Required]
    public string Email { get; set; } = null!;

    /// <summary>
    /// This is used for SendGrid callbacks to update a row with a new status
    /// </summary>
    [Required]
    public Guid Reference { get; set; }
    public EmailTypeEnum EmailType { get; set; }
    public EmailEventEnum Event { get; set; }
}

public enum EmailTypeEnum
{
    ResetPassword
}

public enum EmailEventEnum
{
    Registered, // our own status for when we sent it to the email provider and await status
    Processed,
    Dropped,
    Delivered,
    Deferred,
    Bounce,
    Blocked
}
