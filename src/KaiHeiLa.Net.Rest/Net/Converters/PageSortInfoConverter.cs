using System.Text.Json;
using System.Text.Json.Serialization;
using KaiHeiLa.API.Rest;

namespace KaiHeiLa.Net.Converters;

internal class PageSortInfoConverter : JsonConverter<PageSortInfo>
{
    public override PageSortInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        PageSortInfo pageSortInfo = new() {SortKey = null, SortMode = SortMode.Unspecified};
        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.PropertyName:
                    pageSortInfo.SortKey = reader.GetString();
                    break;
                case JsonTokenType.Number:
                    pageSortInfo.SortMode = reader.GetInt32() switch
                    {
                        -1 => SortMode.Descending,
                        1 => SortMode.Ascending,
                        _ => SortMode.Unspecified,
                    };
                    break;
                case JsonTokenType.EndObject:
                    return pageSortInfo;
            }
        }
        return pageSortInfo;
    }

    public override void Write(Utf8JsonWriter writer, PageSortInfo value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        switch (value.SortMode)
        {
            case SortMode.Ascending:
                writer.WriteNumber(value.SortKey, 1);
                break;
            case SortMode.Descending:
                writer.WriteNumber(value.SortKey, -1);
                break;
        }
        writer.WriteEndObject();
    }
}