namespace KaiHeiLa;

public class IntimacyImage
{
    internal IntimacyImage(int id, string url)
    {
        Id = id;
        Url = url;
    }

    public int Id { get; }

    public string Url { get; }
}