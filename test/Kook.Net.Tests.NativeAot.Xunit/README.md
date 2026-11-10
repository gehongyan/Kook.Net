# xUnit NativeAOT tests

This is an additional test project. `../Kook.Net.Tests.NativeAot/Program.cs` and
its .NET 8/10 smoke coverage remain unchanged.

The repository uses `xunit.v3` **4.0.0**, MTP v2 and a .NET 10 SDK. According to
[xUnit's NativeAOT guidance](https://xunit.net/docs/getting-started/v3/native-aot),
NativeAOT requires the separate **`xunit.v3.aot`** package and **.NET 9 or later**.
This project uses the same 4.0.0 release and targets .NET 9 and .NET 10. Its restored
AOT runner graph selects `xunit.v3.core.aot.mtp-v2` and
`xunit.v3.runner.inproc.console.aot` 4.0.0, plus MTP/Telemetry/TrxReport.Abstractions/
Platform.MSBuild 2.3.3. Test discovery and inline theory rows use xUnit source
generation, rather than assembly scanning or reflection roots.

The project deliberately does not import `props/tests.props`: that file pulls in
the reflection-mode xUnit package, NSubstitute, HangDump, the umbrella package and
Experimental. Those dependencies are unnecessary for these protocol assertions;
the new native executable references only the core Webhook dependency chain.
The regular unit tests and their existing runner/dependencies are unchanged.
This standalone project is intentionally outside the main solution: the existing
solution-wide test command passes HangDump/reporting extension flags that this
isolated runner does not import. Its dedicated CI job runs the published executable
directly, and does not require a managed .NET 9 runtime on the .NET 10 CI image.

These tests adapt representative existing card/JSON checks with explicit
assertions for nested cards, known/unknown embeds, custom embed/poke metadata,
read-only template options, metadata-required errors, REST request fields and
pagination, gateway nullable channels/frame payloads, and Webhook challenge JSON.
Reflection-based `Assert.Equivalent` and the managed metadata-inventory test are
not copied into this suite. The original smoke executable retains transport,
compression, AES and public template-send checks.

## Commands

Run from the repository root with the SDK from `global.json`. Native compiler
prerequisites are the same as for the existing smoke project.

```powershell
# Managed source-generated xUnit/MTP runner (not evidence of native execution):
dotnet run --project test/Kook.Net.Tests.NativeAot.Xunit -f net10.0
# Actual native publication and execution on Windows x64:
dotnet publish test/Kook.Net.Tests.NativeAot.Xunit -f net10.0 -c Release -r win-x64 -p:NativeAotXunit=true -p:TrimmerSingleWarn=false -o artifacts/xunit-native
./artifacts/xunit-native/Kook.Net.Tests.NativeAot.Xunit.exe
# Repeat with -f net9.0 for the minimum supported xUnit AOT target.
```

On Linux use `-r linux-x64` and execute the binary without `.exe`. The CI workflow
adds a separate publish-and-execute xUnit matrix for Windows/Linux and .NET 9/10,
leaving the existing .NET 8/10 smoke job in place. Nonzero native runner exit codes
fail CI. No new trim/AOT warning suppressions are introduced.

`NativeAotXunit=true` enables `PublishAot` only inside this test project, avoiding
propagating a global PublishAot flag into referenced legacy target frameworks.
It also defines `EXPECT_NATIVE_AOT`; an executed xUnit fact then requires both
`RuntimeFeature.IsDynamicCodeSupported` and `IsDynamicCodeCompiled` to be false.
All modes require JSON reflection to be disabled. Managed execution is separately
identified and is never reported as a native pass.

This is representative protocol coverage, not native certification of the entire
488-test managed suite. .NET 8 is below xUnit's supported AOT target floor and is
still validated by the original smoke executable. Live KOOK connectivity, complete
gateway/cache lifecycle, optional packages, macOS and ARM64 remain outside scope.

## Local validation (Windows x64)

Validated on 2026-11-10 with .NET SDK 10.0.403 and the Windows native C++ toolchain.

| Execution | Target | Passed | Failed / skipped | Exit code |
| --- | --- | ---: | --- | ---: |
| Managed xUnit/MTP (`dotnet run`) | net10.0 | 15 | 0 / 0 | 0 |
| Published NativeAOT xUnit executable | net10.0, win-x64 | 15 | 0 / 0 | 0 |
| Published NativeAOT xUnit executable | net9.0, win-x64 | 15 | 0 / 0 | 0 |
| Existing managed unit regression | net10.0 | 488 | 0 / 0 | 0 |
| Original smoke (`Program.cs`, managed/reflection disabled) | net8.0 | All smoke checks | None | 0 |

Both xUnit native publish logs contain zero compiler, trim or AOT diagnostics.
The existing unit suite retains its 17 pre-existing xUnit1051 cancellation
suggestions. No warnings are suppressed by the new test project. Both native
runs execute `RuntimeUsesExpectedExecutionMode`, including its no-dynamic-code
assertions. The .NET 8 smoke row above is explicitly a managed regression run;
its earlier native validation remains separate.

Original `test/Kook.Net.Tests.NativeAot/Program.cs` SHA-256 before and after:
`0BE6883FB6BBDFD2D9B3A7D5EA26878D02F0B1FC3B2977C63021B3DE585CFFBA`.

Linux CI is added but has not been executed locally. macOS, ARM64, live KOOK,
transport/AES paths in this new xUnit project, and the remainder of the managed
suite under NativeAOT have not been validated by these runs.
