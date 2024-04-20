using Kook.API.Rest;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class PageSortInfoConverter : JsonConverter<PageSortInfo>
{
    public override PageSortInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        PageSortInfo pageSortInfo = new() { SortKey = null, SortMode = API.Rest.SortMode.Unspecified };
        while (reader.Read())
            switch (reader.TokenType)
            {
                case JsonTokenType.PropertyName:
                    pageSortInfo.SortKey = reader.GetString();
                    break;
                case JsonTokenType.Number:
                    pageSortInfo.SortMode = reader.GetInt32() switch
                    {
                        -1 => API.Rest.SortMode.Descending,
                        1 => API.Rest.SortMode.Ascending,
                        _ => API.Rest.SortMode.Unspecified
                    };
                    break;
                case JsonTokenType.EndObject:
                    return pageSortInfo;
            }

        return pageSortInfo;
    }

    public override void Write(Utf8JsonWriter writer, PageSortInfo value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        if (value.SortKey is not null)
        {
            if (value.SortMode == API.Rest.SortMode.Ascending)
                writer.WriteNumber(value.SortKey, 1);
            else if (value.SortMode == API.Rest.SortMode.Descending)
                writer.WriteNumber(value.SortKey, -1);
        }
        writer.WriteEndObject();
    }
}
