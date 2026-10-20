# How .NET MAUI Runs UI Tests

This document describes how [dotnet/maui](https://github.com/dotnet/maui) runs Windows UI tests in CI, as a reference for WPF. It covers two separate test systems:

1. **Device tests**, which run inside the app and are executed on **Helix** client queues.
2. **Appium UI tests**, which drive the app from outside with simulated mouse and keyboard input and run on **Azure DevOps build agents**, not Helix.

For WinForms' approach, see [WinForms UI testing](winforms-ui-testing.md). For Helix background, see [Helix overview](helix-overview.md) and [Helix queues and UI testing](helix-queues-and-ui-testing.md).

> [!NOTE]
> This information was collected in October 2026 from the public dotnet/maui `main` branch. File names, versions and queue names change over time.

## Summary

| | Device tests | Appium UI tests |
|---|---|---|
| What they test | Platform handlers and controls, from inside a running app | End-to-end user interaction with a sample app |
| How input works | Tests call APIs directly in-process; no simulated input | Appium sends real mouse, keyboard and touch input from a separate test process |
| Test framework | xUnit, hosted by an in-app test runner | NUnit with MAUI's `UITest.*` libraries |
| Where they run (Windows) | Helix queue `windows.11.amd64.client.open` (internal: `windows.11.amd64.client`) | Build agent pool image `1es-windows-2022-open` (internal: `1es-windows-2022`) |
| Pipeline | [`eng/pipelines/ci-device-tests.yml`](https://github.com/dotnet/maui/blob/main/eng/pipelines/ci-device-tests.yml) | [`eng/pipelines/ci-uitests.yml`](https://github.com/dotnet/maui/blob/main/eng/pipelines/ci-uitests.yml) |
| Parallelism | One Helix work item per test project and packaging mode | One build agent job per test category group |
| Scale | 5 test projects × 2 packaging modes (packaged and unpackaged) | About 1,585 test files in `TestCases.Shared.Tests` and about 1,650 Windows reference screenshots |

## Part 1: Device tests on Helix

### What device tests are

Device tests are xUnit test projects that are built as real MAUI apps. When the app starts, an in-app runner (`src/TestUtils/src/DeviceTests.Runners`) runs the tests inside the app process, against real platform controls, and writes an xUnit results file. No external automation tool is involved.

Test projects:

| Scenario | Project |
|---|---|
| Controls | `src/Controls/tests/DeviceTests/Controls.DeviceTests.csproj` |
| Core | `src/Core/tests/DeviceTests/Core.DeviceTests.csproj` |
| Graphics | `src/Graphics/tests/DeviceTests/Graphics.DeviceTests.csproj` |
| Essentials | `src/Essentials/test/DeviceTests/Essentials.DeviceTests.csproj` |
| BlazorWebView | `src/BlazorWebView/tests/DeviceTests/MauiBlazorWebView.DeviceTests.csproj` |

Screenshots can be captured from within tests using `src/TestUtils/src/DeviceTests/CaptureHelper.Windows.cs`.

### Pipeline flow

[`eng/pipelines/arcade/stage-device-tests.yml`](https://github.com/dotnet/maui/blob/main/eng/pipelines/arcade/stage-device-tests.yml) has two Windows stages:

1. **Build (on a build agent).** For each test project, the stage runs `dotnet cake eng/devices/windows.cake --target=buildOnly` twice:
   - `--device=unpackaged` produces a self-contained `.exe`. The publish output is saved before the next build overwrites it.
   - `--device=packaged` produces a signed MSIX package plus its `.cer` certificate. A `GenerateMsixCert` task creates the signing certificate.

   Both outputs are zipped into one archive per test project.
2. **Test (on Helix).** The stage calls Arcade's `send-to-helix.yml` with [`eng/helix_xharness.proj`](https://github.com/dotnet/maui/blob/main/eng/helix_xharness.proj) and `/p:TargetOS=windows`.

### The Helix project

`eng/helix_xharness.proj` uses `Microsoft.DotNet.Helix.Sdk`. For Windows it:

- Targets `windows.11.amd64.client.open` for public builds, or `windows.11.amd64.client` when `HelixInternal=True`.
- Sets `IncludeDotNetCli=true` with the SDK version from the repo, so .NET is installed on the Helix machine.
- Sends the repository's `eng` folder as a `HelixCorrelationPayload`, so the runner script and `Versions.props` are available under `%HELIX_CORRELATION_PAYLOAD%\eng`.
- Creates **two `HelixWorkItem`s per test project**, `<Scenario>-packaged` and `<Scenario>-unpackaged`. Each uses the project's zip as its `PayloadArchive`, has a 2-hour timeout, and runs:

  ```
  call %HELIX_CORRELATION_PAYLOAD%\eng\devices\run-windows-devicetests.cmd <Scenario> <packaged|unpackaged> <PackageId> <TargetFramework>
  ```
- Sets `EnableAzurePipelinesReporter=true` and `FailOnTestFailure=true`, so results appear in the Azure DevOps **Tests** tab and failures fail the build.
- Disables XHarness for Windows (`IncludeXHarnessCli=false`). XHarness is used only for iOS, Mac Catalyst and Android.

### The work item script

[`eng/devices/run-windows-devicetests.cmd`](https://github.com/dotnet/maui/blob/main/eng/devices/run-windows-devicetests.cmd) runs on the Helix machine and:

1. **Installs the Windows App SDK runtime.** It reads the version from `Versions.props` and downloads the installer from `aka.ms/windowsappsdk/<version>/latest/...`.
2. **Packaged mode:**
   - Imports the `.cer` file into `LocalMachine\TrustedPeople`.
   - Removes any previous install of the app, then installs the dependency packages and the app MSIX with `Add-AppxPackage`.
   - Launches the app with `Run-PackagedAppAndWait.ps1`, which uses `IApplicationActivationManager` to get the real app process ID and wait for it to exit. (`Start-Process shell:AppsFolder\...` returns only the launcher's process ID.)
3. **Unpackaged mode:** finds `Microsoft.Maui.<Scenario>.exe` in the payload and runs it with `start /wait`.
4. **Category splitting for Controls tests:** the app is first launched with index `-1` to write a list of test categories to `devicetestcategories.txt`. The script then launches the app once per category, each with an 8-minute timeout and its own results file. This isolates crashes and hangs to a single category.
5. **Collecting results:** it merges all `TestResults-*.xml` files into `testResults.xml` in `%HELIX_WORKITEM_UPLOAD_ROOT%` for Helix to report, and fails the work item if any result file reports failures or errors.
6. **Diagnostics on failure:** it dumps recent Application event log errors, .NET-related events, and the list of processes that have windows.

The script also explains exit code `0xC000027B` (Windows App SDK bootstrap failure), which usually means the Windows App SDK was not included in a self-contained build.

## Part 2: Appium UI tests on build agents

### What the Appium UI tests are

These are end-to-end tests that drive a sample app the way a user would. The test process is separate from the app and communicates with it through Appium.

| Component | Location |
|---|---|
| Sample app with one page per test or issue (about 1,495 issue pages) | `src/Controls/tests/TestCases.HostApp` |
| Shared NUnit tests, written once for all platforms | `src/Controls/tests/TestCases.Shared.Tests` |
| Windows test project and reference screenshots | `src/Controls/tests/TestCases.WinUI.Tests` |
| Test categories (used to split CI jobs) | `src/Controls/tests/TestCases.Shared.Tests/UITestCategories.cs` |
| Cross-platform UI test API | `src/TestUtils/src/UITest.Core` |
| Appium implementation of that API | `src/TestUtils/src/UITest.Appium` |
| NUnit base class | `src/TestUtils/src/UITest.NUnit/UITestBase.cs` |
| Guide | [`docs/UITesting-Guide.md`](https://github.com/dotnet/maui/blob/main/docs/UITesting-Guide.md) |

A typical test:

```csharp
public class Issue18751 : _IssuesUITest
{
    public Issue18751(TestDevice device) : base(device) { }

    public override string Issue => "Can scroll CollectionView inside RefreshView";

    [Test]
    [ShardedTestCategory(UITestCategories.CollectionView, shard: 2)]
    public void Issue18751Test()
    {
        App.WaitForElement("WaitForStubControl");
        VerifyScreenshot(retryTimeout: TimeSpan.FromSeconds(2));
    }
}
```

### How input is simulated

- **Automation stack:** Appium server `2.17.1` with the Appium **Windows driver** `3.1.1` (pinned in `eng/Versions.props`), which uses Windows UI Automation through WinAppDriver. Appium and its drivers are installed by `src/Provisioning/Provisioning.csproj` (target `ProvisionAppium`).
- **Starting the server:** `AppiumServerContext.cs` starts a local Appium server on port 4723 if one is not already running. `AppiumWindowsApp.cs` creates a `WindowsDriver` with `PlatformName=Windows` and `AutomationName=Windows`, pointing at `Controls.TestCases.HostApp.exe` (or the path in `WINDOWS_APP_PATH`).
- **Finding elements:** elements are located by `AutomationId`, for example `App.WaitForElement("...")`.
- **Mouse:** `AppiumMouseActions.cs` implements click, click at coordinates, double-click and long press using W3C pointer actions (`PointerInputDevice(PointerKind.Mouse)`).
- **Other actions:** there are action classes for text entry, scrolling, swiping, sliders, drag and drop, the virtual keyboard, the clipboard, theme changes, and Windows-specific behavior (context menus, alerts, steppers).
- **Screenshot verification:** `VerifyScreenshot(...)` compares the window against a reference image in `TestCases.WinUI.Tests/snapshots`, using ImageMagick through a `VisualRegressionTester`. It supports cropping, a tolerance percentage and retries. The pipeline publishes snapshot diffs as artifacts (`ui-tests-collect-snapshot-diffs.yml`).

### Pipeline flow

[`eng/pipelines/ci-uitests.yml`](https://github.com/dotnet/maui/blob/main/eng/pipelines/ci-uitests.yml) uses [`eng/pipelines/common/ui-tests.yml`](https://github.com/dotnet/maui/blob/main/eng/pipelines/common/ui-tests.yml):

1. **`build_ui_tests_windows` stage:** builds the HostApp once and publishes it as the `ui-tests-samples-windows` artifact.
2. **`winui_ui_tests` stage:** a job matrix with one job per category group in `categoryGroupsToTest`. Large categories such as CollectionView are split into shards. WebView is excluded on Windows because Appium cannot attach to WebView2 ([appium/appium#22217](https://github.com/appium/appium/issues/22217)). Each job, defined in [`ui-tests-steps.yml`](https://github.com/dotnet/maui/blob/main/eng/pipelines/common/ui-tests-steps.yml):
   - Downloads the built app.
   - Sets the screen resolution to 1920×1080 with `eng/scripts/Set-ScreenResolution.ps1`, because hosted agents default to 1024×768.
   - Installs Node.js and Appium.
   - Runs `./build.ps1 -Script eng/devices/windows.cake --target=uitest --project=<TestCases.WinUI.Tests.csproj>` with the category filter. Appium logs go to `appium_windows_<name>.log`.
   - Publishes results with `PublishTestResults@2`, and publishes logs and snapshot diffs as artifacts.

Pools:

| Build | Pool | Image |
|---|---|---|
| Public | dnceng public pool | `1es-windows-2022-open` |
| Internal | `NetCore1ESPool-Internal` | `1es-windows-2022` |

These are build agents with an interactive desktop session, not Helix machines.

## Why MAUI splits the work this way

- **Device tests suit Helix.** Each test project is a self-contained app plus a runner script. It needs no external tools, produces an xUnit results file, and can run in parallel on many client machines while build agents do other work.
- **Appium tests stay on build agents.** They need Node.js, an Appium server, drivers, a fixed screen resolution and a large matrix of category jobs. These are easier to provision on a build agent than to ship in a Helix payload.

## Lessons for WPF

1. **Helix works for Windows UI tests.** MAUI shows that `windows.11.amd64.client(.open)` provides an interactive desktop where apps can be installed, launched and screenshotted.
2. **Make each work item self-contained.** Ship the test app and a single runner script; install runtime prerequisites in the script; and write xUnit XML into `%HELIX_WORKITEM_UPLOAD_ROOT%`. For WPF, QualityVault results would need to be converted to xUnit format, as the original Helix plan intended.
3. **Isolate crashes and hangs.** Run one category per process with a per-category timeout, so one bad test does not lose the whole work item's results.
4. **Collect diagnostics automatically.** Dump the event log and running windows when a run fails, because there is no way to connect to a Helix machine interactively.
5. **Set the screen resolution** before running visual or input tests.
6. **Shard large suites by category,** either as separate Helix work items or as separate build agent jobs.
7. **Choose external input simulation deliberately.** MAUI uses Appium with the Windows driver on build agents. WPF could instead use UI Automation or `SendInput` directly from its existing test infrastructure, as WinForms does, which avoids the Node.js and Appium dependencies.

## Key files

| File | Purpose |
|---|---|
| [`eng/helix_xharness.proj`](https://github.com/dotnet/maui/blob/main/eng/helix_xharness.proj) | Helix project for device tests: queues, payloads and work items |
| [`eng/devices/run-windows-devicetests.cmd`](https://github.com/dotnet/maui/blob/main/eng/devices/run-windows-devicetests.cmd) | Helix work item runner for Windows device tests |
| `eng/devices/Run-PackagedAppAndWait.ps1` | Launches a packaged app and waits for the real process to exit |
| [`eng/devices/windows.cake`](https://github.com/dotnet/maui/blob/main/eng/devices/windows.cake) | Build and test tasks for Windows (`buildOnly`, `testOnly`, `uitest`) |
| [`eng/pipelines/arcade/stage-device-tests.yml`](https://github.com/dotnet/maui/blob/main/eng/pipelines/arcade/stage-device-tests.yml) | Device test build and Helix stages |
| [`eng/pipelines/common/ui-tests.yml`](https://github.com/dotnet/maui/blob/main/eng/pipelines/common/ui-tests.yml) | Appium UI test stages and category matrix |
| [`eng/pipelines/common/ui-tests-steps.yml`](https://github.com/dotnet/maui/blob/main/eng/pipelines/common/ui-tests-steps.yml) | Steps for each Appium UI test job |
| `src/TestUtils/src/UITest.Appium/AppiumWindowsApp.cs` | Appium Windows driver setup |
| `src/Controls/tests/TestCases.Shared.Tests/UITest.cs` | Test base class, app path resolution and `VerifyScreenshot` |
| `src/Provisioning/Provisioning.csproj` | Installs Appium and its drivers |
