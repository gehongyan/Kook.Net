using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API;

internal class ContextModule : ModuleBase
{
    [JsonPropertyName("elements")]
    public ElementBase[] Elements { get; set; }
}