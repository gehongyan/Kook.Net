namespace KaiHeiLa
{
    /// <summary>
    ///     Defines whether the object is reloadable or not.
    /// </summary>
    public interface IReloadable
    {
        /// <summary>
        ///     Reload this object's properties with its current state.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents an asynchronous reloading operation.
        /// </returns>
        /// <remarks>
        ///     <note type="warning">
        ///         This method will fetch the latest data from REST API,
        ///         and replace the current object's properties with the new data.
        ///     </note>
        /// </remarks>
        Task ReloadAsync(RequestOptions options = null);
    }
}
