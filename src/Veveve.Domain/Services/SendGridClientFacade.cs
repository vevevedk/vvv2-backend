using Veveve.Domain.Database.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using Veveve.Domain.Models.Options;

namespace Veveve.Domain.Services;

public class SendGridClientFacade : ISendGridClientFacade
{
    private readonly ILogger<SendGridClientFacade> _logger;
    private readonly ISendGridClient _sendGridClient;
    private readonly SendGridSettings _sendGridSettings;

    public SendGridClientFacade(
        ILogger<SendGridClientFacade> logger,
        IOptions<SendGridSettings> sendGridSettings,
        ISendGridClient sendGridClient)
    {
        _logger = logger;
        _sendGridClient = sendGridClient;
        _sendGridSettings = sendGridSettings.Value;
    }

    public async Task SendEmail(SendGridMailDto mailDto)
    {
        await Task.Run(() => { });
        var msg = new SendGridMessage();
        msg.AddTo(mailDto.To.Email, mailDto.To.Name);
        if (mailDto.ReplyTo != null)
            msg.SetReplyTo(new EmailAddress(mailDto.ReplyTo.Email, mailDto.ReplyTo.Name));
        msg.SetFrom(mailDto.From.Email, mailDto.From.Name);
        msg.SetTemplateId(mailDto.TemplateId);
        msg.SetTemplateData(mailDto.TemplateData);
        msg.AddCustomArg(nameof(EmailLogEntity.Reference), mailDto.Reference.ToString());

        var response = await _sendGridClient.SendEmailAsync(msg);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Body.ReadAsStringAsync();
            throw new Exception($"SendGrid invoked sending to with template {msg.TemplateId}, response {content}");
        }
    }
}