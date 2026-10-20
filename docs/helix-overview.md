# Helix Overview

This document summarizes what Helix is, how .NET repositories use it, what it can and cannot do, and how it relates to testing in dotnet/wpf today. For the original design notes on running WPF tests in Helix, see [Testing in Helix](testing-in-helix.md). For the authoritative and most current reference, see the Arcade [Helix SDK readme](https://github.com/dotnet/arcade/blob/main/src/Microsoft.DotNet.Helix/Sdk/Readme.md).

## What Helix is

Helix is the distributed test execution service run by the .NET engineering team (dnceng) at [helix.dot.net](https://helix.dot.net/). A build submits a **job** made up of one or more **work items**. Each work item is a command that runs on a machine pulled from a named **queue** (for example `Windows.10.Amd64.Open`). Results are reported back to Azure DevOps.

Helix lets a repository run its tests across many operating systems and machine configurations in parallel, without having to own or maintain those machines.

## How a repository uses Helix

1. **Define a Helix project.** Create an MSBuild project that uses the `Microsoft.DotNet.Helix.Sdk` SDK. Arcade ships a generic one at [`eng/common/helixpublish.proj`](https://github.com/dotnet/wpf/blob/main/eng/common/helixpublish.proj). The project defines:
   - `HelixTargetQueues`: the semicolon-separated list of queues to run on. The project is built once per queue.
   - `HelixWorkItem` items: the `Command` to run, an optional `PayloadDirectory` that is zipped and sent with the work item, an optional `Timeout`, and optional `PreCommands` / `PostCommands`.
   - `HelixCorrelationPayload` items: directories shared by every work item in the job. They are unzipped under `HELIX_CORRELATION_PAYLOAD` on the machine.
   - `HelixPreCommands` / `HelixPostCommands`: commands that run before or after every work item.
   - `HelixSource`, `HelixType` and `HelixBuild`: labels used by Helix to identify and report on the job. `HelixSource` must start with `pr/`, `official/`, `prodcon/` or `agent/`.
2. **Submit from a pipeline.** Use the [`eng/common/templates/steps/send-to-helix.yml`](https://github.com/dotnet/wpf/blob/main/eng/common/templates/steps/send-to-helix.yml) step template, which builds the Helix project and sends the job.
3. **Optionally use built-in test runners.** Instead of writing work items by hand, list `XUnitProject` or `MTPProject` (Microsoft.Testing.Platform) items to get one work item per test project. Set `IncludeDotNetCli=true` (with `DotNetCliPackageType` and `DotNetCliVersion`) to ship a .NET SDK or runtime to the machine as a correlation payload.

A minimal Helix project looks like this:

```xml
<Project Sdk="Microsoft.DotNet.Helix.Sdk" DefaultTargets="Test">
  <PropertyGroup>
    <HelixSource>pr/testing/</HelixSource>
    <HelixType>test/stuff/</HelixType>
    <HelixBuild>23456.01</HelixBuild>
    <HelixTargetQueues>Windows.10.Amd64.Open</HelixTargetQueues>
  </PropertyGroup>

  <ItemGroup>
    <HelixWorkItem Include="Run tests">
      <Command>runtests.cmd</Command>
      <PayloadDirectory>$(TestFolder)</PayloadDirectory>
      <Timeout>00:30:00</Timeout>
    </HelixWorkItem>
  </ItemGroup>
</Project>
```

### Useful environment variables on Helix machines

| Variable | Meaning |
|---|---|
| `HELIX_CORRELATION_ID` | ID of the Helix job. Include it when contacting dnceng. |
| `HELIX_CORRELATION_PAYLOAD` | Root folder where correlation payloads are unzipped. |
| `HELIX_WORKITEM_PAYLOAD` | Folder where the work item payload was unzipped. |
| `HELIX_WORKITEM_ROOT` | Folder where the work item is executed. |
| `HELIX_WORKITEM_UPLOAD_ROOT` | Files left here at the end of the work item are uploaded and kept. |
| `HELIX_DUMP_FOLDER` | Process dumps written here are uploaded automatically. |
| `HELIX_PYTHONPATH` | Path to a Python 3 executable. |
| `HELIX_WORKITEM_FRIENDLYNAME` | The work item name given at submission time. |

## Capabilities

- **Parallel runs across many OSes.** The same work items can fan out across several queues at once.
- **Docker.** A work item can run inside a container using the queue format `queue@image` (for example `(Alpine.39.Amd64)ubuntu.1604.amd64.open@mcr.microsoft.com/...`).
- **Result reporting.** xUnit, JUnit and TRX result files are published to the Azure DevOps **Tests** tab. Files in `HELIX_WORKITEM_UPLOAD_ROOT` and dumps in `HELIX_DUMP_FOLDER` are uploaded.
- **Test retries and quarantine.** An `eng/test-configuration.json` file can rerun individual flaky tests (reported as "Passed on Rerun") and quarantine tests, based on test name, assembly or failure message rules.
- **Work item helpers.** From a work item, `helix.public` (Python) can request an infrastructure retry, request a machine reboot after the work item, or send custom metrics.
- **Helix Job Monitor.** A separate, lightweight pipeline job (`eng/common/core-templates/job/helix-job-monitor.yml`) waits on Helix instead of the build job. With `EnableHelixJobMonitor=true`, the submitting job exits as soon as the work is queued, results are published as each job completes, and re-running the monitor job resubmits only failed work items.
- **Mobile and WASM.** Android, iOS and WASM tests are supported through XHarness on specially equipped queues.

## Constraints

- **Access.** Queues ending in `.Open` accept anonymous jobs and their results are public. Internal queues require Entra authentication (`HelixUseEntraAuthentication`) or the legacy `HelixApiAccessToken` from the `DotNet-HelixApi-Access` variable group. When a token is set, `Creator` must be empty.
- **Python dependency.** Helix uses Python 3 on the machine for uploading results and other client features. Custom Docker images must include it.
- **Shared, reused machines.** Machines run work items from many repositories. Work items should clean up after themselves (WPF's `runtests.ps1` deletes QualityVault's AppData folder for this reason).
- **Timeouts.** Every work item has a timeout. MTP work items default to 5 minutes.
- **Queues are retired over time.** A job fails if a target queue no longer exists, unless `FailOnMissingTargetQueue=false` is set. Check [the queue list](https://helix.dot.net/api/2019-06-17/info/queues) before choosing queues.
- **Docker on Windows.** Windows Docker hosts only run Windows containers, and the container OS version must match the host.
- **Infrastructure retries are expensive.** A retried work item goes to the back of the queue, which can make a job much longer.
- **UI tests need an interactive desktop.** WPF tests need a logged-on user session, so WPF used "Client" queues (for example `Windows.10.Amd64.ClientRS5`) rather than server queues.
- **Non-xUnit frameworks need adapting.** Helix understands xUnit, JUnit and TRX results. Other test harnesses (such as WPF's QualityVault) must produce one of those formats and place the file in the work item directory.

## Helix in dotnet/wpf today

WPF no longer submits jobs to Helix from the dotnet/wpf repository. Parts of the original setup remain:

- **Test payload creation is disabled.** The import of `CreateTestPayload.targets` is commented out in [`eng/WpfArcadeSdk/Sdk/Sdk.targets`](https://github.com/dotnet/wpf/blob/main/eng/WpfArcadeSdk/Sdk/Sdk.targets).
- **No pipeline sends a Helix job.** Neither [`azure-pipelines-pr.yml`](https://github.com/dotnet/wpf/blob/main/azure-pipelines-pr.yml) nor [`eng/wpfautomatedtests.yml`](https://github.com/dotnet/wpf/blob/main/eng/wpfautomatedtests.yml) uses `send-to-helix.yml`. Both still define Helix variables (`_TestHelixAgentPool`, `_HelixSource`, `_HelixToken`, `_HelixCreator`), but nothing reads them.
- **Tests run on the build agent.** `eng/wpfautomatedtests.yml` downloads prebuilt test binaries from another pipeline, runs `RunDrts.cmd` and `RunTests.cmd` (Microsuites, and optionally feature or full suites) directly, and publishes `testResults.xml` with `PublishTestResults@2`.
- **Leftover Helix-aware scripts.** [`eng/WpfArcadeSdk/tools/runtests.ps1`](https://github.com/dotnet/wpf/blob/main/eng/WpfArcadeSdk/tools/runtests.ps1) and [`configure-machine.ps1`](https://github.com/dotnet/wpf/blob/main/eng/WpfArcadeSdk/tools/configure-machine.ps1) still use `HELIX_*` variables, for example to upload `DrtReport.xml` with `upload_result.py` when run with `-ci`.
- **One DRT.** The only DRT in dotnet/wpf is `DrtXaml` (`src/Microsoft.DotNet.Wpf/tests/IntegrationTests/DRT/DrtXaml`), which was also the project responsible for creating the Helix test payload.

As a result, parts of [Testing in Helix](testing-in-helix.md), including its queue list, describe the earlier setup rather than the current one.
