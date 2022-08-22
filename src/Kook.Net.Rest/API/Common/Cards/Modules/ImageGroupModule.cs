using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API;

internal class ImageGroupModule : ModuleBase
{
    [JsonPropertyName("elements")]
    public ImageElement[] Elements { get; set; }
}