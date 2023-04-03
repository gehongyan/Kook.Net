# Changelog

---

## v0.4.0 [2023-04-03]

### Update Path

Due to the following reasons, this version has made adjustments to the parameter types passed in some events
in `BaseSocketClient`:

- Some parameters are always present and do not need to be wrapped with `Cacheable`, such as `MessageDeleted`
  and `UserConnected`;
- The data sent by the gateway is incomplete, resulting in missing parameters in some events. These event parameters
  have been adjusted to entities wrapped with `Cacheable` to fix the issue of unknown entity IDs that cannot be obtained
  through the REST client API request for complete data;
- The original processing logic for incomplete data sent by the gateway was to obtain complete data by initiating API
  requests through the REST client, which may cause performance issues or timeouts in large servers, such
  as `GuildMemberOnline` and `GuildMemberOffline`. These event parameters have been adjusted to entities wrapped
  with `Cacheable`, and can be obtained on demand through the `GetOrDownloadAsync` method;
- Some event parameters are too specific, resulting in a mismatch of types and passing empty values, such
  as `ReactionAdded`;
- Some event parameters are too general, such as `MessageDeleted` and `UserConnected`, which can avoid unnecessary
  pattern matching;
- Some event parameters are missing, such as `UserBanned`;
- Some events pass excessively redundant parameters, such as `MessageButtonClicked`.
- The specific changes to the event parameters can be found in the appendix at the end of the document. All applications
  involving these events need to be updated accordingly.

All formatting methods in the KMarkdown formatting helper class `Format` have been changed to extension methods, and a
new optional parameter sanitize has been added to support whether to escape special characters in the text, which
defaults to true. The calling method of extension methods is still compatible with the original static method calling
method. By default, all formatting methods escape special characters in the text that conflict with KMarkdown syntax to
avoid syntax parsing errors. This feature is enabled by default and can be disabled through the sanitize parameter. If
the text parameter passed to this method has already escaped special characters, the sanitize parameter should be set to
false, or the input parameter should be adjusted to the original unescaped text.

The `Parse` and `TryParse` methods in the `CardJsonExtension` class have been renamed to `ParseSingle`
and `TryParseSingle` to avoid conflicts with the `ParseMany` and `TryParseMany` methods used to parse multiple cards.
All calls involving this method need to be updated accordingly.

The `Features` property type in `IGuild` and `IRecommendInfo` was originally `object[]`, and is now implemented as
the `GuildFeatures` type. All calls involving this property need to be updated accordingly.

The `RestPresence` namespace has been corrected to `Kook.Rest`. All calls involving `RestPresence` need to update the
namespace reference accordingly.

### Added

- Added methods for managing friends and blocked users. For details, see the appendix at the end of the document.
- Added debug display text for the `Cacheable` and `Quote` classes
- Added `MaxJoinedGuildDataFetchingRetryTimes` and `JoinedGuildDataFetchingRetryDelay` properties to `KookSocketConfig`
  for controlling the number of times and delay between retries for fetching data when joining a server
- Added `ParseMany` and `TryParseMany` methods to `CardJsonExtension`
- (Experimental feature) Added the `IVoiceRegion.MinimumBoostLevel` property
- (Experimental feature) Added `ValidateCardAsync` and `ValidateCardsAsync` methods to `KookRestClient`

### Changed

- Changed the parameter types for some events in `BaseSocketClient`. For details, see the appendix at the end of the
  document.
- The formatting methods in the `Format` helper class have been changed to extension methods, and a new optional
  sanitize parameter has been added to support escaping special characters in the text. The default value is true.
- Renamed the `Parse` and `TryParse` methods in `CardJsonExtension` to `ParseSingle` and `TryParseSingle`.
- Implemented the `Features` property of `IGuild` and `IRecommendInfo` as the `GuildFeatures` type.
- Corrected the namespace for `RestPresence` to `Kook.Rest`.
- (Experimental feature) The `KookRestClient.GetAdminGuildsAsync` method now supports bot authentication.

