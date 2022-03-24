namespace KaiHeiLa;

public interface ITag
{
    int Index { get; }
    int Length { get; }
    TagType Type { get; }
    dynamic Key { get; }
    object Value { get; }
}