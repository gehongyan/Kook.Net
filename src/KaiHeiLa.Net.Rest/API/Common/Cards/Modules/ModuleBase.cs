using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API;

internal class ModuleBase : IModule
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(ModuleTypeConverter))]
    public ModuleType Type { get; set; }
}