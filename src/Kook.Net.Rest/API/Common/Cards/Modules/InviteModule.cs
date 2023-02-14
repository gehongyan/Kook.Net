using System.Text.Json.Serialization;

namespace Kook.API;

internal class InviteModule : ModuleBase
{
    [JsonPropertyName("code")]
    public string Code { get; set; }
}
