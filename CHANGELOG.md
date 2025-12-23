# Changelog

---

## v0.10.4 [2025-12-23]

### Added

- Added support for disconnecting guild users from voice channels.
- Added support for automatically passing the `reply_msg_id` parameter when modifying messages, used for daily message
  send count quota consumption discount.

## v0.10.3 [2025-11-15]

### Fixed

- Resolved an issue where `ModifyPermissionOverwriteAsync` could fail in certain scenarios resulting from
  `AddPermissionOverwriteAsync`.

### Misc

- Fixed base image versions in `Dockerfile`s in sample projects.

## v0.10.2 [2025-11-11]

### Fixed

- Resolved an issue where injected debugging methods in `KookDebuggers` caused application crashes when exceptions were
  thrown.

### Changed

- Moved the configuration option `AutoLogoutBeforeLogin` to `KookSocketConfig`.

### Misc

- Added builds targeting .NET 10 and upgraded the target framework of the sample and test projects to .NET 10.
- Migrated to the `slnx` solution format.
- Migrated to CPM (centralized package management).
- Updated NuGet dependencies.

## v0.10.1 [2025-09-19]

### Added

- Added `KookRestConfig.AutoLogoutBeforeLogin`, which configures whether to force logout other clients connected to the
  KOOK gateway when logging into the `BaseKookClient` client. This ensures that the current client's connection will not
  miss any business events due to connections from other clients.
- Added support for pinning and unpinning messages in text channels. You can use methods such as
  `IUserMessage.PinAsync`, `IUserMessage.UnpinAsync`, `ITextChannel.PinMessageAsync`, and
  `ITextChannel.UnpinMessageAsync` to perform these operations.
- `MessageReference` has a new property `ReplyMessageId`, which is used for daily message send limit discount. If the
  `replyMessageId` parameter is left empty in the constructor, it will automatically use the value of the
  `quotedMessageId` parameter.

## v0.10.0 [2025-07-23]

### Update Path

This update includes some breaking changes. Please review the "Changed" section carefully.

### Added

- Added static class `KookDebugger` to support outputting debugging information related to Kook.Net's underlying API
  requests, gateway payloads, rate limiting, and voice logs for interactions with the KOOK server.
- Added support for publishing, retrieving, and deleting threads, posts, and replies within thread channels
  (`IThreadChannel`). Also added support for retrieving the default layout (`DefaultLayout`) and default sort mode
  (`DefaultSortMode`) of thread channels.
- `ThreadExtensions` now includes the extension method `GetJumpUrl` to retrieve jump links for thread-related entities.
- Added support for retrieving the currently effective gateway intents via `KookSocketClient.GatewayIntents`.
- Added the error code enum value `KookErrorCode.ThreadContentAuditing`.
- `ImageElement` and `ImageElementBuidler` now support the `FallbackUrl` property.
- Added support for the `RecordAudio` permission value in server and channel permission enums.
- (Experimental Feature) Added support for searching thread topic tags by keyword via `QueryThreadTagsAsync`.

### Changed

- Changed the enum values of `VoiceQuality` to use names that are independent of specific bitrate values.
- Moved `Kook.Commands.MessageExtensions.TryExpandCardContent` to `Kook.MessageExtensions.TryExtractCardContent`.
- `IPermissionOverwrite<TTarget>` has been changed to `IPermissionOverwrite<TTarget, TId>`. Now, `TTarget Target`
  represents the target entity type for the permission overwrite, `TId TargetId` represents the type of the target
  entity's ID, and `PermissionOverwriteTarget TargetType` indicates the type of the target.
- For entities or data types returned by the KOOK API or delivered by the gateway, properties of type `DateTimeOffset`
  within interfaces or classes such as `IThread`, `IThreadPost`, `IThreadReply`, `IMessage`, `IGuildUser`, `IIntimacy`,
  `IIntimacyRelation`, `IInvite`, `CountModule`, `Quote`, and `BoostSubscriptionMetadata` now default to the local
  machine's time zone.

### Fixed

- Fixed issues with methods for adding and removing guild bans failing.

### Optimized

- The type for gateway payloads has been changed to `JsonElement` to avoid an extra level of boxing.
- Gateway payload deserialization now uses asynchronous methods.
- The `Card.Build` method now validates that the `Theme` is `Invisible` only for module types that support borderless
  cards.

### Misc

- Added quick reference documentation for thread-related interfaces.

## v0.9.11 [2025-06-25]

### Added

- Added support for message template related APIs.
- `Debug` mode log level now outputs the gateway session ID.

### Changed

- Changed the `templateId` parameter type in message sending methods from `int` to `ulong` to support a larger message
  ID range.

### Fixed

- Fixed an issue where changes in the gradient color data structure for roles caused application startup failures.

### Misc

- Added a quick reference document for the message template API.

## v0.9.10 [2025-06-18]

### Fixed

- Fixed an issue where the text command framework could not correctly parse file, audio, and video card modules as `Uri`
  parameters.

## v0.9.9 [2025-06-01]

### Fixed

- Fixed an issue where enum parameters are not correctly registered in the text command framework.

