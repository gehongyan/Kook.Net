namespace Kook;

/// <summary>
///     表示 <see cref="ButtonElement"/> 被点击时触发的事件类型。
/// </summary>
public enum ButtonClickEventType
{
    /// <summary>
    ///     用户点击按钮时不会触发任何事件。
    /// </summary>
    None,

    /// <summary>
    ///     用户点击按钮时将会被重定向到指定的 URL。
    /// </summary>
    Link,

    /// <summary>
    ///     用户点击按钮时将会提交按钮的 <see cref="P:Kook.ButtonElement.Value"/> 属性的值，KOOK 将会通过网关携带此值下发事件。
    /// </summary>
    ReturnValue
}
