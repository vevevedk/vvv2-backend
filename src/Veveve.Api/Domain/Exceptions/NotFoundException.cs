using Veveve.Api.Infrastructure.ErrorHandling;

namespace Veveve.Api.Domain.Exceptions;

/// <summary>
/// To be thrown when stuff wasnt found.
/// This exception type is caught by the global filter and formatted before sending responses to the client.
/// </summary>
public class NotFoundException : BusinessRuleException
{
    public NotFoundException(ErrorCodesEnum errorCode) : base(errorCode)
    {
    }
}