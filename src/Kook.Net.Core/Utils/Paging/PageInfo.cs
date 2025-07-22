namespace Kook;

internal class PageInfo<T> : PageInfo
{
    public T? Position { get; set; }

    internal PageInfo(T? pos, int? count, int pageSize)
    {
        Page = 1;
        Position = pos;
        Count = count;
        Remaining = count;
        PageSize = pageSize;

        if (Count < PageSize)
            PageSize = Count.Value;
    }
}

internal class PageInfo
{
    public int Page { get; set; }
    public int? Count { get; set; }
    public int PageSize { get; set; }
    public int? Remaining { get; set; }
}
