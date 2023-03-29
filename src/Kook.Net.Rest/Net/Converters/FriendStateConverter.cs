using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Converters;

internal class FriendStateConverter : JsonConverter<FriendState>
{
    public override FriendState Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string provider = reader.GetString();
        return provider switch
        {
            "request" => FriendState.Pending,
            "friend" => FriendState.Accepted,
            "blocked" => FriendState.Blocked,
            _ => throw new ArgumentOutOfRangeException(nameof(FriendState))
        };
    }

    public override void Write(Utf8JsonWriter writer, FriendState value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value switch
        {
            FriendState.Pending => "request",
            FriendState.Accepted => "friend",
            FriendState.Blocked => "blocked",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        });
}
