# Building, Running Tests

## Prerequisites

Follow the prerequisites listed at [Machine Setup](developer-guide.md) before building the repo.

## Building Tests

In the root of the repo, we have a script `build.cmd` which works similar to the build scripts in other dotnet repos.

For building the tests, you can simply execute the following command:
```
  .\build.cmd -platform $(Platform) -configuration $(Configuration)
```

where, 
  `$(Configration)` can be either 'Debug' or 'Release',
  `$(Platform)` can be x86 or x64

Use `/help` parameter to check the different arguments that can be passed along with the build command.

### Building Tests Suite or Area wise

By default the command above builds the whole test suite as it picks the solution file in the folder where the script exists

In order to build the tests for a single area, use the `-projects` argument like this
```
  .\build.cmd -project .\src\Test\Annotations.sln
```
Each area has it's own sln file which can be used to build the area seperately.

As of now we don't have a seperate sln file for DRTs 

### Understanding build outputs

The repo is designed in a way that the final results we get after the build are in a matter of speaking self-contained.
Once, the build is complete, navigate to `$(RepoRoot)\publish\test\$(Configuration)\$(Platform)\Test` and you will see the following structure:

```
Test
 |
 |--  Common
 |
 |--  DRT
 |
 |--  FeatureTests
 |      |
 |      |-- Annotations
 |      |-- Diagnostics
 |      |-- ...
 |      
 |--  Infra
 |--  CIRunDrts.cmd            // For running tests in CI
 |--  CommonData.deps.json
 |--  DiscoveryInfo.xml
 |--  DiscoverInfoDrts.xml
 |--  DQV.cmd
 |--  QV.cmd
 |--  DrtReportToHtml.cmd
 |--  RunDrts.cmd
 |--  RunDrtsDebug.cmd
 |--  RunTests.cmd
 |--  RunTestsDebug.cmd
```

- `RunDrts.cmd` and `RunTests.cmd` : These are used to runthe DRTs and Feature Tests respectively. These files call `QV.cmd` which is responsible for lauching the test infra.
- `RunDrtsDebug.cmd` and `RunTestsDebug.cmd` : They are similar to `RunDrts.cmd` and `RunTests.cmd`, the only difference being that they use `DQV.cmd` ( Debug Quality Vault ) to run the tests. These files are used when we need to debug the **test infrastructure**.
- `QV.cmd` and `DQV.cmd`: These scripts are responsible for an xcopy deployment of QualityVault ( test infra ). They are not supposed to be used directly. Developers\testers need to call `RunDrts*` or `RunTests*` to run the tests. The difference between these two is that `DQV.cmd` launches the test infra with a debugger attached.
- `DiscoveryInfoDrts.xml` and `DiscoveryInfo.xml` : These file are used by the test infra to discover all the tests that need to be run for the current command. 
- `Common` and `Infra` :  These folders contain binaries corresponsing to the test infrastructure.
- `DRT` : Contains all the files required for running the DRT Test suite.
- `FeatureTests` : Consists of subdirectories corresponding to each feature area. Each subdirectory contains the files required by that area to run the tests specific to that area.

*PS: By self-contained we mean, you can copy this folder to any machine with WPF installed and use `.\RunDrts.cmd` and `.\RunTests.cmd` to run the tests.*

## Running Tests

Once, you have built the tests,`cd` into `$(RepoRoot)\publish\test\$(Configuration)\$(Platform)\Test`. and run the tests using `RunDrts.cmd` or `RunTests.cmd`. `RunDrts.cmd` will run the DRT test suite whereas `RunTests.cmd` are used to run feature tests.

### Running DRTs

### Running Feature Tests

### Running Microsuites

### Running P0/P1/.. tests

### Running specific tests

 You can use `/Area` and `/Name` parameters to run tests from a specific area or with a certain name.

At the end of the run, you should see something like this:

```
  A total of 84 test Infos were processed, with the following results.
   Passed: 84
   Failed (need to analyze): 0
   Failed (with BugIDs): 0
   Ignore: 0

```

### Running tests on locally built WPF assemblies

### Debugging Tests

If there were any failures, run the tests manually with the `/debugtests` flag using the `RunDrts.cmd` script. Note that you do not run the `RunDrtsDebug` script, as this will debug the test infrastructure, `QualityVault`. When you pass the `/debugtests` flag, a cmd window will open where you can open the test executable in Visual Studio and debug it. When the cmd pops up, you will see instructions for debugging using a few different commands, however these commands will enable you to debug the `Simple Test Invocation` executable, `sti.exe`, which simply launches the test executable you are most likely interested in debugging. Using `DrtXaml.exe` as an example, this is how you can debug the test executable. Any MSBuild style properties should be replaced with actual values:

1. `$(RepoRoot)\artifacts\test\$(Configuration)\$(Platform)\Test\RunDrts.cmd /name=DrtXaml /debugtests`
2. Enter following command into the cmd window that pops up:
`"%ProgramFiles%\Microsoft Visual Studio\2022\Preview\Common7\IDE\devenv.exe" DrtXaml.exe`
1. Once Visual Studio is open, go to `Debug-> DrtXaml Properties` and do the following:
    - Manually change the `Debugger Type` from `Auto` to `Mixed (CoreCLR)`.
    - Change the `Environment` from `Default` to a custom one that properly defines the `DOTNET_ROOT` variable so that the host is able to locate the install of `Microsoft.NETCore.App`.
      - x86 (Default): Name: `DOTNET_ROOT(x86)` Value: `$(RepoRoot).dotnet\x86`
      - x64 (/p:Platform=x64): Name: `DOTNET_ROOT` Value: `$(RepoRoot).dotnet` 
2. From there you can F5 and the test will execute.

*NOTE: Some tests require the screen resolution to be set to 1920 x 1080.*

*NOTE: This requires being run from an admin window at the moment.*