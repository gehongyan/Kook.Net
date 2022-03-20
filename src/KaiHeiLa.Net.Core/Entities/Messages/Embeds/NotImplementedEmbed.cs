using System.Text.Json;
using System.Text.Json.Nodes;

namespace KaiHeiLa;

public class NotImplementedEmbed : IEmbed
{
    internal NotImplementedEmbed(string rawType, JsonNode jsonNode)
    {
        RawType = rawType;
        JsonNode = jsonNode;
    }

    public EmbedType Type => EmbedType.NotImplemented;

    public string RawType { get; internal set; }
    
    public string Url { get; internal set; }
    
    public JsonNode JsonNode { get; internal set; }
}