## v0.9.8 [2025-05-20]

### Added

- Added `BaseSocketClient.UnknownDispatchReceived` for handling unknown event types.
- Added `KookSocketConfig.SuppressUnknownDispatchWarnings` to suppress warnings for unknown event types.

### Fixed

- Fixed an issue where `RelationType` and `CreatedAt` on `IIntimacyRelation` could be null, causing problems retrieving
  intimacy relation information. These properties are now nullable.
- Fixed a potential issue where `SocketMessage.Update` could block asynchronous threads abnormally.

### Misc

- Updated related dependency versions.
- Migrated the test framework to xunit.v3.

## v0.9.7 [2025-03-09]

### Added

- Added support for multi-instance injection of Webhook clients.

### Fixed

- Fixed an issue where the overload method of `AddKookAspNetWebhookClient` receiving `KookAspNetWebhookConfig` failed to
  properly register client instances.

### Misc

- Removed dependency on `FluentAssertions`.
- Fixed null reference warnings in F# sample code.
- Optimized entity creation process in integration test projects by using `IAsyncLifetime`.
- Improved the OAuth sample project by providing example code for retrieving joined guilds.

## v0.9.6 [2025-01-05]

### Added

- Added support for managing intimate relationships.
- The pipe message client now supports sending card messages, as well as quoting messages and ephemeral user.
- Added the `Modules` property to the `ICard` interface.

### Fixed

- Corrected the rate value for preemptive rate limiting.
- Fixed an issue where the maximum capacity of the message cache could overflow.
- Fixed an issue where the cache was not properly updated when a guild channel was deleted.

### Misc

- Removed support for the .NET 6.0 target framework. Updated the target framework for dependency injection, message
  queue, and hosting extension packages to .NET 9.0.
- Updated all externally referenced NuGet packages to their latest versions.

## v0.9.5 [2024-11-13]

### Added

- Added the Kook.Net.Pipe package, which provides `KookPipeClient` for sending pipe messages.
- Added `TokenType.Pipe`, but it is only used to indicate the internal API client to log in with a pipe token type
  and should not be used for client login in user code.

### Misc

- Removed the target framework .NET 7.0, added the target framework .NET 9.0.

## v0.9.4 [2024-11-12]

### Added

- Added support for sending and modifying template messages.
    - Overloads for `IMessageChannel.SendTextAsync`, `IMessageChannel.SendCardsAsync`, `IDMChannel.SendTextAsync`,
      and `IDMChannel.SendCardsAsync` methods now accept parameters `templateId`, `parameters`, and
      `jsonSerializerOptions` for sending template messages.
    - Added `TemplateId`, `Parameters`, and `JsonSerializerOptions` properties to `MessageProperties` for modifying
      template messages.
    - Added `MessageProperties<T>` class to support passing generic-typed `parameters` arguments.
- Text command framework now supports keyed services. The `FromKeyedServicesAttribute` can be used to obtain keyed
  service instances.

### Fixed

- Fixed the issue where `SocketGuild.CurrentUser` was null.
- Fixed an issue where direct messages might not be edited correctly.

## v0.9.3 [2024-10-27]

### Added

- Added basic support for thread channels. Introduced the thread channel type `ChannelType.Thread`, and related entities
  `IThreadChannel`, `RestThreadChannel`, `SocketThreadChannel`. Added support for operations to get, create, modify,
  and delete thread channels.
- Added permissions `GuildPermission.ReplyToPost` and `ChannelPermission.ReplyToPost` to represent the permission to
  reply to posts, and added support for post channel permissions in methods related to permissions.
- Added `SocketRole.Members`, which can be used to retrieve all users with a specified role from the cache.

### Fixed

- Fixed the issue where the `SocketGuildUser.RemoveRolesAsync` method did not respect `RequestOptions`.
- Fixed the issue where `IChannel.Name` may be null.

## v0.9.2 [2024-10-18]

### Added

- Support sending borderless theme `CardTheme.Invisible` cards
- `DefaultRestClient` supports passing an `IWebProxy` instance
- Implemented `IEquatable<T>` interface for `Color`, `AlphaColor`, and `GradientColor`

### Fixed

- Fixed symbol reference errors in some XML documents
- Fixed issue where attempting to get the message entity from a cacheable object returned by the send message method
  resulted in null

### Other

- Enabled SourceLink

## v0.9.1 [2024-09-07]

### Fixed

- Fix the issue where the channel creator field being an empty string causes startup failure.

## v0.9.0 [2024-08-30]

### Update Roadmap

This update introduces the following new packages:

- `Kook.Net.Webhook`: Webhook support package
- `Kook.Net.Webhook.HttpListener`: Webhook implementation package for HTTP Listener
- `Kook.Net.Webhook.AspNet`: Webhook implementation package integrated with ASP.NET
- `Kook.Net.MessageQueue.InMemory`: In-memory message queue support package
- `Kook.Net.MessageQueue.MassTransit`: MassTransit message queue support package
- `Kook.Net.DependencyInjection.Microsoft`: Dependency injection extension package for
  Microsoft.Extensions.DependencyInjection
