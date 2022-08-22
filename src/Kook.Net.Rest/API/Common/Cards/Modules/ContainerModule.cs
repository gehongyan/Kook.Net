using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API;

internal class ContainerModule : ModuleBase
{
    [JsonPropertyName("elements")]
    public ImageElement[] Elements { get; set; }
}