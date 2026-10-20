# Helix Queues and UI Testing Across .NET Repositories

This document summarizes which machines Helix offers, where to find queue information, and how other repositories in the dotnet organization run UI and interaction tests (keyboard and mouse input). It is intended to help decide how WPF UI tests should run in CI. For Helix concepts, capabilities and constraints, see [Helix overview](helix-overview.md).

> [!NOTE]
> This information was collected in October 2026 from the live Helix queue API and a code search of public dotnet repositories. Internal Azure DevOps pipelines were not checked. Queue names and removal dates change over time; always check the queue API before choosing a queue.

## Machines offered by Helix

Helix machines are grouped into **queues**. At the time of writing, the Helix queue API listed **251 queues: 109 Windows, 117 Linux and 25 macOS**.

- Queues ending in `.open` accept anonymous jobs from public builds, and their results are public.
- Queues without `.open` are internal-only and require authentication.
- `.perf` queues are dedicated benchmarking machines.
- `.rt` and `.svc` are additional variants of a queue; their exact purpose is not documented in the sources checked.

### Windows queue families

| Family | Example queues | Estimated removal |
|---|---|---|
| Windows 10 client (logged-on desktop, suitable for UI tests) | `windows.10.amd64.clientrs5.open` | 2028-11 |
| Windows 11 client (logged-on desktop, suitable for UI tests) | `windows.11.amd64.client.open` | 2028-01 |
| Windows 10 general | `windows.10.amd64.open`, `windows.10.amd64.x86` | 2027-01 |
| Windows Server 2019 (RS5), 2022, 2025 | `windows.10.amd64.serverrs5.open`, `windows.amd64.server2022.open`, `windows.server2025.amd64.open` | 2028 to 2034 |
| Machines with Visual Studio installed | `windows.amd64.vs2022.open`, `windows.amd64.vs2026.open`, `windows.server.amd64.vs2019.open` | 2029 to 2032 |
| ARM64 | `windows.11.arm64.open`, `windows.2025.arm64.cedarcrest.open` | 2027 to 2028 |
| Performance labs | `windows.11.amd64.viper.perf.open` and other `.perf` queues | Varies |
| Special purpose | `windows.11.amd64.cet.open` (Control-flow Enforcement Technology), `windows.11.amd64.android.open` (Android emulator), `windows.11.amd64.experimental.open` | Varies |

Notes:

- `windows.10.amd64.client.open` was still listed but was past its estimated removal date (2025-05).
- The Windows 7 queue listed in [Testing in Helix](testing-in-helix.md) no longer exists. The `Windows.10.Amd64.ClientRS5` queue that WPF's pipelines still reference is still available.

## Where to find queue information

1. **Helix queue API (source of truth):** <https://helix.dot.net/api/2019-06-17/info/queues>. Each entry includes the queue ID, OS, architecture, whether it is internal-only, estimated removal date, current queue depth, and the software installed on the machine image ("Artifacts").
2. **Helix API browser (Swagger):** <https://helix.dot.net/swagger/ui/index.html>.
3. **Other repositories' queue lists:** for example [aspnetcore `docs/Helix.md`](https://github.com/dotnet/aspnetcore/blob/main/docs/Helix.md) and [`eng/targets/Helix.Common.props`](https://github.com/dotnet/aspnetcore/blob/main/eng/targets/Helix.Common.props), or [MAUI `eng/helix_xharness.proj`](https://github.com/dotnet/maui/blob/main/eng/helix_xharness.proj).
4. **Azure DevOps (internal):** machine images and queue definitions are believed to be maintained by dnceng in the `dotnet-helix-machines` repository in the dnceng internal project. This was not verified.

To list the Windows queues that are currently available:

```powershell
$q = Invoke-RestMethod "https://helix.dot.net/api/2019-06-17/info/queues"
$q | Where-Object { $_.IsAvailable -and $_.OperatingSystemGroup -eq 'windows' } |
    Sort-Object QueueId |
    Select-Object QueueId, Architecture, IsInternalOnly, EstimatedRemovalDate
```

