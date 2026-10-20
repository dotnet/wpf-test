# How WinForms Runs UI Tests

This document describes how [dotnet/winforms](https://github.com/dotnet/winforms) runs UI and interaction tests (simulated keyboard and mouse input) in CI, as a reference for WPF. WinForms does **not** use Helix; its UI tests run directly on Azure DevOps build agents.

For MAUI's approach, see [MAUI UI testing](maui-ui-testing.md). For Helix background, see [Helix overview](helix-overview.md) and [Helix queues and UI testing](helix-queues-and-ui-testing.md).

> [!NOTE]
> This information was collected in October 2026 from the public dotnet/winforms `main` branch. Internal pipelines were not checked.

## Summary

| | |
|---|---|
| Test location | [`src/test/integration/UIIntegrationTests`](https://github.com/dotnet/winforms/tree/main/src/test/integration/UIIntegrationTests) |
| Size | About 36 test classes |
| Framework | xUnit, run by the Arcade SDK test targets |
| Input simulation | Win32 `SendInput`, called in-process through a C# input simulator |
| Where it runs | dnceng build agents, image `windows.vs2026preview.scout.amd64(.open)` |
| Helix | Not used. Only Arcade's shared `eng/common` files mention Helix. |

## Test coverage

`UIIntegrationTests` contains about 36 test classes covering controls and scenarios such as:

- Button, ComboBox, DataGridView, Form and other controls
- Drag and drop
- DPI scaling (`Dpi/FormDpiTests.cs`, `Dpi/SplitContainerTests.cs`, `Dpi/ToolStripItemTests.Dpi.cs`)
- Dialogs such as FolderBrowserDialog
- Designer behaviors and anchor layout

## How input is simulated

The tests use the real Windows input pipeline: the mouse cursor actually moves and keystrokes go to the foreground window.

| File | Purpose |
|---|---|
| `Input/InputSimulator.cs` | Wraps Win32 `SendInput` with keyboard and mouse simulators |
| `Infra/SendInput.cs` | Helper that brings a form to the foreground, sends input, then waits for the UI to become idle |
| `Infra/ControlTestBase.cs` | Base class for UI tests; handles setup, idle waiting and cursor positioning |
| `Infra/ScreenshotService.cs` | Captures screenshots for failed tests |
| `Infra/ScreenRecordService.cs` | Records the screen during tests |
| `Infra/DataCollectionService.cs` | Collects diagnostic data (screenshots, recordings) on failure |
| `Infra/DenyExecutionSynchronizationContext.cs` | Guards against code running on the wrong thread during tests |

### Keyboard

`SendInput.SendAsync(form, ...)` accepts strings, characters and virtual key codes:

```csharp
inputSimulator.Keyboard.KeyPress(VIRTUAL_KEY.VK_RETURN);
inputSimulator.Keyboard.TextEntry("some text");
```

Newlines in strings are sent as Enter key presses.

### Mouse

Tests move the cursor to a control, then click or drag. For example, in `DragDropTests.cs`:

```csharp
await MoveMouseToControlAsync(form.ListDragSource);
await InputSimulator.SendAsync(form, inputSimulator => inputSimulator.Mouse
    .LeftButtonDown()
    .MoveMouseTo(targetMousePosition.X - 40, targetMousePosition.Y)
    .MoveMouseTo(targetMousePosition.X, targetMousePosition.Y)
    .MoveMouseTo(targetMousePosition.X + 2, targetMousePosition.Y + 2)
    .MoveMouseTo(targetMousePosition.X + 4, targetMousePosition.Y + 4)
    .LeftButtonUp());
```

Because input is real, tests can be affected by anything else on the desktop (other windows, focus changes or screen locking), which is why they need a dedicated interactive session.

## Other integration tests

`src/test/integration` also contains:

| Project | Purpose |
|---|---|
| `System.Windows.Forms.Maui.IntegrationTests` (with `MauiTests`) | Older WinForms tests reused from an internal test harness also called "MAUI"; unrelated to the .NET MAUI framework |
| `DesignSurface` | Design surface scenarios |
| `TrimTest`, `TrimTestBinaryDeserialization` | Trimming compatibility |
| `WinformsControlsTest` | Test app used by integration tests |
| `System.Windows.Forms.IntegrationTests.Common` | Shared helpers |

`src/Microsoft.VisualBasic/tests/IntegrationTests` contains `Microsoft.VisualBasic.IntegrationTests`, which covers interaction APIs such as `InputBox` and `MsgBox`.

## How the tests run in CI

[`eng/pipelines/build-PR.yml`](https://github.com/dotnet/winforms/blob/main/eng/pipelines/build-PR.yml) runs the tests in the same job that builds the product:

1. **Build:** `eng\cibuild.cmd` builds the product.
2. **Unit tests:** `eng\cibuild.cmd -test`, excluding tests marked `IgnoreForCI` or `failing`. They run with `/m:1` to work around a coverlet issue.
3. **Integration tests:** UI tests are included here.

   ```
   eng\cibuild.cmd -integrationTest /p:TestRunnerAdditionalArguments='--filter-not-trait Category=IgnoreForCI --filter-not-trait Category=failing'
   ```

   They also run with `/m:1`, so test assemblies don't run in parallel and compete for the mouse, keyboard and foreground window.
4. **Results:** published by the Arcade job template (`enablePublishTestResults: true`).

Pools:

| Build | Pool | Image |
|---|---|---|
| Public | `$(DncEngPublicBuildPool)` | `windows.vs2026preview.scout.amd64.open` |
| Internal | `$(DncEngInternalBuildPool)` | `windows.vs2026preview.scout.amd64` |

These build agents run tests in an interactive desktop session, which `SendInput` requires.

## Strengths and trade-offs

**Strengths**

- Simple: no Helix payload, no separate test machine setup, and the same command works locally and in CI.
- Tests run against the freshly built product in the same job.
- In-process input simulation needs no external tools such as Appium or WinAppDriver.
- Failure diagnostics (screenshots and screen recordings) are built into the test infrastructure.

**Trade-offs**

- UI tests hold a build agent for their full duration.
- Tests can't run in parallel on one agent, because they share a single mouse, keyboard and foreground window.
- Coverage is limited to the build agent's OS image; there is no matrix across Windows versions.
- Flaky tests are handled by trait filters (`IgnoreForCI`, `failing`) rather than Helix's automatic retry and quarantine features.

## Lessons for WPF

1. **The build agent model works for real input testing.** WinForms runs mouse and keyboard tests on dnceng build agents without Helix. WPF's `eng/wpfautomatedtests.yml` already runs DRTs and Microsuites on the build agent in a similar way.
2. **Use real input, in-process.** A small `SendInput` wrapper with "wait for idle" helpers, like `InputSimulator.cs` and `SendInput.cs`, avoids external automation dependencies. WPF's existing test infrastructure has its own input helpers that could fill the same role.
3. **Serialize UI tests** (`/m:1` or an equivalent) so they don't compete for input and focus.
4. **Capture diagnostics automatically** with screenshots and screen recordings on failure.
5. **Separate flaky tests with traits** so CI stays green while they are investigated.
6. **Move to Helix if you need an OS matrix or want to free build agents.** In that case, see [MAUI UI testing](maui-ui-testing.md) for how to package and run Windows UI tests on Helix client queues.
