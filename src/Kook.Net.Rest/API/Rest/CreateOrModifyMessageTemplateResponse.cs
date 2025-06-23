using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class CreateOrModifyMessageTemplateResponse
{
    [JsonPropertyName("model")]
    public required MessageTemplate Model { get; set; }
}
