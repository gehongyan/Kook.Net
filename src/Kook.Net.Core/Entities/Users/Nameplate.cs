using System.Diagnostics;

namespace Kook;

/// <summary>
///     表示一个用户的铭牌。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record Nameplate
{
    /// <summary>
    ///     获取此铭牌的名称。
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     获取此铭牌的类型。
    /// </summary>
    public int Type { get; }

    /// <summary>
    ///     获取此铭牌的图标的 URL。
    /// </summary>
    public string Icon { get; }

    /// <summary>
    ///     获取此铭牌的提示信息。
    /// </summary>
    public string Tips { get; }

    private Nameplate(string name, int type, string icon, string tips)
    {
        Name = name;
        Type = type;
        Icon = icon;
        Tips = tips;
    }

    internal static Nameplate Create(string name, int type, string icon, string tips)
    {
        return new Nameplate(name, type, icon, tips);
    }

    private string DebuggerDisplay => Name;
}
