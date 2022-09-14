using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Veveve.Domain.Database.Entities;

namespace Veveve.Api.Controllers.SendGrid;

public class SendGridWebhookEventRequest
{
    [Required]
    [JsonPropertyName("event")]
    public string Event { get; set; } = null!;

    public EmailEventEnum? EventAsEnum
    {
        get
        {
            if(!Enum.TryParse<EmailEventEnum>(Event, true, out EmailEventEnum parsed))
                return null;

            return parsed;
        }
    }

    [Required]
    [JsonPropertyName("reference")]
    public Guid Reference { get; set; }

    [Required]
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }
}