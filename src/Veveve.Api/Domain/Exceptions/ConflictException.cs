using System;
using Veveve.Api.Infrastructure.ErrorHandling;

namespace Veveve.Api.Domain.Exceptions;

/// <summary>
/// To be thrown when business logic fails due to a conflict.
/// This exception type is caught by the global filter and formatted before sending responses to the client.
/// </summary>
public class ConflictException : BusinessRuleException
{
    public ConflictException(ErrorCodesEnum errorCode) : base(errorCode)
    {
    }
}