using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     Represents a header module in an <see cref="ICard"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class HeaderModule : IModule
{
    internal HeaderModule(PlainTextElement text)
    {
        Text = text;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Header;

    /// <summary>
    ///     Gets the text element of the header.
    /// </summary>
    /// <returns>
    ///     A <see cref="PlainTextElement"/> representing the text of the header.
    /// </returns>
    public PlainTextElement Text { get; internal set; }
    
    public override string ToString() => Text.ToString();
    private string DebuggerDisplay => $"{Type}: {Text}";
}