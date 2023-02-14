using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class ActionGroupModule : ModuleBase
{
    [JsonPropertyName("elements")]
    public ButtonElement[] Elements { get; set; }
}
