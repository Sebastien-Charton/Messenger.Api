using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Messenger.Api.Web.Infrastructure.Filters;

public class AcceptedLanguageOperationFilter : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var parameter = new OpenApiParameter
        {
            Name = "Accept-Language",
            Kind = OpenApiParameterKind.Header,
            Description = "Language preference for the response.",
            IsRequired = true,
            IsNullableRaw = true,
            Default = "en-US",
            Schema = new JsonSchema
            {
                Type = JsonObjectType.String, Item = new JsonSchema { Type = JsonObjectType.String }
            }
        };

        parameter.Schema.Enumeration.Add("en-US");
        parameter.Schema.Enumeration.Add("fr-FR");

        context.OperationDescription.Operation.Parameters.Add(parameter);

        return true;
    }
}
