using System.Text.Json.Nodes;

namespace Kook.API;

internal class NotImplementedEmbed : EmbedBase
{
    internal NotImplementedEmbed(string rawType, JsonNode rawJsonNode)
    {
        RawType = rawType;
        RawJsonNode = rawJsonNode;
    }

    public string RawType { get; set; }

    public JsonNode RawJsonNode { get; set; }
}
