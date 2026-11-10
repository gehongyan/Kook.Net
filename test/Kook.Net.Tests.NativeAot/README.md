# NativeAOT: core protocol validation

This first stage covers generated JSON metadata in `Kook.Net.Core`, `Kook.Net.Rest`,
`Kook.Net.WebSocket`, and `Kook.Net.Webhook` on .NET 8 and later. The existing
.NET Framework 4.6.2 and .NET Standard targets remain supported for managed builds.
It is not a claim that every API or every package in this repository is NativeAOT compatible.

## Reproduce the offline checks

Use the repository SDK from `global.json` and the native compiler prerequisites for
your platform (Visual Studio C++ build tools on Windows; clang and zlib development
packages on Linux). No KOOK credentials or live network connection are used by the checks.
NuGet restore requires access to the configured feeds.

```sh
# Managed execution with JSON reflection explicitly disabled:
dotnet run --project test/Kook.Net.Tests.NativeAot -f net8.0
# Publish and run an actual native executable; use win-x64 on Windows:
dotnet publish test/Kook.Net.Tests.NativeAot -f net8.0 -c Release -r linux-x64 -p:NativeAotSmoke=true -p:TrimmerSingleWarn=false -o artifacts/nativeaot
./artifacts/nativeaot/Kook.Net.Tests.NativeAot
# Repeat with -f net10.0 to verify the current main target.
# Existing regression tests:
dotnet run --project test/Kook.Net.Tests.Unit
# All retained library targets:
dotnet build src/Kook.Net.Webhook -c Release
```

`NativeAotSmoke` enables `PublishAot` only on the smoke executable. Passing
`-p:PublishAot=true` globally also affects referenced legacy target frameworks
while restoring this multi-target repository and can produce NETSDK1207.
The dedicated workflow publishes and executes the app on Windows and Linux for
.NET 8 and .NET 10. This executable deliberately does not import the regular
xUnit test props or reference the umbrella package and Commands framework.

The smoke checks cover:

- Public card parsing/writing and nested card/module/element converters, Unicode
  and relaxed encoding, arrays/enumerables, and unknown-card rejection.
- Known and unknown embeds, poke resources, typed application-owned embed/poke
  resolution, closed generic pagination, empty-array/object nullable channels,
  and representative gateway event metadata.
- Real REST request/response processing through `DefaultRestClient`, an in-memory
  `HttpMessageHandler`, rate-limit buckets, optional request fields, API errors,
  and public text/card template APIs with application-generated metadata.
- WebSocket text and zlib-compressed binary input, frame dispatch, login/connect
  state transitions with a fake transport, and outbound ping serialization.
- Webhook AES decryption, text and compressed challenge replies, verify-token
  rejection, and forwarding a normal event.

The regular unit tests also discover concrete REST/gateway protocol model types
and require generated metadata, preventing new models from silently relying on
reflection. That discovery runs on the managed test runner, not in the native app.

## Application-owned JSON types

Template APIs retain their existing `JsonSerializerOptions` parameters. Supply a
source-generated resolver for custom parameter types in NativeAOT applications:

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class TemplateParameters
{
    public string? Title { get; set; }
}

[JsonSerializable(typeof(TemplateParameters))]
internal partial class BotJsonContext : JsonSerializerContext;

// In an async method with an IMessageChannel:
// await channel.SendTextAsync(templateId, new TemplateParameters { Title = "Hello" },
//     jsonSerializerOptions: new JsonSerializerOptions { TypeInfoResolver = BotJsonContext.Default });
```

The same options path applies to direct messages and template edits. Register all
runtime types needed by custom converters or polymorphism. Options are copied
before adding library metadata, so read-only caller options remain valid. Normal
JIT applications retain their reflection default and serializer configuration.
When reflection is disabled and no resolver was supplied, custom-template APIs
throw an actionable `NotSupportedException`.

`NotImplementedEmbed.Resolve<T>` and `NotImplementedPokeResource.Resolve<T>` now
accept `JsonTypeInfo<T>` (and validated untyped `JsonTypeInfo`) from an application
context. Their original options-based overloads remain available, with trimming
and dynamic-code annotations; prefer typed metadata in native applications.
The resolving-function overloads remain available as well.

## Boundaries

Commands source generators and the reflection-based Commands framework are outside
this stage. ASP.NET MVC/Webhook.AspNet integration, DependencyInjection/Hosting,
Experimental/WebRTC/audio, Pipe, CardMarkup and message-queue packages are not
certified by these checks. A native bot should reference the required core packages
directly rather than infer compatibility from the umbrella package.

The checks use deterministic fake transports. They do not verify a live KOOK bot,
full gateway reconnect/resume and cache lifecycle, all endpoint combinations,
production Webhook hosting, native audio dependencies, macOS, ARM64, or .NET 9
native publishing. A successful native smoke run establishes only the paths listed
above. Keep AOT and trim diagnostics visible when publishing an application.

There are two narrowly scoped analyzer suppressions in the JIT-only custom-type
resolver fallback. They are guarded by `JsonSerializer.IsReflectionEnabledByDefault`;
the reflection-disabled smoke checks verify the error branch. No core converter
warnings are globally suppressed. Existing xUnit1051 cancellation suggestions in
other unit tests are unrelated to this change.

## Provenance

Adapted selectively from [PR #27](https://github.com/gehongyan/Kook.Net/pull/27),
latest reviewed head `3571a701042043891645a5c9f7449e832493b71d`, by Gehongyan and
Copilot/copilot-swe-agent, onto current master
`359ad738c2d883f629073d9bfec168fae8934f5f`. The old branch was not merged and
PR #27 remains untouched. Source-generation contexts, typed converter calls and
public resolve overloads retain that authorship; configuration fixes, complete
current protocol roots, regression checks and native smoke coverage extend it.
The repository MIT license and third-party notices remain in force.