### Fixed

- Fixed an issue where the results of the `Format.Quote` and `Format.BlockQuote` methods were displayed incorrectly in
  KOOK
- Fixed an issue where the error message displayed when `CountdownModuleBuilder.Build` throws an exception was incorrect
- Fixed an issue where `BaseSocketClient.DirectMessageUpdated` might pass an incorrect user entity
- Fixed an issue where the `Author` property was null in message-related events when the user was not cached
- Fixed an issue where `IGuild.OwnerId` was `0`
- Fixed an issue where the `IsPinned` property was not set correctly in `BaseSocketClient.Pinned`
  and `BaseSocketClient.Unpinned` events
- Fixed an issue where the `IPresence.ActiveClient` property could be unintentionally cleared
- Fixed an issue where the debug display format of `IPresence` was incorrect
- Fixed an issue where the default implementation of `IRestClient` could throw an exception when `DEBUG_REST`
  preprocessor directive was enabled for debugging high-concurrency requests in the source code
- Fixed an issue where `Quote.Empty` was not a static property.

### Optimized

- Fixed an issue with missing preprocessor directives in `KookRestApiClient`.
- Optimized the `SocketUser.UpdateIntimacyAsync` method's implementation of the `IUser` interface.
- Improved the use of `NumberBooleanConverter`.
- `KookSocketClient` logs the content of received packets when they are out of order or not handled correctly.
- When printing exception packets, `KookSocketClient` uses the passed-in serializerOptions serialization options.
- Completed XML documentation for `BaseSocketClient` events.

### Misc

- Due to the issue of missing field values when creating roles being fixed on the KOOK server side, commit
  1726d5c7c36ead0544784eb937660537c6ec1f4f has been reverted.
- Corrected some errors in the documentation.
- Fixed the issue where the test server created by integration testing was not properly deleted.

### Appendix

Added APIs:

- Get all friends: `IKookClient.GetFriendsAsync`
- Request to add a friend: `IUser.RequestFriendAsync`
- Remove a friend: `IUser.RemoveFriendAsync`
- Get all friend requests: `IKookClient.GetFriendRequestsAsync`
- Accept a friend request: `IFriendRequest.AcceptAsync`
- Decline a friend request: `IFriendRequest.DeclineAsync`
- Get all blocked users: `IKookClient.GetBlockedUsersAsync`
- Block a user: `IUser.BlockAsync`
- Unblock a user: `IUser.UnblockAsync`

Event parameter changes in `BaseSocketClient`:

- `ReactionAdded` and `ReactionRemoved`
  - `Cacheable<IUserMessage, Guid>` → `Cacheable<IMessage, ulong>`
  - `Cacheable<IMessageChannel, ulong>` → `ISocketMessageChannel`
- `DirectReactionAdded` and `DirectReactionRemoved`
  - `Cacheable<IUserMessage, Guid>` → `Cacheable<IMessage, ulong>`
- `MessageDeleted`
  - `Cacheable<IMessageChannel, ulong>` → `ISocketMessageChannel`
- `MessageUpdated`
  - `Cacheable<IMessage, Guid>` → `Cacheable<SocketMessage, Guid>`
  - `SocketMessage` → `Cacheable<SocketMessage, Guid>`
- `MessagePinned` and `MessageUnpinned`
  - `Cacheable<IMessage, Guid>` → `Cacheable<SocketMessage, Guid>`
  - `SocketMessage` → `Cacheable<SocketMessage, Guid>`
  - `SocketGuildUser` → `Cacheable<SocketGuildUser, ulong>`
- `DirectMessageUpdated`
  - `SocketMessage` → `Cacheable<SocketMessage, Guid>`
- `UserLeft`
  - `SocketUser` → `Cacheable<SocketUser, ulong>`
