using System.Text.Json.Serialization;

namespace Kook.API;

internal class ContextModule : ModuleBase
{
    [JsonPropertyName("elements")]
    public ElementBase[] Elements { get; set; }
}
