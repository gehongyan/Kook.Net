using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class IntimacyRelationTypeConverter : JsonConverter<IntimacyRelationType?>
{
    public override IntimacyRelationType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        int provider = reader.GetInt32();
        return provider switch
        {
            0 => null,
            1 => IntimacyRelationType.Lover,
            2 => IntimacyRelationType.Buddy,
            3 => IntimacyRelationType.BestFriendsForever,
            _ => throw new ArgumentOutOfRangeException(nameof(FriendState))
        };
    }

    public override void Write(Utf8JsonWriter writer, IntimacyRelationType? value, JsonSerializerOptions options) =>
        writer.WriteNumberValue(value switch
        {
            null => 0,
            IntimacyRelationType.Lover => 1,
            IntimacyRelationType.Buddy => 2,
            IntimacyRelationType.BestFriendsForever => 3,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        });
}
