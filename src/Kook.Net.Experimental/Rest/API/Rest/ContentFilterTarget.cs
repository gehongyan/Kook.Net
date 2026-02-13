using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Rest;

[JsonConverter(typeof(ContentFilterTargetJsonConverter))]
internal class ContentFilterTarget
{
    public ContentFilterMode Enabled { get; set; }
    public ContentFilterTargetItem[]? TargetItems { get; set; }
    public string[]? StringItems { get; set; }
}
