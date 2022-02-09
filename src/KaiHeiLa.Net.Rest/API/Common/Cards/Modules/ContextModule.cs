using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API;

internal class ContextModule : ModuleBase
{
    [JsonPropertyName("elements")]
    public ElementBase[] Elements { get; set; }
}