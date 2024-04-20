using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class GuildFeaturesConverter : JsonConverter<GuildFeatures>
{
    public override GuildFeatures Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        IList<string> rawValues = JsonSerializer.Deserialize<IList<string>>(ref reader, options) ?? [];
        GuildFeature features = GuildFeature.None;

        foreach (string item in rawValues)
        {
            if (Enum.TryParse(item, true, out GuildFeature result))
                features |= result;
        }

        return new GuildFeatures(features, rawValues);
    }

    public override void Write(Utf8JsonWriter writer, GuildFeatures value, JsonSerializerOptions options)
    {
        Array enumValues = Enum.GetValues(typeof(GuildFeature));

        writer.WriteStartArray();
        foreach (object enumValue in enumValues)
        {
            GuildFeature val = (GuildFeature)enumValue;
            if (val is GuildFeature.None)
                continue;

            if (value.Value.HasFlag(val))
                writer.WriteStringValue(FeatureToApiString(val));
        }

        writer.WriteEndArray();
    }

    private static string FeatureToApiString(GuildFeature feature) =>
        feature.ToString().ToLower();
}
