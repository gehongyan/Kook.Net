using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Rest;

internal class DeleteUserChatParams
{
    [JsonPropertyName("chat_code")]
    [JsonConverter(typeof(ChatCodeConverter))]
    public Guid ChatCode { get; set; }

    public static implicit operator DeleteUserChatParams(Guid chatCode) => new() {ChatCode = chatCode};
}