namespace KaiHeiLa;

public class SearchGuildMemberProperties
{
    public string SearchName { get; set; }
    public uint? RoleId { get; set; }
    public bool? IsMobileVerified { get; set; }
    public SortMode? SortedByActiveTime { get; set; }
    public SortMode? SortedByJoinTime { get; set; }
}