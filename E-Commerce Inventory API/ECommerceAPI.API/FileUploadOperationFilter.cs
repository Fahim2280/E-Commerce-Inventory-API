using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace ECommerceAPI.API
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileUploadMime = "multipart/form-data";
            if (operation.RequestBody == null || !operation.RequestBody.Content.Any(x => x.Key.Equals(fileUploadMime, StringComparison.InvariantCultureIgnoreCase)))
                return;

            var fileParams = context.MethodInfo.GetParameters().Where(p => p.ParameterType == typeof(IFormFile));
            if (!fileParams.Any())
                return;

            operation.RequestBody.Content[fileUploadMime].Schema.Properties =
                operation.RequestBody.Content[fileUploadMime].Schema.Properties
                .ToDictionary(k => k.Key, v => v.Value);
        }
    }
}