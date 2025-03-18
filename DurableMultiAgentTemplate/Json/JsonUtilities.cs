using System.ComponentModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using System.Text.Json.Serialization.Metadata;
using System.Text.Unicode;
using Microsoft.Extensions.Options;

namespace DurableMultiAgentTemplate.Json;


/// <summary>
/// Utilities for JSON.
/// </summary>
public class JsonUtilities(IOptions<JsonSerializerOptions> options)
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = options.Value;
    private static readonly JsonSchemaExporterOptions _jsonSchemaExporterOptions = new()
    {
        TreatNullObliviousAsNonNullable = true,
        // Description を追加する
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

    public string GenerateSchema(JsonTypeInfo type) =>
        JsonSchemaExporter.GetJsonSchemaAsNode(type, _jsonSchemaExporterOptions)
        .ToJsonString(_jsonSerializerOptions);

    public BinaryData GenerateSchemaAsBinaryData(JsonTypeInfo type) =>
        BinaryData.FromString(GenerateSchema(type));

    public JsonElement SerializeToElement<T>(T value) =>
        JsonSerializer.SerializeToElement(value, _jsonSerializerOptions);

    public string Serialize<T>(T value) =>
        JsonSerializer.Serialize(value, _jsonSerializerOptions);

    public T? Deserialize<T>(JsonElement element) =>
        JsonSerializer.Deserialize<T>(element, _jsonSerializerOptions);

    public T? Deserialize<T>(JsonElement element,
        JsonTypeInfo<T> jsonTypeInfo) =>
        JsonSerializer.Deserialize<T>(element, jsonTypeInfo);

    public T? Deserialize<T>(string json,
        JsonTypeInfo<T> jsonTypeInfo) =>
        JsonSerializer.Deserialize<T>(json, jsonTypeInfo);

    public T? Deserialize<T>(BinaryData json,
        JsonTypeInfo<T> jsonTypeInfo) =>
        JsonSerializer.Deserialize<T>(json, jsonTypeInfo);

    public T? Deserialize<T>(string json) =>
        JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
    public T? Deserialize<T>(BinaryData json) =>
        JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
}