- `UserBanned`
  - `IReadOnlyCollection<SocketUser>` → `IReadOnlyCollection<Cacheable<SocketUser, ulong>>`
  - `SocketUser` → `Cacheable<SocketUser, ulong>`
  - Add a new parameter of type `string` to represent the reason for adding to the blacklist.
- `UserUnbanned`
  - `IReadOnlyCollection<SocketUser>` → `IReadOnlyCollection<Cacheable<SocketUser, ulong>>`
  - `SocketUser` → `Cacheable<SocketUser, ulong>`
- `UserUpdated`
  - `SocketUser` → `Cacheable<SocketUser, ulong>`
- `GuildMemberUpdated`
  - `SocketGuildUser` → `Cacheable<SocketGuildUser, ulong>`
- `GuildMemberOnline` and `GuildMemberOffline`
  - `IReadOnlyCollection<SocketGuildUser>` → `IReadOnlyCollection<Cacheable<SocketGuildUser, ulong>>`
- `UserConnected` and `UserDisconnected`
  - `SocketUser` → `Cacheable<SocketGuildUser, ulong>`
  - Remove the `SocketGuild` parameter, please retrieve it from `SocketVoiceChannel.Guild`.
- `MessageButtonClicked`
  - `SocketUser` → `Cacheable<SocketGuildUser, ulong>`
  - `IMessage` → `Cacheable<IMessage, Guid>`
  - Remove the `SocketGuild` parameter, please retrieve it from `SocketVoiceChannel.Guild`.
- `DirectMessageButtonClicked`
  - `SocketUser` → `Cacheable<SocketUser, ulong>`
  - `IMessage` → `Cacheable<IMessage, Guid>`

## v0.3.1 [2023-03-19]

### Added

- Added `ICard.ToJsonString` and `ICardBuilder.ToJsonString` for cards JSON serialization;
  Added `CardJsonExtension.Parse` and `CardJsonExtension.TryParse` for cards JSON deserialization.
  (c076ce8feadf9c6aaa54b44bd68466b47d3767e4)

## v0.3.0 [2023-03-19]

### Update Path

This release changed the default value of `everyoneHandling` parameter in `IUserMessage.Resolve` to `TagHandling.Name`.
All usages of this method need to take note of this change.

### Added

- Added method overload `GetUserAsync(string,string,RequestOptions)` to the `IKookClient` interface and implemented it in `BaseSocketClient`.
  (6555c2ba0c9993b4523a95a8cbdc544474541984)
- Added support to disable Unicode bidirectional formatting of username strings via `KookConfig.FormatUsersInBidirectionalUnicode`.
  (bfc038ec4ce68124aa7eef94b421f6d0f1f941ce)

### Fixed

- Fixed issue with `IUserMessage.Resolve` incorrectly textifying mentions for @everyone and @here.
  (91c1175ad5eb22323cc1fe3a5cc93b9e8fc86ea5)
- Fixed deserialization issue with `IGuild.CreateRoleAsync` results. (1726d5c7c36ead0544784eb937660537c6ec1f4f)
- (Experimental feature) Fixed issue with `KookRestClient.CreateGuildAsync` not returning complete guild information.
  (2b5c59d60e8db309c30cf42509a9756625e98903)

### Misc

- Added XML documentation to all public APIs. (fe1e9bcd0b7bd9a0a99c850234beef1b4993977b, 2ad3301bcc06a731e9f57bbd542484a31e77c438)
- Applied code styling. (6325b16b9ad1f8f958a8eaaf125e7d34ae280a10)

## v0.2.5 [2023-03-16]

### Update Path

This release added a new optional parameter `Nullable<GameCreationSource>` for `Kook.GetGamesAsync`,
to support getting games with specified creation source. All usages of this method that pass positional arguments
to `RequestOptions` need to be updated.

### Added

- Added an optional `Nullable<GameCreationSource>` parameter for `Kook.GetGamesAsync`.
  (22b799ac87867590045c87771b0344acd3e80339)

