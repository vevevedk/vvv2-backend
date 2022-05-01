using System;
using Veveve.Api.Infrastructure.ErrorHandling;

namespace Veveve.Api.Domain.Exceptions;

/// <summary>
/// To be thrown when business logic fails.
/// This exception type is caught by the global filter and formatted before sending responses to the client.
/// </summary>
public class BusinessRuleException : Exception
{
    public BusinessRuleException(ErrorCodesEnum errorCode, params string[] messageParams)
    {
        ErrorCode = errorCode;
        MessageParams = messageParams;
    }

    public ErrorCodesEnum ErrorCode { get; set; }
    public string[] MessageParams { get; set; }
}