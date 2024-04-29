using System.Text.Json.Serialization;

namespace Kook.API;

internal class ImageGroupModule : ModuleBase
{
    [JsonPropertyName("elements")]
    public required ImageElement[] Elements { get; set; }
}
