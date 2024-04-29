using System.Text.Json.Serialization;

namespace Kook.API;

internal class ContainerModule : ModuleBase
{
    [JsonPropertyName("elements")]
    public required ImageElement[] Elements { get; set; }
}
