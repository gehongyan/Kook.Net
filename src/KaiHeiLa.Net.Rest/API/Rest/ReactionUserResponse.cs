using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Rest;

internal class ReactionUserResponse : User
{
    [JsonPropertyName("reaction_time")]
    [JsonConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset ReactionTime { get; set; }
}