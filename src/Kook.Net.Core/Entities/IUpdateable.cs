namespace Kook;

/// <summary>
///     Defines whether the object is updateable or not.
/// </summary>
public interface IUpdateable
{
    /// <summary>
    ///     Updates this object's properties with its current state.
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> A task that represents an asynchronous reloading operation. </returns>
    /// <remarks>
    ///     <note type="warning">
    ///         This method will fetch the latest data from REST API,
    ///         and replace the current object's properties with the new data.
    ///     </note>
    /// </remarks>
    Task UpdateAsync(RequestOptions? options = null);
}
