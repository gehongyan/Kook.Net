using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class IntimacyStateConverter : JsonConverter<IntimacyState?>
{
    public override IntimacyState? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        int provider = reader.GetInt32();
        return provider switch
        {
            0 => null,
            1 => IntimacyState.Pending,
            2 => IntimacyState.Accepted,
            _ => throw new ArgumentOutOfRangeException(nameof(FriendState))
        };
    }

    public override void Write(Utf8JsonWriter writer, IntimacyState? value, JsonSerializerOptions options) =>
        writer.WriteNumberValue(value switch
        {
            null => 0,
            IntimacyState.Pending => 1,
            IntimacyState.Accepted => 2,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        });
}
