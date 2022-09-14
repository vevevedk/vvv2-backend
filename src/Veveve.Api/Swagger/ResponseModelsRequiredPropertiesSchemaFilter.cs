using Veveve.Api.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Veveve.Api.Swagger
{
    /// <summary>
    /// Mark all properties in our response models as required.
    /// When generating a typescript proxy from the open-api spec, the model properties will no longer have | undefined
    /// </summary>
    public class ResponseModelsRequiredPropertiesSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if(!schema.Properties.Any())
                return;

            if(!context.Type.IsAssignableTo(typeof(BaseResponse)))
                return;

            schema.Required = new HashSet<string>();

            foreach(var schemaProp in schema.Properties){
                schema.Required.Add(schemaProp.Key);
            }
        }
    }
}