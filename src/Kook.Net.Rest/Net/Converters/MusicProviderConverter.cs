using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class MusicProviderConverter : JsonConverter<MusicProvider>
{
    public override MusicProvider Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string provider = reader.GetString();
        return provider switch
        {
            "cloudmusic" => MusicProvider.NetEaseCloudMusic,
            "qqmusic" => MusicProvider.TencentMusic,
            "kugou" => MusicProvider.KuGouMusic,
            _ => MusicProvider.Unspecified
        };
    }

    public override void Write(Utf8JsonWriter writer, MusicProvider value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            MusicProvider.NetEaseCloudMusic => "cloudmusic",
            MusicProvider.TencentMusic => "qqmusic",
            MusicProvider.KuGouMusic => "kugou",
            _ => string.Empty
        });
    }
}