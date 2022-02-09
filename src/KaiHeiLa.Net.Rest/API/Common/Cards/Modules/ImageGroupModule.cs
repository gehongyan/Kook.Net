using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API;

internal class ImageGroupModule : ModuleBase
{
    [JsonPropertyName("elements")]
    public ImageElement[] Elements { get; set; }
}