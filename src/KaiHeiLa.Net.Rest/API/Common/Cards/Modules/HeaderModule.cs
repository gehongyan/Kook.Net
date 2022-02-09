using System.Text.Json.Serialization;

namespace KaiHeiLa.API;

internal class HeaderModule : ModuleBase
{
    [JsonPropertyName("text")]
    public PlainTextElement Text { get; set; }
}