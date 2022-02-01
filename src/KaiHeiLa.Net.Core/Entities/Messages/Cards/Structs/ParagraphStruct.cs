using System.Text.Json.Serialization;

namespace KaiHeiLa;

/// <summary>
///     区域文本
/// </summary>
/// <remarks>
///     支持分栏结构，将模块分为左右两栏，根据顺序自动排列，支持更自由的文字排版模式，提高可维护性
/// </remarks>
public class ParagraphStruct : IStruct
{
    private int _columnCount;

    public ParagraphStruct(int columnCount)
    {
        _columnCount = columnCount;
        Fields = new List<IStruct>();
    }
    
    public StructType Type => StructType.Paragraph;

    [JsonPropertyName("cols")]
    public int ColumnCount
    {
        get => _columnCount;
        internal set
        {
            if (value is < 1 or > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "可以的取值为 1-3");
            }
            _columnCount = value; 
        }
    }

    public List<IStruct> Fields { get; internal set; }

    public ParagraphStruct Add(IStruct field)
    {
        if (Fields.Count >= 50)
        {
            throw new ArgumentOutOfRangeException(nameof(Fields), $"{nameof(Fields)} 最多有 50 个元素");
        }
        if (field is not (PlainTextElement or KMarkdownElement))
        {
            throw new ArgumentOutOfRangeException(nameof(field),
                $"{Fields} 可以的元素为 {nameof(PlainTextElement)} 或 {nameof(KMarkdownElement)}");
        }
        Fields.Add(field);
        return this;
    }
}