using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class DeleteMessageTemplateParams
{
    [JsonPropertyName("id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public required ulong Id { get; set; }

    public static implicit operator DeleteMessageTemplateParams(ulong id) => new() { Id = id };
}
