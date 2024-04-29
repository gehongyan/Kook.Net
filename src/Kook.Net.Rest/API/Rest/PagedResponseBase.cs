using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class PagedResponseBase<TItem>
{
    [JsonPropertyName("items")]
    public required TItem[] Items { get; set; }

    [JsonPropertyName("meta")]
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

    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("page_total")]
    public int PageTotal { get; set; }

    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    public static PageMeta Default => new();
}

internal struct PageSortInfo
{
    public string? SortKey { get; set; }
    public SortMode SortMode { get; set; }
}
