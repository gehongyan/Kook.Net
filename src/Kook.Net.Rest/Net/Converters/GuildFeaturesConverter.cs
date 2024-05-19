using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class GuildFeaturesConverter : JsonConverter<GuildFeatures>
{
    public override GuildFeatures Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        IList<string> rawValues = JsonSerializer.Deserialize<IList<string>>(ref reader, options) ?? [];
        GuildFeature features = rawValues.Aggregate(GuildFeature.None,
            (current, item) => current | ApiStringToFeature(item));
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

    private static GuildFeature ApiStringToFeature(string apiString)
    {
        return apiString switch
        {
            "official" => GuildFeature.Official,
            "partner" => GuildFeature.Partner,
            "ka" => GuildFeature.KeyAccount,
            _ => GuildFeature.None
        };
    }

    private static string FeatureToApiString(GuildFeature feature)
    {
        return feature switch
        {
            GuildFeature.Official => "official",
            GuildFeature.Partner => "partner",
            GuildFeature.KeyAccount => "ka",
            _ => feature.ToString().ToLowerInvariant()
        };
    }
}
