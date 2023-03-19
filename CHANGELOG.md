# Changelog

---

## v0.3.0 [2023-03-19]

### Update Path

This release changed the default value of `everyoneHandling` parameter in `IUserMessage.Resolve` to `TagHandling.Name`.
All usages of this method need to take note of this change.

### 新增

- Added method overload `GetUserAsync(string,string,RequestOptions)` to the `IKookClient` interface and implemented it in `BaseSocketClient`.
  (6555c2ba0c9993b4523a95a8cbdc544474541984)
- Added support to disable Unicode bidirectional formatting of username strings via `KookConfig.FormatUsersInBidirectionalUnicode`.
  (bfc038ec4ce68124aa7eef94b421f6d0f1f941ce)

### 修复

- Fixed issue with `IUserMessage.Resolve` incorrectly textifying mentions for @everyone and @here.
  (91c1175ad5eb22323cc1fe3a5cc93b9e8fc86ea5)
- Fixed deserialization issue with `IGuild.CreateRoleAsync` results. (1726d5c7c36ead0544784eb937660537c6ec1f4f)
- (Experimental feature) Fixed issue with `KookRestClient.CreateGuildAsync` not returning complete guild information.
  (2b5c59d60e8db309c30cf42509a9756625e98903)

### 其它

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
