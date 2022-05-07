using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Rest;

internal class ModifyGuildChannelParams
{
    [JsonPropertyName("name")] public string Name { get; set; }
}