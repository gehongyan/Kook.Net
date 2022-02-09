using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API;

internal class ActionGroupModule : ModuleBase
{
    [JsonPropertyName("elements")]
    public ButtonElement[] Elements { get; set; }
}