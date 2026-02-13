using System.Text.Json;
using System.Text.Json.Serialization;
using Kook.API.Rest;

namespace Kook.Net.Converters;

internal class ContentFilterTargetJsonConverter : JsonConverter<ContentFilterTarget>
{
    /// <inheritdoc />
    public override ContentFilterTarget? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            reader.Skip();
            return null;
        }

        ContentFilterMode? mode = null;
        ContentFilterTargetItem[]? whitelistItems = null;
        ContentFilterTargetItem[]? blacklistItems = null;
        string[]? whitelistStrings = null;
        string[]? blacklistStrings = null;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected PropertyName token");
            string propertyName = reader.GetString()!;
            reader.Read();
            switch (propertyName)
            {
                case "enabled":
                    mode = (ContentFilterMode)Enum.Parse(typeof(ContentFilterMode), reader.GetString()!, true);
                    break;
                case "whitelist":
                    ReadTargets(ref reader, ref whitelistItems, ref whitelistStrings);
                    break;
                case "blacklist":
                    ReadTargets(ref reader, ref blacklistItems, ref blacklistStrings);
                    break;
            }
        }

        return mode switch
        {
            ContentFilterMode.Whitelist => new ContentFilterTarget
            {
                Enabled = ContentFilterMode.Whitelist,
                TargetItems = whitelistItems,
                StringItems = whitelistStrings
            },
            ContentFilterMode.Blacklist => new ContentFilterTarget
            {
                Enabled = ContentFilterMode.Blacklist,
                TargetItems = blacklistItems,
                StringItems = blacklistStrings
            },
            _ => null
        };

        void ReadTargets(ref Utf8JsonReader reader, ref ContentFilterTargetItem[]? targetItems, ref string[]? targetStrings)
        {
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                ContentFilterTargetItem[]? items = JsonSerializer.Deserialize<ContentFilterTargetItem[]>(ref reader, options);
                targetItems = items;
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                string? words = reader.GetString();
                targetStrings = words?.Split(['|'], StringSplitOptions.RemoveEmptyEntries);
            }
            else
                reader.Skip();
        }
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ContentFilterTarget value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        string mode = value.Enabled.ToString().ToLowerInvariant();
        writer.WriteString("enabled", mode);
        if (value.TargetItems is not null)
        {
            writer.WritePropertyName(mode);
            JsonSerializer.Serialize(writer, value.TargetItems, options);
        }
        else if (value.StringItems is not null)
        {
            string values = string.Join("|", value.StringItems);
            writer.WriteString(mode, values);
        }
        writer.WriteEndObject();
    }
}
