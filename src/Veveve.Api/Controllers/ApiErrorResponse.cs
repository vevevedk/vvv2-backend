using Veveve.Domain.Utils;
using Veveve.Domain.Exceptions;

namespace Veveve.Api.Controllers;

/// <summary>
/// The typical error type that will be sent to the client when an error happens in the API
/// </summary>
public class ApiErrorResponse : BaseResponse
{
    public ApiErrorResponse(ErrorCodesEnum errorCode, params string[] messageParams)
    {
        ErrorCode = (int)errorCode;
        ErrorMessage = string.Format(errorCode.GetDescription(), messageParams);
        ValidationErrors = new ModelValidationError[] { };
    }

    public ApiErrorResponse(ErrorCodesEnum errorCode, ModelValidationError[]? validationErrors = null)
    {
        ErrorCode = (int)errorCode;
        ErrorMessage = errorCode.GetDescription();
        ValidationErrors = validationErrors ?? new ModelValidationError[] { };
    }

    public int ErrorCode { get; set; }

    public string ErrorMessage { get; set; }

    /// <summary>
    /// will contain validation errors if any
    /// </summary>
    public ModelValidationError[] ValidationErrors { get; set; } = new ModelValidationError[] { };
}

public class ModelValidationError : BaseResponse
{
    public ModelValidationError(string field, string description)
    {
        Field = field;
        Description = description;
    }

    public string Field { get; set; }

    public string Description { get; set; }
}