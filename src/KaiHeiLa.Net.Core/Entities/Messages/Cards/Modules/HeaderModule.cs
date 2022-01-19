namespace KaiHeiLa;

/// <summary>
///     标题模块
/// </summary>
/// <remarks>
///     标题模块只能支持展示标准文本（text），突出标题样式
/// </remarks>
public class HeaderModule : IModule
{
    private PlainTextElement _text;

    public HeaderModule(string content)
    {
        Text = content;
    }
    
    public HeaderModule(PlainTextElement text)
    {
        Text = text;
    }

    public ModuleType Type => ModuleType.Header;

    public PlainTextElement Text
    {
        get => _text;
        internal set
        {
            if (_text.Content.Length > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "content 不能超过 100 个字");
            }
            _text = value; 
            
        }
    }

    public static implicit operator HeaderModule(string content) => new(content);
}