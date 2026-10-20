# Implementing DRTs on CI

## Status

Draft plan. Nothing here has been implemented yet. For the background, required capabilities (C1–C4) and decisions, see [Running DRTs on CI](running-drts-on-ci.md).

## Scope

Phase 1 covers DRTs only, in wpf-test, on Azure DevOps build agents.

- **Configurations:** x64 Release first. x86 comes later.
- **WPF under test:** the .NET SDK pinned in `global.json`.
- **Out of scope for now:**
  - Feature tests.
  - Running in dotnet/wpf (phase 2).
  - The internal mirror (phase 3).
  - Helix.

The pipeline logic goes into a template so that dotnet/wpf can reuse it in phase 2.

## Files

| File | Purpose |
|---|---|
| `eng/pipelines/drts.yml` | Pipeline entry point. Defines the triggers and runtime parameters, then calls the template. |
| `eng/pipelines/templates/drt-stages.yml` | Reusable build and test stages. Parameters: the artifact source, platform, configuration, filters, and whether to rerun failures. |
| `eng/scripts/New-DrtPayload.ps1` | Collects the minimal DRT payload from `publish\<config>\<platform>\Test`. Also writes the matrix of areas. |
| `eng/scripts/Invoke-Drts.ps1` | Runs QV on the agent. Prepares the machine, runs, and sets the exit code. Used locally as well. |

The existing `azure-pipelines.yml` and `eng/pipeline.yml` stay unchanged. Running DRTs on every PR would slow every PR down, so the DRT pipeline is a separate pipeline definition in ADO.

## Pipeline

```mermaid
flowchart LR
  T[Trigger: /azp run, manual, or schedule] --> B
  subgraph Build stage
    B[CIBuild.cmd x64 Release] --> P[New-DrtPayload.ps1] --> A[(Artifact: DrtPayload, area matrix as an output variable)]
  end
  A --> M
  subgraph Test stage: one job per area
    M[Download payload] --> D[Install .NET from global.json] --> R[Invoke-Drts.ps1 -Area X] --> X[PublishTestResults: xUnit] --> L[Publish Report folder]
  end
```

### Triggers and parameters (C1, C2)

```yaml
trigger: none
pr: none          # to be verified: does `/azp run` need a pr: trigger? (I15)
schedules:
- cron: "0 2 * * *"
  branches: { include: [ main ] }
  always: false

parameters:
- name: areas       # empty means every area in DrtList.xml
  type: string
  default: ' '
- name: name        # forwarded to /Name=
  type: string
  default: ' '
- name: keywords    # forwarded to /Keywords=
  type: string
  default: ' '
- name: priority    # forwarded to /Priority=
  type: string
  default: ' '
- name: rerunFailures
  type: boolean
  default: true
```

- **PR comments:** `/azp run <drt-pipeline-name>` runs every DRT with the default parameters.
- **Filtered runs:** use the ADO **Run pipeline** dialog. Filtering from a PR comment would need a bot and is deferred.

### Stage 1: Build

One job on the same pool and image as `eng/pipeline.yml` (`NetCore-Public`, `windows.vs2026preview.scout.amd64.Open`).

1. `checkout: self`.
2. `eng\scripts\CIBuild.cmd -configuration Release -prepareMachine /p:Platform=x64`.
3. `New-DrtPayload.ps1 -Configuration Release -Platform x64 -Areas '${{ parameters.areas }}'`.
   - Copies `DRT\`, `Infra\` and `Common\`, the `*.cmd` scripts, `DiscoveryInfoDrts.xml`, `xsltTranform\`, and `Invoke-Drts.ps1` into `artifacts\DrtPayload`. That is about 95 MB, compared with 1.8 GB for the full `Test` folder.
   - Reads `DRT\DrtList.xml` and builds the area matrix. If areas are given, it keeps only those areas.
   - Writes the matrix as an output variable: `##vso[task.setvariable variable=matrix;isOutput=true]{"Annotations":{"area":"Annotations"},...}`.
4. `PublishPipelineArtifact`: `DrtPayload_x64_Release`.

Building the matrix from `DrtList.xml` means that new or renamed areas need no YAML change.

### Stage 2: Test (C3, C4)

```yaml
- stage: Test
  dependsOn: Build
  jobs:
  - job: Drt
    strategy:
      matrix: $[ stageDependencies.Build.Build.outputs['areas.matrix'] ]
      maxParallel: 8          # to be tuned against agent availability
    timeoutInMinutes: 60
    workspace: { clean: all }
```

Steps:

1. `checkout: self` with `fetchDepth: 1`. This is only needed for `global.json` and `eng/common`.
2. Download `DrtPayload_x64_Release` to `$(Pipeline.Workspace)\drt`.
3. Install the .NET SDK from `global.json` with `eng/common/tools.ps1` (`InitializeDotNetCli -install`). Then set `DOTNET_ROOT`, and `DOTNET_MULTILEVEL_LOOKUP=0` so that the DRTs use this runtime and not one installed on the machine.
   - **Decision:** install the SDK in every job. The DRTs are framework-dependent, so each machine needs the exact runtime. Shipping only the runtime in the payload would be smaller, but installing the SDK keeps the payload simple and matches how the build gets .NET. Revisit this if the install time per job becomes a problem.
4. `Invoke-Drts.ps1 -Area $(area) [filters] [-RerunFailures]`.
   - Before the run: delete the run directory and stop any leftover test processes.
   - Run QV with `/RunDirectory=$(Agent.TempDirectory)\QV\Run`, a known path rather than `%APPDATA%`.
   - Exit with a non-zero code if `Report\DrtReport.xml` is missing or the pass rate is below 100%. This applies until QV change Q2 is made.
