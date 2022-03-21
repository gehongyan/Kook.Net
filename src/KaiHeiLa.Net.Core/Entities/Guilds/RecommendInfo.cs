namespace KaiHeiLa;

public class RecommendInfo
{
    public int Id { get; set; }
    public int GuildId { get; set; }
    public int Type { get; set; }
    public int Status { get; set; }
    public string Banner { get; set; }
    public string Desc { get; set; }
    public int TagID { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public int RecommendAt { get; set; }
    public int IsOfficialPartner { get; set; }
    public int Sort { get; set; }
}