- `Kook.Net.Hosting`: Hosting service extension package

The `Kook.Net` package now references the `Kook.Net.Webhook` package and has removed the reference to the
`Kook.Net.CardMarkup` package. If you need to continue using the functionality for building card messages via markup
language, please reference the `Kook.Net.CardMarkup` package separately.

Voice connection and streaming features have been changed to be implemented by an officially supported method. The newly
added API for receiving voice data streams is experimental and not officially supported.

The exception thrown by `Emote.Parse` when an error occurs has been changed from `ArgumentException` to
`FormatException`. The return result of `Emote.ToString` has been changed to be equivalent to the result of
`ToKMarkdownString`. The link format provided by `MessageExtensions.GetJumpUrl` has been adjusted.
`SocketGuild.ValidBoostSubscriptions` has been changed to `SocketGuild.ActiveBoostSubscriptions`. `Tag<T>` has been
changed to `Tag<TKey, TValue>`.

### Added

- Added support for custom message queues, with the default implementation being synchronous message processing. Support
  for setting the message queue provider by installing extension NuGet packages and configuring
  `KookSocketConfig.MessageQueueProvider`. `Kook.Net.MessageQueue.InMemory` is the in-memory queue support package, and
  `Kook.Net.MessageQueue.MassTransit` is the MassTransit queue support package. Refer to the examples for usage.
- Added support for Webhook mode, which is currently implemented based on the Socket implementation. The Webhook
  integrated by Kook.Net is the abstract class `KookWebhookClient`. `Kook.Net.Webhook.HttpListener` is the Webhook
  implementation package for HTTP Listener, and `Kook.Net.Webhook.AspNet` is the Webhook implementation package
  integrated with ASP.NET. Refer to the examples for usage.
- Added extension methods package for the Microsoft.Extensions.DependencyInjection dependency injection framework
  `Kook.Net.DependencyInjection.Microsoft`, to support quick addition of various clients from Kook.Net, refer to the
  examples for usage.
- Added hosting service extension package based on `IHost` and `IHostedService` `Kook.Net.Hosting`, to support quick
  addition of various clients from Kook.Net as hosting services, refer to the examples for usage.
- Added support for receiving voice data stream related APIs. (Experimental feature, not officially supported)
- The text command framework has added built-in support for parameter parsing of `DateOnly` and `TimeOnly` types, added
  support for parameter parsing of `Uri` type, and added command parsing for mixed text-image messages.
- Added extension method `MaybeTextImageMixedMessage` on `IUserMessage` to determine if it might be a mixed text-image
  message.
- Made the constructor of `Emote` public.
- Added `KookComparers` class to support KOOK entities comparison by ID.
- Added `Parse` and `TryParse` methods to the `Color` class.
- `IKookClient` provides `LoginAsync` and `LogoutAsync` methods.
- Added `TagUtil` to convert `ITag` to `Tag<TKey, TValue>`.

### Fixed

- Fixed the issue where the `AudioClient.ClientDisconnected` event was not correctly raised.
- Fixed the issue where `IsOwner` on `Rest/SocketGuildUser` could be determined but the value was `null`.
- Fixed the issue where `RequireRoleAttribute` could be incorrectly added to inappropriate targets.
- Fixed the issue where the server member update event threw an exception when the nickname parameter was not carried.
- Fixed the issue where the implementation on `IGuild` did not expose the `IsAvailable` property.
- Fixed the issue where the values of `EveryoneMention` and `HereMention` in `ITag` might be `0` instead of `0U`.

### Changed

- After user code manipulates server member roles via API, the framework will attempt to update the cache to get as
  accurate role information as possible without updating user role information via API.
- The exception thrown by `Emote.Parse` when an error occurs has been changed from `ArgumentException` to
  `FormatException`.
- The return result of `Emote.ToString` has been changed to be equivalent to the result of `ToKMarkdownString`.
- Voice connection and streaming features have been changed to be implemented by an officially supported method.
- Changed the link format provided by `MessageExtensions.GetJumpUrl`, adjusted the server channel to the newly supported
  official link format, and adjusted the parameters in private chat channels to use chat codes.
- Changed `Tag<T>` to `Tag<TKey, TValue>`.
- Adjusted the result of `IUserMessage.Resolve` to be more in line with the KMarkdown format.
- Renamed the first parameter on `ModuleBase.ReplyTextAsync` to `text`.
- Renamed `SocketGuild.ValidBoostSubscriptions` to `SocketGuild.ActiveBoostSubscriptions`.
- The `Kook.Net` package no longer includes a reference to `Kook.Net.CardMarkup`.

### Removed

- Due to the deactivation of many unofficial interfaces, most APIs on Kook.Net.Experimental have been removed.

### Misc

- XML documentation has been rewritten in Simplified Chinese.
- Added usage examples for MessageQueue, Webhook, and OAuth.
- Added Simplified Chinese README.

## v0.8.0 [2024-05-28]

### Update Roadmap

