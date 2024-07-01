namespace Kook.Rest;

/// <summary>
///     Provides extension methods of experimental functionalities for <see cref="KookRestClient"/>s.
/// </summary>
public static class KookRestClientExperimentalExtensions
{
    /// <summary>
    ///     Gets a collection of the available voice regions.
    /// </summary>
    /// <param name="client">The KOOK rest client instance.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection
    ///     with all of the available voice regions in this session.
    /// </returns>
    /// <remarks>
    ///     <note type="warning">
    ///         This method is still in experimental state, which means that it is not for official API implementation
    ///         usage, may violate the developer rules or policies, not guaranteed to be stable, and may be changed or removed in the future.
    ///     </note>
    /// </remarks>
    public static Task<IReadOnlyCollection<RestVoiceRegion>> GetVoiceRegionsAsync(this KookRestClient client, RequestOptions? options = null) =>
        ExperimentalClientHelper.GetVoiceRegionsAsync(client, options);

    /// <summary>
    ///     Gets a voice region.
    /// </summary>
    /// <param name="client">The KOOK rest client instance.</param>
    /// <param name="id">The identifier of the voice region (e.g. <c>eu-central</c> ).</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the voice region
    ///     associated with the identifier; <c>null</c> if the voice region is not found.
    /// </returns>
    /// <remarks>
    ///     <note type="warning">
    ///         This method is still in experimental state, which means that it is not for official API implementation
    ///         usage, may violate the developer rules or policies, not guaranteed to be stable, and may be changed or removed in the future.
    ///     </note>
    /// </remarks>
    public static Task<RestVoiceRegion?> GetVoiceRegionAsync(this KookRestClient client, string id, RequestOptions? options = null) =>
        ExperimentalClientHelper.GetVoiceRegionAsync(client, id, options);

    /// <summary>
    ///     Creates a guild for the logged-in user.
    /// </summary>
    /// <remarks>
    ///     This method creates a new guild on behalf of the logged-in user.
    ///     <note type="warning">
    ///         This method is still in experimental state, which means that it is not for official API implementation
    ///         usage, may violate the developer rules or policies, not guaranteed to be stable, and may be changed or removed in the future.
    ///     </note>
    /// </remarks>
    /// <param name="client">The KOOK rest client instance.</param>
    /// <param name="name">The name of the new guild.</param>
    /// <param name="region">The voice region to create the guild with.</param>
    /// <param name="icon">The icon of the new guild.</param>
    /// <param name="templateId">The identifier of the guild template to be used to create the new guild.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous creation operation. The task result contains the created guild.
    /// </returns>
    public static async Task<RestGuild> CreateGuildAsync(this KookRestClient client, string name,
        IVoiceRegion? region = null, Stream? icon = null, int? templateId = null, RequestOptions? options = null)
    {
        RestGuild guild = await ExperimentalClientHelper
            .CreateGuildAsync(client, name, region, icon, templateId, options)
            .ConfigureAwait(false);
        await guild.UpdateAsync().ConfigureAwait(false);
        return guild;
    }

    /// <summary>
    ///     Gets a collection of guilds where the current user has the
    ///     <see cref="GuildPermission.Administrator"/> permission.
    /// </summary>
    /// <param name="client">The KOOK rest client instance.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection
    ///     of guilds where the current user has the <see cref="GuildPermission.Administrator"/> permission.
    /// </returns>
    public static Task<IReadOnlyCollection<RestGuild>> GetAdminGuildsAsync(this KookRestClient client, RequestOptions? options = null) =>
        ExperimentalClientHelper.GetAdminGuildsAsync(client, options);

    /// <summary>
    ///     Validates a card.
    /// </summary>
    /// <param name="client">The KOOK rest client instance.</param>
    /// <param name="card">The card to be validated.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous validation operation.
    /// </returns>
    public static Task ValidateCardAsync(this KookRestClient client, ICard card, RequestOptions? options = null) =>
        ValidateCardsAsync(client, [card], options);

    /// <summary>
    ///     Validates a collection of cards.
    /// </summary>
    /// <param name="client">The KOOK rest client instance.</param>
    /// <param name="cards">The cards to be validated.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous validation operation.
    /// </returns>
    public static Task ValidateCardsAsync(this KookRestClient client, IEnumerable<ICard> cards, RequestOptions? options = null) =>
        ExperimentalClientHelper.ValidateCardsAsync(client, cards, options);

    /// <summary>
    ///     Validates a collection of cards.
    /// </summary>
    /// <param name="client">The KOOK rest client instance.</param>
    /// <param name="cardsJson">The JSON representation of the cards to be validated.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous validation operation.
    /// </returns>
    public static Task ValidateCardsAsync(this KookRestClient client, string cardsJson, RequestOptions? options = null) =>
        ExperimentalClientHelper.ValidateCardsAsync(client, cardsJson, options);
}
