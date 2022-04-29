namespace KaiHeiLa;

/// <summary>
///     Specifies the event type fired when a <see cref="ButtonElement"/> is clicked.
/// </summary>
public enum ButtonClickEventType
{
    /// <summary>
    ///     Nothing will happen when the button is clicked.
    /// </summary>
    None,
    /// <summary>
    ///     The user will be redirected to the specified URL when the button is clicked.
    /// </summary>
    Link,
    /// <summary>
    ///     The value of the button's <see cref="ButtonElement.Value"/> property will be submitted.
    /// </summary>
    ReturnValue
}