Due to changes in the KOOK API, Bot users can no longer obtain all the necessary basic guild information at startup
through the `/guild/index` API. Instead, it is now required to traverse each guild via the `/guild/view`
API. This change can result in a significantly longer startup time and a large number of API requests for Bots
that have joined many guilds. Therefore, the current version introduces the `KookSocketConfig.StartupCacheFetchMode`
configuration item, which defines how the Bot loads the basic guild data needed at startup.

- `Synchronous`: In synchronous mode, after obtaining a simple list of guilds at client startup, the client fetches the
  basic data of each guild through the API before triggering the `Ready` event.
- `Asynchronous`: In asynchronous mode, after obtaining a simple list of guilds at client startup, the `Ready` event is
  triggered immediately, and a background task is started to fetch all the basic guild data.
- `Lazy`: In lazy mode, after obtaining a simple list of guilds at client startup, the `Ready` event is triggered
  immediately without proactively fetching the basic guild data. When events involving the guild are received from the
  gateway, the guild's basic data will be fetched through the API if it has not already been obtained.
- `Auto`: In automatic mode, the default setting, the client's startup mode is automatically determined based on the
  number of guilds the Bot has joined. If the number of guilds reaches `LargeNumberOfGuildsThreshold` (default is 50),
  it will be `Lazy`; if it reaches `SmallNumberOfGuildsThreshold` (default is 5), it will be `Asynchronous`; otherwise,
  it will be `Synchronous`. This determination is made each time the Bot connects to the WebSocket.

When not using `Synchronous` mode, after the `Ready` event, accessing cached guild entities might result in entities
that do not fully contain basic guild data. The `IsAvailable` property indicates whether the guild entity has fully
cached basic data through the API. In such cases, please proactively call the `UpdateAsync` method to update the cached
guild entity through the API. The basic guild data mentioned above mainly includes the guild's channels, roles,
channel permission overrides, and the current user's nickname within guilds.

The entire framework code has been updated to support nullable reference static analysis diagnostics. For the concept of
nullable reference types in C#, please refer to [Nullable reference types - C# | Microsoft Learn]. After updating to the
current version, all values that may be null will be marked as nullable types. This may cause some code to generate
warnings during compilation, which should be treated as potential null reference exceptions and fixed accordingly.

Additionally, `IQuote` has a new implementation `MessageReference`, which only contains the ID of the message to be
referenced and is used when calling the API in user code. Existing user code that creates `Quote` should migrate
to `MessageReference` as soon as possible.

`fileName` has been renamed to `filename`; the event parameter `Cacheable<SocketMessage, Guid>` has been changed
to `Cacheable<IMessage, Guid>`; `SectionAccessoryMode.Unspecified` is now replaced by `null`; `Format.StripMarkDown` has
been renamed to `StripMarkdown`; and the `filename` parameter in the `SendFileAsync` overload that accepts the `Stream`
type is now mandatory. Please note that these changes may cause compilation errors and should be fixed accordingly.

### Additions

- `KookSocketConfig` adds the `StartupCacheFetchMode`, `LargeNumberOfGuildsThreshold`,
  and `SmallNumberOfGuildsThreshold` configuration items to customize how the Bot's Socket client fetches the basic
  guild data needed at startup via the API.
- Two new configuration properties `AutoUpdateRolePositions` and `AutoUpdateChannelPositions` have been added
  to `KookSocketConfig`, defaulting to `false`. When enabled, the client will automatically fetch data via the API upon
  receiving related events to maintain the cached role and channel sorting information.
- `Embed` adds `CardEmbed`.
- Card entities and builders now implement `IEquatable<T>`.
- `SocketSelfUser` now implements `IUpdateable`.
- Added `IGuild.RecommendInfo.Certifications`.
- `IQuote` has a new implementation `MessageReference`, which only contains the ID of the message to be referenced and
  is used when calling the API in user code.
- Support for the event types `embeds_append`, `sort_channel`, `updated_server_type`, `batch_added_channel`,
  `batch_updated_channel`, `batch_deleted_channel`, `live_status_changed`, `PERSON` typed `updated_guild`,
  `add_guild_mute`, `delete_guild_mute`, `unread_count_changed` has been added, but it is not yet confirmed
  whether these events will actually be dispatched.

### Fixes

- Fixed the issue where the author of private messages was incorrect.
- Fixed the issue where `SocketUserMessage.Quote.Author` could be null.
- Fixed the issue where Tags were missing corresponding values when referencing nonexistent entities in messages.
- Fixed the issue where the voice client failed to handle undefined events, causing stream crashes.
- Fixed the issue where parsing newly introduced mixed media messages failed.
- Corrected the behavior of updating user nicknames.

### Changes

