using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Rest;

internal class ReactionUserResponse : User
{
    [JsonPropertyName("reaction_time")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public DateTimeOffset ReactionTime { get; set; }
}