## v0.2.4 [2023-03-09]

### Added

- Added support for parsing color type `ColorType` and gradient color information `GradientColor` for role `IRole`.
  (9414b94fb60dc6b45f14dc524acc75005071e37f)

## v0.2.3 [2023-01-19]

### Added

- Added `KookRestClient.GetAdminGuildsAsync` for Bearer token. (58a0966d930e16e37f9796d4bd947d2a24aaade2)
- Added `Format.Colorize` for KMarkdown text colorization. (acca2ae40b0977ebea237b8cd02195f8758df115)

### Fixed

- Fixed `GetGuildsAsync` downloads data too slowly on massive guilds. (83ba3cadc106c7496c1eb45114ba552cf6ced24e)
- Fixed `GetGuildsAsync` does not work on Bearer token. (e7641676ad8efc87e035fd256b57a46606f9e86f)
- Fixed potion of values are incorrect in `Color`. (e46cbc088a6d2c48deaee0fd1826ca389d656476)

## v0.2.2 [2022-12-30]

### Fixed

- Fixed an issue that `SocketTextChannel.SendCard(s)Async` does not assign `ephemeralUser` parameter well.
  (7511f113146f85c9f6157534ba2ce1444080b795)

## v0.2.1 [2022-12-25]

### Fixed

- Fixed incorrect JSON converter resulting in startup failure. (7955aefdfd61fcc28420e30640628890c15ba781)

## v0.2.0 [2022-12-25]

### Update Path

This release split out non-official listed endpoints implementation into a separate package, namely
`INestedChannel.SyncPermissionsAsync`. Developers who are using this endpoints should install the
Kook.Net.Experimental package.

### Removed

- Removed `INestedChannel.SyncPermissionsAsync` method on interface definition. Implementations are
  now available on the Kook.Net.Experimental package. (49f2d974640423bfff4dd0eeca3c8f2d9e7c7cd0)

### Added

- Added Kook.Net.Experimental package for non-official listed endpoints implementation.
  (49f2d974640423bfff4dd0eeca3c8f2d9e7c7cd0)

### Fixed

- Fixed potential NRE resulting from empty `IGuild.OpenId`. (f112a37073171fb02b79c46a56741a62f00461c2)

### Misc

- Fixed incorrect indentations. (d3e974bae04b3097554dd646117e502e4026d3e3)

## v0.1.2 [2022-12-18]

### Update Path

This release changed the type of `SocketGuild.MemberCount` from `int` to `int?`, where the `null` value represents
that the number of the guild members is unknown. In addition, similar changes occur on `SocketGuild.HasAllMembers`.
All usages relying on these two properties need to be updated.

### Changed

- `AlwaysDownloadUsers` will also define whether the number of guild members will be loaded upon startup
  (8f4fb79643eebd07f4736a681ddc30f3f04d6f20)

### Fixed

- Fixed incorrect preconditions when modifying voice channels (9f808b3a2153d2e0a37aceaad8097768288d43de)
- Fixed incorrect documentation (9e2669a56267970e4295555cf6e8840dfa28f6ab)

## v0.1.1 [2022-11-20]

### Added:

- Added `IGuild.GetActiveBoostSubscriptionsAsync()` (32b1617a2e8c6bb0fa86f3e2ebc40e97a203bf61)
- Payloads themselves will be logged as well when deserialization exceptions occur (369dd044f5bd4e94bec37c4ba8e4d76e586cd01d)

### Fixed

- Fixed `BaseSocketClient.UserUnbanned` that was not working (624750a0427925fb4b42e26720708b4cc9d60d1c)
- Fix `DownloadVoiceStatesAsync` and `DownloadBoostSubscriptionsAsync` were bypassed unexpectedly
  when `BaseSocketClient.GuildAvailable` is triggered (390b8ab65ad6cab22142c33ee51a62e54b3c1870)

## v0.1.0 [2022-11-15]

Initial release.
