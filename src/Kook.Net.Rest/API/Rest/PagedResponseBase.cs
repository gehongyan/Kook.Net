using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class PagedResponseBase<TItem>
{
    [JsonPropertyName("items")]
    public required TItem[] Items { get; set; }

    [JsonPropertyName("meta")]
    [JsonConverter(typeof(PageMetaConverter))]
    public required PageMeta Meta { get; set; }

    [JsonPropertyName("sort")]
    [JsonConverter(typeof(PageSortInfoConverter))]
    public PageSortInfo PageSortInfo { get; set; }
}

internal class PageMeta
{
    public PageMeta(int page = 1, int pageSize = 100)
    {
        Page = page;
        PageTotal = 1;
        PageSize = pageSize;
    }

    public int Page { get; set; }
    public int PageTotal { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }

    public static PageMeta Default => new();
}

internal struct PageSortInfo
{
    public string? SortKey { get; set; }
    public SortMode SortMode { get; set; }
}
