using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class ContainerModule : ModuleBase
{
    [JsonPropertyName("elements")]
    public ImageElement[] Elements { get; set; }
}
