using System.ComponentModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using System.Text.Json.Serialization.Metadata;
using System.Text.Unicode;

namespace DurableMultiAgentTemplate.Json;


/// <summary>
/// Generator for JSON schema creation.
/// Supports adding descriptions to JSON schemas by specifying Description attributes on class definitions.
/// </summary>
internal static class JsonSchemaGenerator
{
    private static readonly JsonSchemaExporterOptions _jsonSchemaExporterOptions = new()
    {
        TreatNullObliviousAsNonNullable = true,
        // Add description to schema
        TransformSchemaNode = (context, schema) =>
        {
            var attributeProvider = context.PropertyInfo is not null ?
                context.PropertyInfo.AttributeProvider :
                context.TypeInfo.Type;

            var description = (DescriptionAttribute?)attributeProvider?.GetCustomAttributes(false)
                .FirstOrDefault(x => x is DescriptionAttribute);

            if (description == null) return schema;

            if (schema is JsonObject jsonObject)
            {
                jsonObject.Insert(0, "description", description.Description);
            }

            return schema;
        },
    };

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
    };

    /// <summary>
    /// Generates a JSON schema as a string from the specified type information.
    /// </summary>
    /// <param name="type">The JSON type information to generate the schema from</param>
    /// <returns>JSON schema as a string with Unicode encoding support</returns>
    public static string GenerateSchema(JsonTypeInfo type) =>
        JsonSchemaExporter.GetJsonSchemaAsNode(type, _jsonSchemaExporterOptions).ToJsonString(_jsonSerializerOptions);
    
    /// <summary>
    /// Generates a JSON schema as binary data from the specified type information.
    /// </summary>
    /// <param name="type">The JSON type information to generate the schema from</param>
    /// <returns>JSON schema as binary data</returns>
    public static BinaryData GenerateSchemaAsBinaryData(JsonTypeInfo type) =>
        BinaryData.FromString(GenerateSchema(type));
}
