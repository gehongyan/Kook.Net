namespace Kook;

/// <summary>
///     Determines whether the object is deletable or not.
/// </summary>
public interface IDeletable
{
    /// <summary>
    ///     Deletes this object and all its children.
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    Task DeleteAsync(RequestOptions? options = null);
}
