using System.Text.Json;
using System.Text.Json.Serialization;
using Kook.API.Rest;

namespace Kook.Net.Converters;

internal class PageMetaConverter : JsonConverter<PageMeta>
{
    /// <inheritdoc />
    public override PageMeta Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        PageMeta pageMeta = new();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return pageMeta;

            if (reader.TokenType != JsonTokenType.PropertyName)
                continue;

            string? propertyName = reader.GetString();
            reader.Read();

            switch (propertyName)
            {
                case "page" or "currentPage":
                    pageMeta.Page = reader.GetInt32();
                    break;
                case "page_total" or "pageCount":
                    pageMeta.PageTotal = reader.GetInt32();
                    break;
                case "page_size" or "perPage":
                    pageMeta.PageSize = reader.GetInt32();
                    break;
                case "total" or "totalCount":
                    pageMeta.Total = reader.GetInt32();
                    break;
                default:
                    continue;
            }
        }

        return pageMeta;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, PageMeta value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("page", value.Page);
        writer.WriteNumber("page_total", value.PageTotal);
        writer.WriteNumber("page_size", value.PageSize);
        writer.WriteNumber("total", value.Total);
        writer.WriteEndObject();
    }
}
