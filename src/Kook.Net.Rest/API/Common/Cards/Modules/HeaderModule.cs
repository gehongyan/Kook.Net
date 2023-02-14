using System.Text.Json.Serialization;

namespace Kook.API;

internal class HeaderModule : ModuleBase
{
    [JsonPropertyName("text")]
    public PlainTextElement Text { get; set; }
}
