using System.Text.Json;

namespace Kook;

/// <summary>
///     提供用于修改 <see cref="Kook.IUserMessage"/> 的属性。
/// </summary>
/// <seealso cref="Kook.IUserMessage.ModifyAsync(System.Action{Kook.MessageProperties},Kook.RequestOptions)"/>
public class MessageProperties
{
    /// <summary>
    ///     获取或设置要设置到此消息的消息内容。
    /// </summary>
    /// <remarks>
    ///     修改此值为非空字符串可以修改消息的内容；不修改此值或将其设置为 <c>null</c> 可以保持消息的原内容。
    /// </remarks>
    public string? Content { get; set; }

    /// <summary>
    ///     获取或设置要设置到此消息的卡片。
    ///     修改此值，或修改此 <see cref="System.Collections.Generic.IList{T}"/> 中的成员，可以修改消息中卡片的内容。
    /// </summary>
    public IList<ICard>? Cards { get; set; }

    /// <summary>
    ///     获取或设置要设置到此消息的消息引用。
    /// </summary>
    /// <remarks>
    ///     仅支持通过将此属性设置为 <c>MessageReference.Empty</c> 来清除消息引用，不支持更改现有消息引用。
    /// </remarks>
    /// <seealso cref="Kook.MessageReference.Empty"/>
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

    /// <summary>
    ///     获取或设置要为更新此消息生成内容时使用的模板的 ID。
    /// </summary>
    /// <remarks>
    ///     Kook.Net 无法通过网关或 API 获知消息是否使用了模板，无法获取消息的模板 ID。
    ///     因此如果要让新编辑的内容也使用模板，请在修改消息时手动设置此属性，且应重新为此属性赋值，不要尝试直接修改此属性中的成员。
    /// </remarks>
    public ulong? TemplateId { get; set; }

    /// <summary>
    ///     获取或设置要为更新此消息生成内容时使用的模板参数。
    /// </summary>
    /// <remarks>
    ///     Kook.Net 无法通过网关或 API 获知消息是否使用了模板，无法获取消息的模板参数。
    ///     因此在修改消息时，请重新为此属性赋值，不要尝试直接修改此属性中的成员。
    /// </remarks>
    public object? Parameters { get; set; }

    /// <summary>
    ///     获取或设置要为更新此消息序列化模板参数时使用的选项。
    /// </summary>
    /// <remarks>
    ///     Kook.Net 无法通过网关或 API 获知消息是否使用了模板，无法获取消息的模板参数。
    ///     因此在修改消息时，请重新为此属性赋值，不要尝试直接修改此属性中的成员。
    /// </remarks>
    public JsonSerializerOptions? JsonSerializerOptions { get; set; }
}

/// <summary>
///     提供用于修改 <see cref="Kook.IUserMessage"/> 的属性。
/// </summary>
/// <typeparam name="T"> 模板参数的类型。 </typeparam>
/// <seealso cref="Kook.IUserMessage.ModifyAsync{T}(System.Action{Kook.MessageProperties{T}},Kook.RequestOptions)"/>
public class MessageProperties<T> : MessageProperties
{
    /// <summary>
    ///     获取或设置要为更新此消息生成内容时使用的模板参数。
    /// </summary>
    /// <remarks>
    ///     Kook.Net 无法通过网关或 API 获知消息是否使用了模板，无法获取消息的模板参数。
    ///     因此在修改消息时，请重新为此属性赋值，不要尝试直接修改此属性中的成员。
    /// </remarks>
    public new T? Parameters
    {
        get => (T?)base.Parameters;
        set => base.Parameters = value;
    }
}
