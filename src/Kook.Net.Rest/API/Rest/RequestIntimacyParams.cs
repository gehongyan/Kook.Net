using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class RequestIntimacyParams
{
    [JsonPropertyName("user_code")]
    public required string FullQualification { get; set; }

    [JsonPropertyName("relation_type")]
    public required IntimacyRelationType RelationType { get; set; }
}
