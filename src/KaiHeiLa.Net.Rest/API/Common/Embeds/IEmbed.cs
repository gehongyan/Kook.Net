namespace KaiHeiLa.API;

internal interface IEmbed
{
    EmbedType Type { get; }

    string Url { get; }
}