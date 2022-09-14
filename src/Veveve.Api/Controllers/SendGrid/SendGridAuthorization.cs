using Microsoft.AspNetCore.Mvc;
using EllipticCurve;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Filters;
using Veveve.Domain.Models.Options;

namespace Veveve.Api.Controllers.SendGrid;

// Inspiration: https://github.com/sendgrid/sendgrid-csharp/blob/main/examples/eventwebhook/RequestValidator.cs
// When testing this functionality locally, you must setup ngrok and configure sendgrid webhooks to use ngrok url.
public class SendGridAuthorization : Attribute, IAsyncAuthorizationFilter
{
    /// <summary>
    /// Signature verification HTTP header name for the signature being sent.
    /// </summary>
    private const string SIGNATURE_HEADER = "X-Twilio-Email-Event-Webhook-Signature";

    /// <summary>
    /// Timestamp HTTP header name for timestamp.
    /// </summary>
    private const string TIMESTAMP_HEADER = "X-Twilio-Email-Event-Webhook-Timestamp";

    private SendGridSettings _sendgridSettings = null!;
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        context.HttpContext.Request.EnableBuffering(); // allows request body stream to be read multiple times.

        _sendgridSettings = context.HttpContext.RequestServices.GetService<IOptions<SendGridSettings>>()!.Value;
        if (!await IsValidSignature(context.HttpContext.Request))
            context.Result = new UnauthorizedObjectResult(null);
    }

    public async Task<bool> IsValidSignature(HttpRequest request)
    {
        using var reader = new StreamReader(request.Body, leaveOpen: true); // stream must be left open to avoid being disposed. Otherwise the model binder will fail.
        var requestBody = await reader.ReadToEndAsync();
        request.Body.Position = 0; // set the position of the reader so it can be re-read from the beginning

        var timestamp = request.Headers[TIMESTAMP_HEADER];
        var signature = request.Headers[SIGNATURE_HEADER];
        var decodedSignature = Signature.fromBase64(signature);

        var ecPublicKey = PublicKey.fromPem(_sendgridSettings.VerificationKey);
        var timestampedPayload = timestamp + requestBody;
        return Ecdsa.verify(timestampedPayload, decodedSignature, ecPublicKey);
    }


}