## How dotnet repositories use Helix for UI testing

| Repository | Uses Helix | UI or interaction testing on Helix |
|---|---|---|
| [dotnet/maui](https://github.com/dotnet/maui) | Yes, through `eng/helix_xharness.proj` | **Yes.** Windows device tests (Controls, Core, Graphics, Essentials) install the test app as an MSIX package (packaged and unpackaged runs) and run on `windows.11.amd64.client.open` (internal queue: `windows.11.amd64.client`) using `eng/devices/run-windows-devicetests.cmd`. iOS and Mac Catalyst run on `osx.15.arm64.maui.open` and `osx.26.arm64.open`, and Android on `ubuntu.2204.amd64.android.33.open`, through XHarness. |
| [dotnet/android](https://github.com/dotnet/android) | Yes | MAUI app tests (`run-maui-r2r-helix-matrix.yaml`) on `Windows.11.Amd64.Android.Open`, which runs Android emulators. |
| [dotnet/runtime](https://github.com/dotnet/runtime) | Yes | Some test runs use `Windows.10.Amd64.Client.Open` and the Android queues. These are mostly not desktop UI tests. |
| [dotnet/aspnetcore](https://github.com/dotnet/aspnetcore) | Yes, extensively | Its Windows queues include `Windows.11.Amd64.Client.Open` and `ClientPre`, but they run regular test projects. No browser or Playwright UI tests on Helix were found. |
| [dotnet/efcore](https://github.com/dotnet/efcore) | Yes (`eng/helix.proj`) | Uses client queues for ordinary tests, not UI tests. |
| [dotnet/performance](https://github.com/dotnet/performance) | Yes | Benchmarks only, on `ClientRS5` and `.perf` queues. |
| [dotnet/winforms](https://github.com/dotnet/winforms) | **No** | UI tests run on the build agent (see [WinForms UI testing](winforms-ui-testing.md)). |
| [dotnet/wpf](https://github.com/dotnet/wpf) | **No** (previously yes) | Tests run on the build agent. Helix variables remain in the pipelines but are unused. |

MAUI is the closest example of running Windows UI tests on Helix. It shows that `windows.11.amd64.client(.open)` can host real desktop UI tests, including app installation and screenshot capture (`src/TestUtils/src/DeviceTests/CaptureHelper.Windows.cs`).

## WinForms and MAUI UI testing in detail

- **WinForms** runs about 36 UI test classes that use real mouse and keyboard input (Win32 `SendInput`) on dnceng build agents, not Helix. See [WinForms UI testing](winforms-ui-testing.md).
- **MAUI** runs in-app device tests on Helix client queues and Appium-driven UI tests with simulated input on build agents. See [MAUI UI testing](maui-ui-testing.md).

## Options for WPF

1. **Build agent (WinForms model).** Run UI tests directly on the build agent, as WinForms does and as `eng/wpfautomatedtests.yml` in dotnet/wpf already does for DRTs and Microsuites. This is the simplest option but ties up the build agent for the duration of the test run. See [WinForms UI testing](winforms-ui-testing.md).
2. **Helix client queues (MAUI model).** Package the tests as a Helix payload and run them on `windows.11.amd64.client(.open)` or `windows.10.amd64.clientrs5(.open)`. This allows parallel runs across OS versions and frees build agents, but requires producing xUnit, JUnit or TRX results, cleaning up machine state after each work item, and keeping queue names current. The relevant MAUI files to study are [`eng/helix_xharness.proj`](https://github.com/dotnet/maui/blob/main/eng/helix_xharness.proj) and [`eng/devices/run-windows-devicetests.cmd`](https://github.com/dotnet/maui/blob/main/eng/devices/run-windows-devicetests.cmd). See [MAUI UI testing](maui-ui-testing.md).
