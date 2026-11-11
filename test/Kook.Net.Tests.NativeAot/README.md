# NativeAOT protocol tests

This xUnit v3 suite covers representative card and JSON protocol paths in
Kook.Net.Core, Kook.Net.Rest, Kook.Net.WebSocket, and Kook.Net.Webhook. The
tests exercise nested card elements, known and unknown embeds, custom metadata,
REST request fields and pagination, gateway payloads, and webhook challenge JSON.

The project is included in Kook.Net.slnx. It uses xunit.v3.aot with source-generated
test discovery, and targets .NET 9, .NET 10, and .NET 11. Regular unit tests and
NativeAOT publishing run in separate CI jobs.

## Run locally

Use the SDK specified in global.json. Native publishing requires the Visual
Studio C++ build tools on Windows, or clang and the zlib development package on
Linux.

```powershell
# Run the managed tests with JSON reflection disabled.
dotnet test --project test/Kook.Net.Tests.NativeAot --framework net11.0

# Publish and run the native test executable on Windows x64.
dotnet publish test/Kook.Net.Tests.NativeAot -f net11.0 -c Release -r win-x64 -p:NativeAotXunit=true -p:TrimmerSingleWarn=false -o artifacts/xunit-native
./artifacts/xunit-native/Kook.Net.Tests.NativeAot.exe
```

For Linux x64, publish with `-r linux-x64` and run the executable without the
`.exe` suffix. CI publishes and runs the suite for all three frameworks on
Windows and Linux.

`NativeAotXunit=true` enables NativeAOT for this test project and activates the
assertions that dynamic code is unsupported. All modes test with JSON
reflection disabled. The project explicitly enables AppHost so restore fetches
the platform pack required by xUnit during the build.

The tests use protocol data locally. Live KOOK connectivity, full transport
behavior, and optional packages are outside this suite's coverage.