5. `PublishTestResults@2` with `condition: always()`:
   - `testResultsFormat: XUnit`.
   - `testResultsFiles: $(Agent.TempDirectory)\QV\Run\Report\testResults.xml`.
   - `testRunTitle: DRT $(area) x64 Release (attempt $(System.JobAttempt))`.
   - `failTaskOnFailedTests: true`.
   - `failTaskOnMissingResultsFile: true`.
6. `PublishPipelineArtifact` with `condition: always()`:
   - Contents: the `Report` folder and any `*.dmp` files.
   - Name: `DrtReport_$(area)_$(System.JobAttempt)`. A rerun needs a unique artifact name, so the attempt number is part of the name.

How the capabilities map:

- **C3, retries:** QV's `/RerunFailures` retries a failing DRT once. `retryCountOnTaskFailure` is **not** used at first, because it would hide failing runs.
- **C4, rerunning failed areas:** a failed area fails its own job, so **Rerun failed jobs** reruns only those areas and reuses the same payload. To rerun one test, run the pipeline manually with `name=` set.
- **No `continueOnError`.**

## QualityVault changes

Each change below makes QV easier to drive from a pipeline. They are listed in order of priority. None of them blocks a first working pipeline, because `Invoke-Drts.ps1` can work around all of them.

| # | Change | Why | Where |
|---|---|---|---|
| Q1 | Keep `QualityVaultFrontend`'s exit code in `QV.cmd` (`set rc=%errorlevel%` … `exit /b %rc%`). | `QV.cmd` ends with `rd`, so the frontend's `-1` for an infrastructure failure is lost and the step looks successful. | `src\Test\BranchCommon\data\QV.cmd` |
| Q2 | Return a distinct exit code when tests fail: for example `0` for all passed, `1` for test failures, `-1` for an infrastructure error. Behind a switch such as `/FailOnTestFailure` so local behavior doesn't change. | Today `Main` returns 0 whenever the run finishes, so scripts check the pass rate in `DrtReport.xml` with `findstr`. | `QualityVaultFrontend\MainEntry.cs`, `RunCommand.cs` |
| Q3 | Better xUnit report: name each `<assembly>` after its area; set `type` to the area and `method` to the test name; add `<trait>`s for Area, SubArea, Owner and Priority; put a short failure reason in `<message>` and the full log in `<stack-trace>` or `<output>`. | Today each assembly is named after the first test's executable, `type` and `method` are empty for DRTs, and the message is the whole log. The ADO Tests tab is hard to read. | `QualityVaultUtilities\Reporting\XUnitReportGenerator.cs` |
| Q4 | Record a pass on rerun: keep the first attempt's failure, and report the test as passed with a `Flaky=true` trait and a note in `<output>`. | `ClearResult` drops the first failure, so flaky tests are invisible. The xUnit format has no "flaky" outcome, so a trait is the best available option. | `ExecutionEngine.cs` (rerun around L335 and L386), `XUnitReportGenerator.cs` |
| Q5 | `/RerunCount=N` instead of a single fixed rerun. | Makes it possible to tune retries. | `RunCommand.cs`, `ExecutionEngine.cs` (`ShouldRerun`) |
| Q6 | Let QV run without copying `Infra` into `%ProgramFiles%`, for example with an environment variable or switch that runs it in place. | Copying needs administrator rights and shares one machine-wide location between runs. | `QV.cmd` |
| Q7 | Remove the unused Helix result-upload code (`HELIX_RESULTS_CONTAINER_*`). | Cleanup. | `XUnitReportGenerator.cs` |

`RunDrts.cmd` and `CIRunDrts.cmd` stay as they are for local use. The pipeline uses `Invoke-Drts.ps1`, which calls `QV.cmd` directly.

## Milestones

| # | Milestone | Done when |
|---|---|---|
| M0 | **Probe the agent.** A temporary PR pipeline step runs `CIRunDrts.cmd /Area=<small area>` and logs elevation, session, resolution and the `%APPDATA%` path. A small matrix of 3–4 jobs also logs the machine name, OS install time, boot time and agent uptime, and writes then checks for a marker file under `%ProgramData%`. | We know whether DRTs can run on `windows.vs2026preview.scout.amd64.Open` as they are, and whether each job gets a freshly imaged machine or agents are reused across jobs (I20). |
| M1 | `drts.yml` with a single test job that runs every DRT, publishes results and the report. | A manual run shows the DRT results in the Tests tab. |
| M2 | A matrix with one job per area, built from `DrtList.xml`, plus filter parameters. | **Rerun failed jobs** reruns only failed areas. |
| M3 | QV changes Q1–Q3. | Exit codes are reliable and the Tests tab is readable. |
| M4 | `/azp run` comment trigger and the nightly schedule. | A wpf-test PR comment starts a DRT run. |
| M5 | QV changes Q4 and Q5. | Flaky tests are visible. |
| M6 | Phase 2: dotnet/wpf uses the template with locally built WPF. | A dotnet/wpf PR can start a DRT run. |

## Open questions

- **I15:** Does `/azp run` work for a pipeline with `pr: none`? Which GitHub permissions does a user need to trigger it?
- **I16:** On the agent, does the interactive session's resolution meet what the DRTs need? MAUI sets 1920×1080 before its UI tests.
- **I17:** Is there a sensible `maxParallel` and job size? There are 21 areas; small ones could be grouped.
- **I20:** Does every job in `NetCore-Public` get a freshly imaged VM, or can one agent run several jobs one after another? Each matrix job runs on its own agent, but a reused agent would carry state between jobs. If agents are reused, per-job cleanup in `Invoke-Drts.ps1` becomes required, not just a precaution. To be answered in M0.
- **I18:** Do the DRTs leave machine-wide state (registry, `%ProgramFiles%\QualityVault`, fonts, themes) that matters on reused agents?
