namespace Kook;

/// <summary>
///     提供用于修改 <see cref="T:Kook.IUserMessage"/> 的属性。
/// </summary>
/// <seealso cref="M:Kook.IUserMessage.ModifyAsync(System.Action{Kook.MessageProperties},Kook.RequestOptions)"/>
public class MessageProperties
{
    /// <summary>
    ///     获取或设置要设置到此消息的消息内容。
    /// </summary>
    /// <remarks>
    ///     更改此值为非空字符串可以修改消息的内容；不更改此值或将其设置为 <c>null</c> 可以保持消息的原内容。
    /// </remarks>
    public string? Content { get; set; }

    /// <summary>
    ///     获取或设置要设置到此消息的卡片。
    ///     更改此值，或更改此 <see cref="T:System.Collections.Generic.IList`1"/> 中的成员，可以修改消息中卡片的内容。
    /// </summary>
    public IList<ICard>? Cards { get; set; }

    /// <summary>
    ///     获取或设置要设置到此消息的消息引用。
    /// </summary>
    /// <remarks>
    ///     仅支持通过将此属性设置为 <c>MessageReference.Empty</c> 来清除消息引用，不支持更改现有消息引用。
    /// </remarks>
    /// <seealso cref="P:Kook.MessageReference.Empty"/>
    public IQuote? Quote { get; set; }

    /// <summary>
    ///     获取或设置要临时更新到此消息的用户。
    /// </summary>
    /// <remarks>
    ///     设置此属性会使此用户在本次登录会话中看到的消息内容为本次临时修改的内容，
    ///     该用户在下次登录会话中看到的消息内容仍为原内容。 <br />
    ///     <note type="warning">
    ///         仅支持通过设置此属性为指定用户的 ID 来为该用户临时更新消息。 <br />
    ///         设置此属性无法将非临时消息更改为仅指定用户可见的临时消息，也无法为过去发送的临时消息更改可见用户。
    ///     </note>
    /// </remarks>
    public IUser? EphemeralUser { get; set; }
}
