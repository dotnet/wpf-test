# Running DRTs on CI

## Status

Draft. The direction is decided: build agents, starting in wpf-test. Investigation is in progress (see the [Tracker](#tracker)). The implementation plan is in [Implementing DRTs on CI](drts-on-ci-implementation.md).

## Goal

Run the WPF DRTs (Developer Regression Tests) automatically in CI for this repository, using the Helix queues run by the .NET engineering team. See [Helix overview](helix-overview.md) for what Helix is and how it works, and [Testing in Helix](testing-in-helix.md) for the earlier WPF setup.

## Direction

DRTs will first run on **Azure DevOps build agents** (approach D below). Helix remains a later option. `eng/wpfautomatedtests.yml` in dotnet/wpf is the reference model.

## Required capabilities

| # | Capability | How it could work on build agents | Open questions |
|---|---|---|---|
| C1 | **Start a test run on demand**, for example with a PR comment | Use a separate DRT pipeline that does not run automatically on every PR. Start it with an Azure Pipelines comment command (`/azp run <pipeline-name>`), from the ADO **Run pipeline** button, or on a schedule. | Which pipeline setting makes a pipeline run only when commented on? Who is allowed to trigger it? |
| C2 | **Start a test run with filters** | Add runtime `parameters:` to the pipeline (area, sub-area, name, keywords, priority) and pass them to QV's existing filters: `RunDrts.cmd /Area=… /Name=… /Keywords=…`. `RunDrts.cmd` forwards `%*` to QV. | `/azp run` cannot pass parameters, so comment-started runs use the defaults (all DRTs). Filtering from a comment would need a bot or GitHub Action that calls the ADO REST API. Is that needed? |
| C3 | **Retry failed tests automatically** | Three options: (a) QV's `/RerunFailures` switch, which already exists in `RunCommand` and `ExecutionEngine` and reruns failing tests within the same run; (b) `retryCountOnTaskFailure` on the run step, which reruns the whole area; (c) manual reruns (C4). Prefer (a). | How does `/RerunFailures` decide what to rerun, and how does a pass on rerun appear in `testResults.xml`? Should the Tests tab show it as passed or flaky? |
| C4 | **Rerun only failed areas**, using ADO's **Rerun failed jobs** | Build once in a build stage and publish the DRT payload as an artifact. A test stage then runs **one job per area** (a matrix of 21). Each job downloads the payload, runs `RunDrts.cmd /Area=<area>`, and publishes its results. A job fails when its tests fail, so **Rerun failed jobs** reruns only those areas and reuses the same artifacts. To rerun one test, start a manual run filtered with `/Name=` (C2). | Agent cost: 21 jobs need 21 agent slots and repeat the payload and SDK setup, so consider `maxParallel`. Is one job per area the right size, or should small areas be grouped? |

Consequences for the pipeline design:

- A failing DRT must fail its job. Don't use `continueOnError: true` as `wpfautomatedtests.yml` does. Use `RunDrts.cmd`'s exit code, or set `failTaskOnFailedTests: true` on `PublishTestResults@2`.
- Each job publishes its own `testResults.xml`, with the area in the test run title (for example `DRT <area> x64 Release`), so reruns show up separately in the Tests tab.
- Each job copies QV's `Report` folder as an artifact, even when tests fail (`condition: always()`).

### Can QualityVault, unchanged, support these capabilities?

| # | Supported unchanged? | Details |
|---|---|---|
| C1 | ✅ Yes | How a run starts is decided by the pipeline, not QV. |
| C2 | ✅ Yes | `RunCommand` inherits `FilterableCommand`, which supports `/Area`, `/SubArea`, `/Name`, `/Priority`, `/Keywords` and `/Versions`. `/Area` is confirmed to work (I7). For DRTs, a test's name is `<exe name>(<args>)`, which matters when filtering with `/Name`. |
| C3 | 🟡 Partly | `/RerunFailures` reruns a failing execution group **once**. With `FullIsolation`, that is one DRT. The behavior is fixed in code: one retry only; no retry if a crash dump was produced; and only the second attempt's result is kept (`ClearResult`). As a result, a test that fails and then passes is reported as an ordinary pass: nothing is marked flaky and the first failure is lost. A code comment says rerun has only been tested with the `CodeCoverage` and `ListenToTests` cleanup commands. |
| C4 | ✅ Yes, with care | Rerunning failed areas works through one ADO job per area and needs nothing from QV. Rerunning a single test is a manual run filtered with `/Name`. The job must fail reliably: `RunDrts.cmd` exits with 1 when `DrtReport.xml` has a pass rate below 100%. If QV crashes and writes no `DrtReport.xml`, the exit code is whatever the last command returned, so the pipeline should also fail when the report is missing. |

The ADO results integration works unchanged too: QV writes xUnit v2 XML to `Report\testResults.xml`. Known limitations:

- The file has one `<assembly>` per area, named after the **first** test's executable in that area.
- Each result shows `type="Microsoft.Test.Drt.DrtRunner"` and `method="Main"`, so every DRT appears under the same class in ADO. This was seen in M0.
- A failure's message holds only the repro arguments; the DRT's own output is in the QV `Report` folder, not in the Tests tab (M0).

Small QV changes worth considering later, though none is needed to start:

- A configurable number of retries.
- Recording "passed on rerun" in the report.
- Better names for the `<assembly>` and test entries.
- An explicit non-zero exit code from `QV.cmd` when tests fail.

## Current state

- **DRT inventory.** [`src/Test/BranchCommon/data/DrtList.xml`](../src/Test/BranchCommon/data/DrtList.xml) lists 85 DRTs across 21 areas. The largest are AppModel (30), DigitalDocuments (13) and ElementServices (9); the rest have 1 to 4 each.
- **How DRTs run locally.** `RunDrts.cmd` calls QualityVault (QV) with [`DiscoveryInfoDrts.xml`](../src/Test/BranchCommon/data/DiscoveryInfoDrts.xml). That manifest uses the `DrtAdaptor`, runs each DRT through `sti.exe` and QV's `DrtRunner`, marks DRTs as needing elevation (`UacElevation="Elevated"`), and gives each a 240-second timeout. Results go to `%APPDATA%\QualityVault\Run\Report`.
- **Payload size.** A Release x64 build publishes about 1.8 GB of test files. The DRTs and the infrastructure they need (the `DRT`, `Infra` and `Common` folders) are about 95 MB.
- **Result format.** QV can already write xUnit-format results (`XUnitReportGenerator`), which Helix and Azure DevOps understand.
- **Exit codes.** `RunDrts.cmd` exits with 1 when any test fails. `CIRunDrts.cmd` always exits with 0.
- **Helix wiring is stale and unused.**
  - [`eng/helixpublish.proj`](../eng/helixpublish.proj) targets `net6.0` and imports `DrtsToRun.props` from an old WPF Arcade SDK. It creates one work item per DRT area that runs `runtests.cmd /Area=<area> -ci` with a 20-minute timeout.
  - No pipeline submits it. [`eng/pipeline.yml`](../eng/pipeline.yml) turns off test-result publishing with the comment "tests run in helix", but has no `send-to-helix` step.
  - The pipeline only compresses and publishes the test files for non-PR builds.
- **Available queues.** DRTs need a logged-on desktop session, so they need Helix "client" queues. Current ones include `windows.11.amd64.client.open` (public) and `windows.11.amd64.client` (internal). Windows 10 client and RS5 queues also exist. Check the [queue list](https://helix.dot.net/api/2019-06-17/info/queues) before choosing, because queues are retired over time.

## Approaches

### A. Run QualityVault on Helix as it is

Send the `DRT`, `Infra` and `Common` folders as a shared payload, and have each work item run `RunDrts.cmd`. QV writes xUnit results into the work item folder, and Helix reports them to Azure DevOps.

The work can be split three ways:

| Split | Work items | Trade-off |
|---|---|---|
| One work item | 1 | Simplest. DRTs run one after another, and one hang blocks the rest. |
| One per area | 21 | Good parallelism. QV's setup cost is paid 21 times. Matches the old `helixpublish.proj`. |
| One per DRT | 85 | Most isolation. QV's setup cost is paid 85 times. |

- **Pros:** very little new code. Keeps QV's process isolation, crash dumps, timeouts and machine-state handling.
- **Cons:** keeps QV's legacy behavior. Retries happen only inside QV (`RerunFailures`), not through Helix. Failure reports are coarse.

### B. Run the DRT executables directly, without QualityVault

Generate one Helix work item per DRT from `DrtList.xml`, using each DRT's executable and arguments. Pass or fail comes from the process exit code.

- **Pros:** no QV, the most parallelism, and failures are easy to read.
- **Cons:** loses what QV's `DrtRunner` provides: timeouts, logging, support-file layout and elevation. Some DRTs may depend on it, which must be checked for each DRT.

### C. Wrap the DRTs in a Microsoft.Testing.Platform or xUnit project

Each DRT becomes a test that launches its process. Helix's built-in `MTPProject` or `XUnitProject` support then creates the work items.

- **Pros:** native Helix and Azure DevOps integration: TRX results, per-test retry, hang and crash dumps, and quarantine through `eng/test-configuration.json`. This is the direction .NET testing is moving.
- **Cons:** the most upfront work. The desktop-session and elevation requirements still apply.

### D. Run on the build agent instead of Helix (fallback)

Run `RunDrts.cmd` directly in the pipeline job, as dotnet/wpf does today in `eng/wpfautomatedtests.yml`.

- **Pros:** simple, and no Helix dependency.
- **Cons:** DRTs run one after another, and the build agent may not have a proper interactive desktop.

## Recommendation

Start with **approach D, on build agents**, for DRTs only (see [Direction](#direction)). Approach A, one Helix work item per area, remains the next step if build-agent time or the lack of an OS matrix becomes a problem.

## Where the pipeline lives

DRTs exist to catch **product** regressions, and the product is built in dotnet/wpf. Each repository that could host the pipeline catches something different:

| | wpf-test (public) | dotnet/wpf (public) | dotnet/wpf internal mirror |
|---|---|---|---|
| WPF under test | The SDK pinned in `global.json`, which is already-shipped WPF | WPF built in the same PR | WPF from official or servicing builds |
| What it catches | Broken tests, and product regressions only after they flow into the SDK | Product regressions before merge | Regressions in official and servicing builds |
| How a run starts | `/azp run` on wpf-test PRs, a schedule, or manually | `/azp run` on wpf PRs, opt-in | A schedule or after each official build. GitHub comments are not available. |
| Where the test binaries come from | Built in the same pipeline | Built from wpf-test, or downloaded from a wpf-test pipeline | Internal artifacts, as `wpfautomatedtests.yml` does with definition 479 |
| Effort | Lowest | Medium: two repositories, and the freshly built WPF must be patched into the test SDK (`copy-wpf.ps1 -testhost`) | Medium to high, and not visible to external contributors |

**Decision: build it in phases.** The pipeline logic lives in a reusable YAML template in wpf-test.

1. **wpf-test.** Prove that DRTs run reliably on build agents and implement C1–C4. This also validates test changes.
2. **dotnet/wpf public.** A PR job reuses the wpf-test template, builds the product and injects it into the test SDK. This is where DRTs are most useful.
3. **dotnet/wpf internal mirror.** Validation of official and servicing builds.

See [Implementing DRTs on CI](drts-on-ci-implementation.md) for the plan.

## Decisions needed for any Helix approach

1. **Which WPF build is tested.** The work item must deploy the .NET and WPF runtime under test, for example through `IncludeDotNetCli` or a payload of private WPF binaries. Otherwise the DRTs only test whatever runtime is already on the machine.
2. **Elevation.** ~~Confirm that client queues run work items as administrator.~~ **Answered:** they do, elevated and in an interactive console session. See [I1](#i1-elevation-on-helix-client-queues).
3. **Queues and access.** Choose the queues: for example `windows.11.amd64.client.open` for public PR builds, and `windows.11.amd64.client` with Entra authentication for internal builds. Also choose the operating system and architecture mix.
4. **Pipeline wiring.** Add a `send-to-helix` step, and possibly the Helix Job Monitor. PR builds must also produce the DRT payload.
5. **How failures are signalled.** Choose between the xUnit result file and the exit code (`RunDrts.cmd` or `CIRunDrts.cmd`).
6. **Clean machines.** Helix machines are shared. Each work item must delete `%APPDATA%\QualityVault` and stop leftover processes when it finishes.

## Investigation

### Test runners and how results are published (for the build-agent approach)

| | WinForms UI tests | MAUI Appium UI tests | MAUI device tests | dotnet/wpf `eng/wpfautomatedtests.yml` (DRTs) |
|---|---|---|---|---|
| Where tests run | Build agent | Build agent | Helix | Build agent |
| Runner | xUnit v3 (`TestRunnerName=XUnitV3`) through Arcade: `eng\cibuild.cmd -integrationTest /m:1` | NUnit, started by a Cake script (`windows.cake --target=uitest`) | xUnit, run inside the app by a custom in-app runner | QualityVault, through `RunDrts.cmd` |
| Result format | xUnit XML in `artifacts\TestResults\<config>\*.xml` | TRX | xUnit XML, merged into `testResults.xml` | xUnit XML in `%APPDATA%\QualityVault\Run\Report\testResults.xml`, written by QV's `XUnitReportGenerator` |
| Published to Azure DevOps by | `PublishTestResults@2` (`xUnit`) in `post-build-PR.yml` | `PublishTestResults@2` (`VSTest`) | The Helix SDK reporter (`EnableAzurePipelinesReporter=true`) | `PublishTestResults@2` (`XUnit`, `testRunTitle: 'CTP results'`) |
| Appears in the **Tests** tab | Yes, public builds only | Yes | Yes | Yes, but x64 only |
| A failed test fails the build | Yes, through the test step's exit code | Yes | Yes (`FailOnTestFailure=true`) | **No.** Test steps use `continueOnError: true`. |
| Extra artifacts | Logs, screenshots and screen recordings | Appium logs and screenshot diffs | Logs and event-log dumps | The whole QV `Report` folder, published as `TestResultsX64` or `TestResultsX86` |

Problems with `wpfautomatedtests.yml` to fix when reusing it:

- The QV report path is hard-coded to `C:\Users\cloudtest\AppData\Roaming\...`. Use `$(APPDATA)`, or pass `/RunDirectory`.
- `PublishTestResults@2` only reads the x64 `Results\` folder, so x86 results never reach the **Tests** tab.
- It downloads test binaries from internal pipeline definition 479 (`master`) instead of building them in the same job.
- It installs a fixed `8.0.0-alpha` runtime, then copies WPF binaries over it with `copy-wpf.ps1 -testhost`.
- Failures never fail the build. Either set `failTaskOnFailedTests: true` on the publish task or check `RunDrts.cmd`'s exit code.

Conclusion: all three use Azure DevOps' standard test-result integration. For the build-agent approach, the DRT job can run `RunDrts.cmd` and then publish QV's `Report\testResults.xml` with `PublishTestResults@2` (`xUnit`), with no new runner needed.

### Tracker

| # | Question | Status | Finding |
|---|---|---|---|
| I1 | Do Helix client queues run work items elevated? | ✅ Done | Yes. On `windows.11.amd64.client.open`, work items run as a local administrator at high integrity. UAC elevates administrators without a prompt. See [I1](#i1-elevation-on-helix-client-queues). |
| I2 | Do client queues give an interactive desktop session? | ✅ Done | Yes. Work items run in the active `console` session (session 1) with `UserInteractive=True`. The screen is 1024×768. See [I1](#i1-elevation-on-helix-client-queues). |
| I3 | How does `HelixCorrelationPayload` work, and where does it land on the machine? | ✅ Done | It is zipped and uploaded once per job, and unzipped into `%HELIX_CORRELATION_PAYLOAD%` (`C:\h\w\<job>\p`). Without `Destination`, the folder's contents land in that root. With `Destination`, they go into the named subfolder. See [I3](#i3-how-helixcorrelationpayload-works). |
| I4 | Which Windows client queues can be used? | ✅ Done | Use `windows.11.amd64.client.open` for public builds. `windows.10.amd64.client.open` reached its planned removal date on 2025-05-13 and did not run the probe. |
| I5 | How big is the DRT payload? | ✅ Done | The `DRT`, `Infra` and `Common` folders total about 95 MB, compared with about 1.8 GB for all published tests (Release x64). |
| I6 | Does `RunDrts.cmd` pass arguments such as `/Area=` on to QV? | ✅ Done | Yes, through `%*`. Whether `/Area` actually filters is item I7. |
| I7 | Does `/Area=` filter DRTs, given `SkipTargetAreaFiltering="true"`? | ✅ Done | Yes. `/Area=<area>` limits the run to that area's DRTs, as confirmed by the user, so one work item per area works. |
| I8 | Does the internal `windows.11.amd64.client` queue behave the same as the public one? | ⏸️ Deferred | The queue exists and is available, with planned removal on 2028-01-01. Probing it locally needs Entra authentication through Azure CLI, which is not installed. Check it later from an internal pipeline run, using the `I1` probe commands as a pre-command. |
| I9 | How do we deploy the WPF and .NET build under test to Helix? | 🟡 In progress | **Direction: provide the .NET SDK under test.** The DRT executables are framework-dependent: `*.runtimeconfig.json` requires `Microsoft.NETCore.App` and `Microsoft.WindowsDesktop.App` `11.0.0-rc.1.26420.103`, so they use whatever runtime the launcher resolves. Set `IncludeDotNetCli=true`, `DotNetCliPackageType=sdk` and `DotNetCliVersion=<sdk under test>` for a published SDK, or send a local `dotnet` folder as a correlation payload. A probe to check `DOTNET_ROOT`, the runtime lookup, and whether .NET is already installed on the machine was sent but stopped while still queued; it never ran. |
| I10 | Where is QV's xUnit report, and will Helix and Azure DevOps pick it up? | ⏳ Open | Must be copied into `%HELIX_WORKITEM_UPLOAD_ROOT%`. |
| I11 | Do the DRTs pass end-to-end on a client queue? | ⏳ Open | Send a real DRT payload, for one area or all. |
| I12 | Do any DRTs depend on a larger screen than 1024×768? | ⏳ Open | Comes out of I11. |
| I13 | Can DRTs run on their own, without QV? This matters for approach B. | 🟡 Partly done | At least some DRTs run when you call the DRT executable directly, as confirmed by the user. Not yet checked for every DRT. |
| I14 | Pipeline wiring: a `send-to-helix` step, the Job Monitor, and producing the payload in PR builds. | ⏳ Open | |
| I15 | Build agents: does `/azp run` work for a pipeline with `pr: none`? Who can trigger it? | ⏳ Open | Needed for C1. |
| I16 | Build agents: is there an elevated, interactive session, and at what resolution? | ✅ Done | Yes. In M0, jobs ran as `cloudtest`, a local administrator at High integrity, in the active `console` session 1 with `explorer.exe` running. The screen is 1024×768. The image is Windows Server 2025, and **Server Manager opens on the desktop at logon**, which may steal focus from UI tests. See [M0 results](drts-on-ci-implementation.md#m0-results). |
| I17 | Build agents: what job size and `maxParallel` to use across 21 areas? | ✅ Decided | One job per area with DRTs to run, no `maxParallel`. See the implementation plan. |
| I18 | Do DRTs leave machine-wide state (registry, `%ProgramFiles%\QualityVault`) on reused agents? | ⏳ Open | |
| I20 | Build agents: does each job get a freshly imaged VM, or are agents reused across jobs? | ✅ Done | Fresh VM per job. In M0, all five jobs ran on different machines whose OS had been installed 10–20 minutes earlier, and none found a marker left by another job. |
| I19 | Does QV report test failures through its exit code? | ✅ Done | No. `QualityVaultFrontend` returns `-1` only for infrastructure errors and 0 otherwise. `QV.cmd` then ends with `rd`, which drops even that exit code. `RunDrts.cmd` uses `findstr` on `DrtReport.xml` instead. See QV changes Q1 and Q2 in the [implementation plan](drts-on-ci-implementation.md#qualityvault-changes). |

### I1. Elevation on Helix client queues

I sent a probe job (a `probe.cmd` work item, submitted anonymously) to `windows.11.amd64.client.open`. The run was job `1778b659-5f46-45a3-ab37-239bd6525234` on machine `a000IZZ`, Windows 11 build 26100.

| Check | Result |
|---|---|
| User | `<machine>\runner`, a local account in `BUILTIN\Administrators` |
| Integrity level | `Mandatory Label\High Mandatory Level`, so the process is **already elevated** |
| `net session` | Succeeds. This command needs administrator rights. |
| UAC | `EnableLUA=1` and `ConsentPromptBehaviorAdmin=0`. UAC is on, but administrators are elevated without a prompt, so a `runas` launch will not hang. |
| Session | Session 1 is the `console` session, active and logged on as `runner`. The groups include `INTERACTIVE` and `CONSOLE LOGON`. |
| Desktop | `UserInteractive=True`, and the screen is 1024×768. |
| `%APPDATA%` | `C:\Users\runner\AppData\Roaming`, which is where QV writes its run folder. |

What this means:

- DRTs marked `UacElevation="Elevated"` can run as they are. No extra setup is needed.
- Work items run on a real interactive desktop, which WPF UI tests need.
- The screen is only 1024×768. Tests that assume a larger screen may need changes.
- `windows.10.amd64.client.open` reached its planned removal date on 2025-05-13. The probe sent to it never ran, so do not use that queue.
- The internal queue `windows.11.amd64.client` has not been checked, but it is expected to be set up the same way.

### I3. How HelixCorrelationPayload works

A correlation payload is a folder that is zipped once, uploaded once, and unzipped once on each machine. All work items in the same job share it. A work item payload (`PayloadDirectory`), by contrast, is separate for each work item.

```xml
<ItemGroup>
  <!-- Contents are unzipped directly into %HELIX_CORRELATION_PAYLOAD% -->
  <HelixCorrelationPayload Include="$(TestBinDir)" />
  <!-- Contents are unzipped into %HELIX_CORRELATION_PAYLOAD%\sub\dest -->
  <HelixCorrelationPayload Include="$(SomeOtherDir)" Destination="sub\dest" />
</ItemGroup>
```

The folder layout the probe saw on the machine:

```
%HELIX_CORRELATION_PAYLOAD%   = C:\h\w\<job>\p          (shared by every work item in the job)
  Infra\marker.txt                                       <- from a payload with no Destination
  sub\dest\other.txt                                     <- from a payload with Destination="sub\dest"
  scripts\<guid>\execute.cmd                             <- created by Helix
%HELIX_WORKITEM_PAYLOAD%      = C:\h\w\<job>\w\<wi>\u   (this work item's PayloadDirectory, unzipped)
%HELIX_WORKITEM_ROOT%         = C:\h\w\<job>\w\<wi>\e   (working folder when the Command starts)
%HELIX_WORKITEM_UPLOAD_ROOT%  = C:\h\w\<job>\w\<wi>\uploads
```

Things to know:

- **No `Destination`**: the folder's *contents* land directly in `%HELIX_CORRELATION_PAYLOAD%`, without the folder's own name. Several payloads without a destination are merged into the same root.
- **`Destination`**: places the contents in a subfolder.
- Other item metadata includes `Uri` (use an existing zip instead of uploading one), `AsArchive` (the item is already a `.zip`) and `IncludeDirectoryName` (keep the folder's name). These come from the Arcade source code and were **not** tested by the probe.
- `IncludeDotNetCli` and `AdditionalDotNetPackage` are delivered as correlation payloads too, which is how a runtime under test reaches the machine.

Payload layout proposed for the DRTs:

```xml
<ItemGroup>
  <!-- Shared by all 21 per-area work items: about 95 MB, uploaded once -->
  <HelixCorrelationPayload Include="$(TestBinDir)" />   <!-- a trimmed copy holding DRT\, Infra\, Common\, RunDrts.cmd, QV.cmd and the other entry scripts -->
  <HelixWorkItem Include="DRT-%(DrtArea.Identity)">
    <Command>call %HELIX_CORRELATION_PAYLOAD%\RunDrts.cmd /Area=%(DrtArea.Identity)</Command>
    <Timeout>00:30:00</Timeout>
  </HelixWorkItem>
</ItemGroup>
```

`RunDrts.cmd` passes its arguments (`%*`) on to QV, so `/Area=` reaches QV, and it does filter the DRTs (I7).

Work items must not write into `%HELIX_CORRELATION_PAYLOAD%`, because every work item on the machine shares it. QV's run folder (`%APPDATA%\QualityVault\Run`) sits outside it, but it must be copied to `%HELIX_WORKITEM_UPLOAD_ROOT%` so the results are kept, and then deleted.

How to send the same probe yourself:

- Use a `.proj` with `Sdk="Microsoft.DotNet.Helix.Sdk"`, version `12.0.0-beta.26505.2`, the same as Arcade.
- Set `BUILD_SOURCEBRANCH`, `BUILD_REPOSITORY_NAME`, `SYSTEM_TEAMPROJECT` and `BUILD_REASON`.
- Run `dotnet msbuild probe.proj /restore /p:Creator=<alias>`. Anonymous submissions to `.open` queues require a `Creator`.
