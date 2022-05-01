using Veveve.Api.Infrastructure.ErrorHandling;

namespace Veveve.Api.Domain.Exceptions;

/// <summary>
/// To be thrown when client input is badly formatted.
/// This exception type is caught by the global filter and formatted before sending responses to the client.
/// </summary>
public class BadFormatException : BusinessRuleException
{
    public BadFormatException(string propertyName) 
        : base(ErrorCodesEnum.GENERIC_PROPERTY_BAD_FORMAT, propertyName)
    {}
}