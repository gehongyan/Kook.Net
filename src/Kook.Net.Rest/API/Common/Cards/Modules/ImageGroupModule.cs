using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class ImageGroupModule : ModuleBase
{
    [JsonPropertyName("elements")]
    public ImageElement[] Elements { get; set; }
}
