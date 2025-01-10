using System.ComponentModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using System.Text.Json.Serialization.Metadata;
using System.Text.Unicode;

namespace DurableMultiAgentTemplate.Json;


/// <summary>
/// JsonSchema �𐶐�����B
/// �N���X��`�� Description �������w�肷�邱�Ƃ� JsonSchema �ɂ� description ��ǉ�����B
/// 
/// </summary>
internal static class JsonSchemaGenerator
{
    private static readonly JsonSchemaExporterOptions _jsonSchemaExporterOptions = new()
    {
        TreatNullObliviousAsNonNullable = true,
        // Description ��ǉ�����
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

    public static string GenerateSchema(JsonTypeInfo type) => 
        JsonSchemaExporter.GetJsonSchemaAsNode(type, _jsonSchemaExporterOptions).ToJsonString(_jsonSerializerOptions);
    public static BinaryData GenerateSchemaAsBinaryData(JsonTypeInfo type) =>
        BinaryData.FromString(GenerateSchema(type));
}
