using System.Text.Json.Serialization;

namespace Kook.API;

internal class ContextModule : ModuleBase
{
    [JsonPropertyName("elements")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public ElementBase[]? Elements { get; set; }
}