- Enabled nullable reference types feature. For the concept of nullable reference types in C#, please refer
  to [Nullable reference types - C# | Microsoft Learn].
- Various validations for the card builder are now deferred to the `Build` call.
- The types of properties involving lists in cards have been changed to `IList<T>`.
- `Quote.Empty` and its public constructor have been marked as `Obsolete`, and `MessageReference` should be used
  instead.
- `fileName` has been renamed to `filename`.
- The `filename` parameter in the `SendFileAsync` overload that accepts the `Stream` type is now mandatory.
- `BaseSocketClient._baseConfig` has been renamed to `BaseConfig`.
- The event parameter `Cacheable<SocketMessage, Guid>` has been changed to `Cacheable<IMessage, Guid>` to address the
  issue of entity download failure.
- `SectionAccessoryMode.Unspecified` has been removed; please use `null` instead.
- `Format.StripMarkDown` has been renamed to `StripMarkdown`, and the original method has been marked as `Obsolete`.
- `Format.StripMarkdown` now removes hyphens `-`.

### Others

- Added a reference to `PolySharp` on .NET 7 and earlier target frameworks to support the implementation of some new
  features on older frameworks.
- Added integration tests for some Socket events.

[Nullable reference types - C# | Microsoft Learn]: https://learn.microsoft.com/en-us/dotnet/csharp/nullable-references

## v0.7.0 [2024-04-02]

### Update Roadmap

The KOOK client now supports sending messages within voice channels. As a result, the following APIs have been modified:

- `CreateVoiceChannelProperties` now inherits from `CreateTextChannelProperties`.
- `ModifyVoiceChannelProperties` now inherits from `ModifyTextChannelProperties`.
- `IVoiceChannel` now inherits from `ITextChannel`.
- The value of `ChannelPermissions.Voice` now includes permissions for text channels.
- The `SocketTextChannel.GetMessagesAsync` series of methods are now virtual methods, to be overridden
  by `SocketVoiceChannel`.

It's important to note that, due to limitations in the KOOK API, fetching message history and pinning messages are not
supported within voice channels. Therefore, calling `GetMessagesAsync` and `GetPinnedMessagesAsync` methods from
`IMessageChannel` on voice channels is not supported.

Additionally, while voice channels support operations on `Topic` and `SlowModeInterval` at the API level, the KOOK
client
currently does not reflect these capabilities.

Creating channels does not immediately support specifying a Topic. The `Topic` property in `CreateTextChannelProperties`
has been removed and does not take effect. Please use the `ITextChannel.ModifyAsync` method to modify the channel after
creation.

### Added

- Added support for defining card messages via XML.
- `IVoiceChannel` now inherits from `ITextChannel`, with relevant implementations updated.
- Added `SentRequest` event to `BaseKookClient`.
- Added non-null result prompts for parsing card message-related `Try*` methods.

### Fixed

- Corrected the issue with `ConnectionManager` incorrectly updating `State` when `Disconnected`.

### Changes

- Removed the `Topic` property from `CreateTextChannelProperties`.

### Misc

- Marked the project as not supporting AOT and assembly trimming.
- Added example project `Kook.Net.Samples.CardMarkup`.
- Added documentation for defining card messages via XML.
- Added Logo.

## v0.6.0 [2024-02-28]

### Update Roadmap

In KOOK, mentioning voice channels in text messages is not supported, meaning `IVoiceChannel` should not be derived from
`IMentionable` interface. Incorrect usage of mentioning `IVoiceChannel` should be removed or modified.

The name of the cancellation token has been changed from `CancelToken` to `CancellationToken`. Existing methods,
variables, properties, and parameters related to cancellation token should be updated.

### Added

- Added connection and streaming for voice channels.

### Fixed

- Fixed an issue where `KookSocketClient` did not correctly handle `ConnectionState` when casted to `IKookClient` or
  `BaseKookClient`.

### Changes

- `IVoiceChannel` no longer derived from `IMentionable` interface.
- Renamed the name of methods, variables, properties, and parameters from `CancelToken` to `CancellationToken`.

### Misc

- Updated document template, added example project page, and added voice streaming documentation.

## v0.5.5 [2024-02-02]

### Update Path

`RequireUserAttribute` no longer supports construction via the `IUser` parameter; please use `IUser.Id` instead.

### Added

- Added `RequireRoleAttribute` as a precondition in the text command framework.
- Added `Aliases`, `Summary`, and `Remarks` properties, and optional parameters to `CommandAttribute` in the text
  command framework.

### Fixed

- Fixed an issue where exceptions during data download at Bot startup were not properly outputted to the log.

### Changed

- Removed the unrealistic constructor on `RequireUserAttribute` that accepted an `IUser` parameter.

## v0.5.4 [2024-01-06]

### Update Path

The set accessibility of the `AuthTokenType` and `AuthToken` properties in the `KookRestApiClient` has been changed to
private. Modifying these properties directly in user code may result in framework runtime exceptions. If you need to log
in with a different authentication on the KOOK gateway, please re-login using `LoginAsync` after calling `LogoutAsync`.

### Added

- Added `HasAnnualBuff`, `IsSystemUser`, and `Nameplates` properties to the `IUser` interface.
- Added `BackgroundColor` property to the `UserTag`.
- Added support for parsing interactive emojis.

### Changed

- Changed the set accessibility of the `AuthTokenType` and `AuthToken` properties in the `KookRestApiClient` to private.

## v0.5.3 [2023-11-15]

### Update Path

The `IVoiceChannel.ServerUrl` property has been moved to the `IAudioChannel` interface.
All usages of this property need to be updated.

### Added

- Added voice region properties to the `IAudioChannel` interface.
- Added support for modifying voice region properties in the `IVoiceChannel.ModifyAsync` method.
- Added parameterized constructors to card, module, and element builders.

### Changed

- The `IVoiceChannel.ServerUrl` property has been moved to the `IAudioChannel` interface.
- Removed unintentionally provided public set accessors on some properties.

### Fixed

- Fixed incorrect build validation conditions for `SectionModuleBuild`.

### Misc

- Replaced the Moq testing mock framework with NSubstitute.
- Added Docker usage examples.
- Added support for targeting .NET 8.

## v0.5.2 [2023-08-18]

### Update Path

The parameter `Action<string>` of the `ModifyEmoteNameAsync` method has been changed to `string`. All usages of this
method need to be updated.

### Fixed

- Fixed an issue where incorrect comparison of `SocketReaction` caused the reaction cannot be removed from the cached
  message when a reaction is removed.
- Fixed an issue where the Socket client did not correctly send the close code when disconnecting.
- Fixed an issue where the results were incorrect when querying messages after a reference message in
  the `GetMessagesAsync` method.
- Fixed an issue where the `ModifyEmoteNameAsync` method did not correctly rename guild emotes.

### Misc

- Completed the quick reference guide.
- Added actual functionality to the sample program `SimpleBot`, which can respond to a simple command, reply with
  buttons, and respond to button click events.
- Added implementation examples for Visual Basic and F#.

## v0.5.1 [2023-07-29]

### Added

- Introduced new methods `SocketGuild.GetCategoryChannel` and `RestGuild.GetCategoryChannelAsync`.

### Fixed

- Resolved an issue where the `GetJumpUrl` method was returning incorrect results.
- Corrected the value of `KookConfig.MaxMessagesPerBatch` that was not being set correctly.
- Fixed the incorrect deserialization of deleted quotes.
- Addressed the problem where the `RequestOptions` parameter of `MoveUsersAsync` was not optional.
- Fixed the failure of the `MoveUsersAsync` method.
- Fixed an issue where modifying channel permissions immediately after creating them on a `SocketGuildChannel`
  could result in failure without any error information.

### Optimized

- Optimized the internal implementation of the `GetDirectMessageAsync` method.
- Ensured consistent string representation for `langword` in comments.

## v0.5.0 [2023-05-21]

### Update Path

For the sake of convenience, this version has made adjustments to the parameter types passed in some events
in `BaseSocketClient`. Please refer to the appendix at the end of the document for specific changes to event parameters.
Applications involving these events need to be updated accordingly.

The enum values and properties representing key accounts in `GuildFeature` and `GuildFeatures` have been renamed
for more accurate naming. `GuildPermissions` has been refactored as a struct, and the type of
the `RoleProperties.Permissions` property has been changed accordingly. The type of `RestGuild.Channels` was incorrectly
declared as a dictionary with values of type `RestChannel`, and has been corrected to a dictionary with values of
type `RestGuildChannel`. The return type of some methods on `KookSocketClient` was previously `ValueTask`, but has now
been unified to `Task`. Usages involving these APIs need to be updated accordingly.

### Added

- Added `GuildFeature.Partner` enum value and `GuildFeatures.IsPartner` property.
- Added `IGuild.Banner` property.
- Added `CurrentUserNickname`, `CurrentUserDisplayName`, and `CurrentUserRoles` properties on `RestGuild`.
- Added `SyncPermissionsAsync` method on `INestedChannel`.
- Added `DownloadVoiceStatesAsync` and `DownloadBoostSubscriptionsAsync` abstract methods on `BaseSocketClient`.
- Added `TextChannels`, `VoiceChannels`, and `CategoryChannels` properties on `RestGuild`.

### Changed

.

- Renamed `GuildFeature.Ka` to `GuildFeature.KeyAccount` and `GuildFeatures.IsKa` to `GuildFeatures.IsKeyAccount`.
- Changed `GuildPermissions` to a struct and changed `RoleProperties.Permissions` to type `GuildPermissions?`.
- For convenience, some events in `BaseSocketClient` have been changed or added with event parameters. See the appendix
  at the end of the document for details..
- Changed all parameters of `DownloadBoostSubscriptionsAsync` method on `BaseSocketClient` to optional parameters.
- Changed the type of `RestGuild.Channels` to `ImmutableDictionary<ulong, RestGuildChannel>`.
- Changed the return type of `GetChannelAsync`, `GetDMChannelAsync`, `GetDMChannelsAsync`, and `GetUserAsync` methods
  on `KookSocketClient` to `Task<*>`.

### Fixed

- Fixed an issue where some APIs were unable to correctly parse the string `1` or `0` when returning a bool type.
- Fixed an issue where `IGuild.DefaultChannelId` was not correctly set to the actual default text channel configured on
  the server.
- Fixed an issue where `SocketGuild.CurrentUser` was null when downloading the server user list.
- Fixed an issue where `IsOnline` and `ActiveClient` on `SocketUser` could throw null reference exceptions.
- Fixed an issue where messages of type `MessageType.Poke` were not correctly parsed.
- Fixed an issue where the request bucket was not handling HTTP 429 Too Many Requests errors correctly.

### Optimized

- Optimized the debugger display text of `Cacheable`.
- `FileAttachment.Stream` can now be reused.
- The `SendFileAsync` and `ReplyFileAsync` methods will now cache the asset URI created for `FileAttachment` attachments
  created through files or streams before multiple sends, to avoid uploading the same file repeatedly.

### Misc

- Fixed some strange code indentation.
- Changed the document theme.
- Added a separate workflow for updating the documentation that depends on the `doc` branch.
- Added an API quick reference document.
- Added missing permission values to permission-related unit tests.

### Appendix

Event parameter changes in `BaseSocketClient`:

- `ReactionAdded` and `ReactionRemoved`
    - `ISocketMessageChannel` → `SocketTextChannel`
    - Add `Cacheable<SocketGuildUser, ulong>` representing the user who added or removed the reaction

- `DirectReactionAdded` and `DirectReactionRemoved`
    - Add `Cacheable<SocketUser, ulong>` representing the user who added or removed the reaction

- `MessageReceived`
    - Add `SocketGuildUser` representing the user who sent the message
    - Add `SocketTextChannel` representing the text channel where the message was sent

- `MessageDeleted`、`MessageUpdated`、`MessagePinned` 与 `MessageUnpinned`
    - `ISocketMessageChannel` → `SocketTextChannel`

- `DirectMessageReceived`
    - Add `SocketUser` representing the user who sent the message
    - Add `SocketDMChannel`representing the DM channel where the message was sent

- `DirectMessageDeleted`
    - `Cacheable<IDMChannel, Guid>` → `Cacheable<SocketDMChannel, ulong>`
    - Add `Cacheable<SocketUser, ulong>` representing the user who sent the message

- `DirectMessageUpdated`
    - `IDMChannel` → `Cacheable<SocketDMChannel, ulong>`
    - Add `Cacheable<SocketUser, ulong>` representing the user who sent the message

## v0.4.1 [2023-04-05]

### Fixed

- Fixed an issue where the corresponding permission overwrites of `SocketGuildChannel.UserPermissionOverwrites` and
  `SocketGuildChannel.RolePermissionOverwrites` were not removed when a user left the guild or a role was deleted.

## v0.4.0 [2023-04-03]

### Update Path

Due to the following reasons, this version has made adjustments to the parameter types passed in some events
in `BaseSocketClient`:

- Some parameters are always present and do not need to be wrapped with `Cacheable`, such as `MessageDeleted`
  and `UserConnected`.
- The data sent by the gateway is incomplete, resulting in missing parameters in some events. These event parameters
  have been adjusted to entities wrapped with `Cacheable` to fix the issue of unknown entity IDs that cannot be obtained
  through the REST client API request for complete data.
- The original processing logic for incomplete data sent by the gateway was to obtain complete data by initiating API
  requests through the REST client, which may cause performance issues or timeouts in large servers, such
  as `GuildMemberOnline` and `GuildMemberOffline`. These event parameters have been adjusted to entities wrapped
  with `Cacheable`, and can be obtained on demand through the `GetOrDownloadAsync` method.
- Some event parameters are too specific, resulting in a mismatch of types and passing empty values, such
  as `ReactionAdded`.
- Some event parameters are too general, such as `MessageDeleted` and `UserConnected`, which can avoid unnecessary
  pattern matching.
- Some event parameters are missing, such as `UserBanned`.
- Some events pass excessively redundant parameters, such as `MessageButtonClicked`.

Full changes to the event parameters can be found in the appendix at the end of the document. All usages involving these
events need to be updated accordingly.

All formatting methods in the KMarkdown formatting helper class `Format` have been changed to extension methods, and a
new optional parameter `sanitize` has been added to support whether to escape special characters in the text, which
defaults to true. The calling method of extension methods is still compatible with the original static method calling
method. By default, all formatting methods escape special characters in the text that conflict with KMarkdown syntax to
avoid syntax parsing errors. This feature is enabled by default and can be disabled through the `sanitize` parameter. If
the text parameter passed to this method has already escaped special characters, the `sanitize` parameter should be set
to `false`, or the input parameter should be adjusted to the original unescaped text. Additionally, the logic of
the `Format.Quote` and `Format.BlockQuote` methods have been adjusted. They will now insert line breaks and zero-width
joiners (`\u200d`) as needed within the text to maintain the display effect in the KOOK client. The formatting result of
the `Format.BlockQuote` method guarantees that the entire text block will be displayed as one quoted block in the KOOK
client, while the formatting result of the `Format.Quote` method will split the text into multiple quoted blocks based
on empty lines, with the empty lines not included in the quoted blocks.

The `Parse` and `TryParse` methods in the `CardJsonExtension` class have been renamed to `ParseSingle`
and `TryParseSingle` to avoid conflicts with the `ParseMany` and `TryParseMany` methods used to parse multiple cards.
All calls involving these methods need to be updated accordingly.

The `Features` property type in `IGuild` and `IRecommendInfo` was originally `object[]`, and is now implemented as
the `GuildFeatures` type. All calls involving these properties need to be updated accordingly.

The `RestPresence` namespace has been corrected to `Kook.Rest`. All calls involving `RestPresence` need to update the
namespace reference accordingly.

### Added

- Added methods for managing friends and blocked users. For details, see the appendix at the end of the document.
- Added debug display text for the `Cacheable` and `Quote` classes.
- Added `MaxJoinedGuildDataFetchingRetryTimes` and `JoinedGuildDataFetchingRetryDelay` properties to `KookSocketConfig`
  for controlling the number of times and delay between retries for fetching data when joining a server.
- Added `ParseMany` and `TryParseMany` methods to `CardJsonExtension`.
- (Experimental feature) Added the `IVoiceRegion.MinimumBoostLevel` property.
- (Experimental feature) Added `ValidateCardAsync` and `ValidateCardsAsync` methods to `KookRestClient`.

### Changed

- Changed the parameter types for some events in `BaseSocketClient`. For details, see the appendix at the end of the
  document.
- The formatting methods in the `Format` helper class have been changed to extension methods, and a new optional
  sanitize parameter has been added to support escaping special characters in the text. The default value is `true`.
- Renamed the `Parse` and `TryParse` methods in `CardJsonExtension` to `ParseSingle` and `TryParseSingle`.
- Implemented the `Features` property of `IGuild` and `IRecommendInfo` as the `GuildFeatures` type.
- Corrected the namespace for `RestPresence` to `Kook.Rest`.
- (Experimental feature) The `KookRestClient.GetAdminGuildsAsync` method now supports Bot authentication.

### Fixed

- Fixed an issue where the results of the `Format.Quote` and `Format.BlockQuote` methods were displayed incorrectly in
  KOOK.
- Fixed an issue where the error message in exceptions thrown by `CountdownModuleBuilder.Build` was incorrect.
- Fixed an issue where `BaseSocketClient.DirectMessageUpdated` might pass an incorrect user entity.
- Fixed an issue where the `Author` property was null in message-related events when the user was not cached.
- Fixed an issue where `IGuild.OwnerId` is`0`.
- Fixed an issue where the `IsPinned` property was not set correctly in `BaseSocketClient.Pinned`
  and `BaseSocketClient.Unpinned` events.
- Fixed an issue where the `IPresence.ActiveClient` property could be unintentionally cleared.
- Fixed an issue where the debug display format of `IPresence` was incorrect.
- Fixed an issue where the default implementation of `IRestClient` could throw an exception when `DEBUG_REST`
  preprocessor directive was enabled for debugging high-concurrency requests in the source code.
- Fixed an issue where `Quote.Empty` was not a static property.

### Optimized

- Fixed an issue with missing preprocessor directives in `KookRestApiClient`.
- Optimized the `SocketUser.UpdateIntimacyAsync` method's implementation of the `IUser` interface.
- Improved the use of `NumberBooleanConverter`.
- `KookSocketClient` logs warnings or the content of received packets when they are out of order or not handled
  correctly.
- When printing exception packets, `KookSocketClient` uses the passed-in `serializerOptions` serialization options.
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

- Added method overload `GetUserAsync(string,string,RequestOptions)` to the `IKookClient` interface and implemented it
  in `BaseSocketClient`.
  (6555c2ba0c9993b4523a95a8cbdc544474541984)
- Added support to disable Unicode bidirectional formatting of username strings
  via `KookConfig.FormatUsersInBidirectionalUnicode`.
  (bfc038ec4ce68124aa7eef94b421f6d0f1f941ce)

### Fixed

- Fixed issue with `IUserMessage.Resolve` incorrectly textifying mentions for @everyone and @here.
  (91c1175ad5eb22323cc1fe3a5cc93b9e8fc86ea5)
- Fixed deserialization issue with `IGuild.CreateRoleAsync` results. (1726d5c7c36ead0544784eb937660537c6ec1f4f)
- (Experimental feature) Fixed issue with `KookRestClient.CreateGuildAsync` not returning complete guild information.
  (2b5c59d60e8db309c30cf42509a9756625e98903)

### Misc

- Added XML documentation to all public APIs. (fe1e9bcd0b7bd9a0a99c850234beef1b4993977b,
  2ad3301bcc06a731e9f57bbd542484a31e77c438)
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
- Payloads themselves will be logged as well when deserialization exceptions occur (
  369dd044f5bd4e94bec37c4ba8e4d76e586cd01d)

### Fixed

- Fixed `BaseSocketClient.UserUnbanned` that was not working (624750a0427925fb4b42e26720708b4cc9d60d1c)
- Fix `DownloadVoiceStatesAsync` and `DownloadBoostSubscriptionsAsync` were bypassed unexpectedly
  when `BaseSocketClient.GuildAvailable` is triggered (390b8ab65ad6cab22142c33ee51a62e54b3c1870)

## v0.1.0 [2022-11-15]

Initial release.
