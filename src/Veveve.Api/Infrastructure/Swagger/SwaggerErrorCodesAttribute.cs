using System.Net;
using Veveve.Api.Infrastructure.ErrorHandling;

namespace Veveve.Api.Infrastructure.Swagger;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public class SwaggerErrorCodesAttribute : Attribute
{
    public SwaggerErrorCodesAttribute(HttpStatusCode httpStatusCode, params ErrorCodesEnum[] errorCodes)
    {
        HttpStatusCode = httpStatusCode;
        ErrorCodes = errorCodes;
    }

    public HttpStatusCode HttpStatusCode { get; }
    public ErrorCodesEnum[] ErrorCodes { get; }
}