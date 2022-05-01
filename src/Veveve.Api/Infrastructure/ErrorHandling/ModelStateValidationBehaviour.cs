using Veveve.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text.Json;

namespace Veveve.Api.Infrastructure.ErrorHandling;

/// <summary>
/// Catch invalid model states and format a proper response to the client
/// </summary>
public static class ModelStateValidationBehaviour
{
    /// <summary>
    /// handle modelstate validation errors
    /// </summary>
    public static IMvcBuilder ConfigureApiBehaviorOptions(this IMvcBuilder builder) =>
        builder.ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errorResponse = new ApiErrorResponse(
                    ErrorCodesEnum.GENERIC_VALIDATION,
                    context.ModelState.Keys
                        .SelectMany(key => context.ModelState[key]!.Errors
                            .Select(x => new ModelValidationError(key, x.ErrorMessage)))
                        .ToArray());

                var result = new BadRequestObjectResult(errorResponse);
                result.ContentTypes.Add(MediaTypeNames.Application.Json);

                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogWarning("ModelStateValidationError {errorResponse}", JsonSerializer.Serialize(errorResponse));

                return result;
            };
                // necessary to disable the defualt ProblemDetails model in Swagger
                options.SuppressMapClientErrors = true;
        });
}
