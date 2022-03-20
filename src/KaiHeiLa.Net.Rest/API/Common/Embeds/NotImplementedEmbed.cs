using System.Text.Json.Nodes;

namespace KaiHeiLa.API;

internal class NotImplementedEmbed : EmbedBase
{
    internal NotImplementedEmbed(string rawType, string url, JsonNode rawJsonNode)
    {
        RawType = rawType;
        Url = url;
        RawJsonNode = rawJsonNode;
    }

    public string RawType { get; set; }
    
    public JsonNode RawJsonNode { get; set; }
}