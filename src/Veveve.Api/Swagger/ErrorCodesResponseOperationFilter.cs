using Veveve.Api.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Veveve.Api.Utils;

namespace Veveve.Api.Swagger
{
    /// <summary>
    /// Add a list of all possible error codes for an endpoint along with descriptions
    /// </summary>
    public class ErrorCodesResponseOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Get custom attribute from the controller method
            var attributes = (SwaggerErrorCodesAttribute[])context.MethodInfo.GetCustomAttributes(typeof(SwaggerErrorCodesAttribute), false);
            if (attributes.Length == 0)
                return;

            // Generate a new schema for the patch request object type. This will invoke PatchObjectSchemaFilter
            foreach (var attr in attributes)
            {
                // Generate a new schema for the patch request object type. This will invoke PatchObjectSchemaFilter
                if (!context.SchemaRepository.TryLookupByType(typeof(ApiErrorResponse), out OpenApiSchema schema))
                    schema = context.SchemaGenerator.GenerateSchema(typeof(ApiErrorResponse), context.SchemaRepository);

                var description = "";
                if (attr.ErrorCodes.Any())
                {
                    description = "<p>Error codes:</p><ul>";
                    foreach (var errorCode in attr.ErrorCodes)
                    {
                        description += $"<li><i>{(int)errorCode}</i> - {errorCode.GetDescription()}</li>";
                    }
                    description += "</ul>";
                }

                operation.Responses.Add(((int)attr.HttpStatusCode).ToString(), new OpenApiResponse
                {
                    Content = new Dictionary<string, OpenApiMediaType>{
                        {"application/json", new OpenApiMediaType{
                            Schema = schema
                        }}
                    },
                    Description = description,
                });
            }
            return;
        }
    }
}