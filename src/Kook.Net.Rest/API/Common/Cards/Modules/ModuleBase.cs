using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API;

internal class ModuleBase : IModule
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(ModuleTypeConverter))]
    public ModuleType Type { get; set; }
}