# Changelog

---

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