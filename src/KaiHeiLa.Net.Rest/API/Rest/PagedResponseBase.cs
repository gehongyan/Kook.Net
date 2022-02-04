using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API;

internal class PagedResponseBase<TItem>
{
    [JsonPropertyName("items")]
    public TItem[] Items { get; set; }

    [JsonPropertyName("meta")]
    public PageMeta Meta { get; set; }

    [JsonPropertyName("sort")]
    [JsonConverter(typeof(PageSortInfoConverter))]
    public PageSortInfo PageSortInfo { get; set; }
}

internal struct PageMeta
{
    public int Page { get; set; }
    public int PageTotal { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
}

internal struct PageSortInfo
{
    public string SortKey { get; set; }
    public SortMode SortMode { get; set; }
}