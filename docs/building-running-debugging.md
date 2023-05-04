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
  `$(Configuration)` can be either 'Debug' or 'Release',
  `$(Platform)` can be x86 or x64

Use `/help` parameter to check the different arguments that can be passed along with the build command.

### Building Tests Suite or Area wise

By default the command above builds the whole test suite as it picks the solution file in the folder where the script exists

In order to build the tests for a single area, use the `-projects` argument like this
```
  .\build.cmd -project .\src\Test\Annotations.sln
```
Each area has it's own sln file which can be used to build the area separately.

As of now we don't have a separate .sln file for DRTs 

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

- `RunDrts.cmd` and `RunTests.cmd` : These are used to run the DRTs and Feature Tests respectively. These files call `QV.cmd` which is responsible for launching the test infra.
- `RunDrtsDebug.cmd` and `RunTestsDebug.cmd` : They are similar to `RunDrts.cmd` and `RunTests.cmd`, the only difference being that they use `DQV.cmd` ( Debug Quality Vault ) to run the tests. These files are used when we need to debug the **test infrastructure**.
- `QV.cmd` and `DQV.cmd`: These scripts are responsible for an xcopy deployment of QualityVault ( test infra ). They are not supposed to be used directly. Developers\testers need to call `RunDrts*` or `RunTests*` to run the tests. The difference between these two is that `DQV.cmd` launches the test infra with a debugger attached.
- `DiscoveryInfoDrts.xml` and `DiscoveryInfo.xml` : These file are used by the test infra to discover all the tests that need to be run for the current command. 
- `Common` and `Infra` :  These folders contain binaries corresponding to the test infrastructure.
- `DRT` : Contains all the files required for running the DRT Test suite.
- `FeatureTests` : Consists of subdirectories corresponding to each feature area. Each subdirectory contains the files required by that area to run the tests specific to that area.

*PS: By self-contained we mean, you can copy this folder to any machine with WPF installed and use `.\RunDrts.cmd` and `.\RunTests.cmd` to run the tests.*

## Running Tests

Once, you have built the tests,`cd` into `$(RepoRoot)\publish\test\$(Configuration)\$(Platform)\Test` ( from here onwards, we will be referring to this directory/path as `$(TestBinDir)` ) and run the tests using `RunDrts.cmd` or `RunTests.cmd`. 

At the end of the run, you should see something like this:

```
  A total of 84 test infos were processed, with the following results.
   Passed: 84
   Failed (need to analyze): 0
   Failed (with BugIDs): 0
   Ignore: 0
```

Once the tests run, the results are generated here `C:\Users\$(CurrentUser)\AppData\Roaming\QualityVault\Run\Report`. You can see the result of the run in `testResults.xml` file.

***NOTE: Some tests require the screen resolution to be set to 1920 x 1080.***

***NOTE: This requires being run from an admin window at the moment.***

### Running DRTs, Feature Tests and MicroSuites:

Once, you have opened `cmd` with admin privileges and navigated to `$(TestBinDir)` you can use the following to run the different suites:

* To run DRT suite:
  ```
  .\RunDrts.cmd [/Area=<area-name>] [/SubArea=<subarea-name>] [/Name=<name-of-test>]
  ```
* To run Features Tests suite:
  ```
  .\RunTests.cmd [/Area=<area-name>] [/SubArea=<subarea-name>] [/Name=<name-of-test>]
  ```
* To run Microsuite tests:
  ```
  .\RunTests.cmd /Keywords=Microsuite [/Area=<area-name>] [/SubArea=<subarea-name>] [/Name=<name-of-test>]
  ```
* To run P0/P1/.. tests:
  ```
  .\RunTests.cmd /Priority=0,1 [/Area=<area-name>] [/SubArea=<subarea-name>] [/Name=<name-of-test>]
  ```
  This command above runs will run the P0 and P1 tests. The priorities range from 0-4.


### Running specific tests

When our changes are contained with one component, say controls like Datagrid, VSP or an area like XAML, data binding, we can avoid running the whole test suite by specifying the `/Area`, `/SubArea` and `/Name` arguments while running the `RunDrts.cmd` or `RunTests.cmd` scripts.

Here are some examples:

* `.\RunTests.cmd /Area=Controls /Subarea=Datagrid`
    This will run all the feature tests related to Datagrid control.
* `.\RunTests.cmd /Area=Controls /Subarea=VirtualizingStackPanel,VirtualizedScrolling`
    This will run the feature tests related to virtualization and VSP
* `.\RunTests.cmd /Area=XamlV3,XamlV4 /Keywords=Microsuite`
    This will run the microsuites from XAML area ( PS : XAML area tests are included in 2 areas `XamlV3` and `XamlV4`)
* `.\RunDrts.cmd /Name=DrtWindow`
    This will run the DRT named `DrtWindow`.

You can find the list of all the areas and subareas in [test suites](test-suites.md#categorization-on-the-basis-of-area).

### Understanding run outputs

When you run the tests all the run information are stored in a `RunDirectory`. By default, we use this `C:\Users\$(CurrentUser)\AppData\Roaming\QualityVault\Run` for storing all the run information.

Once, the run tests command completes, reports for the tests are generated at `$(RunDirectory)\Report`. By default this location is `C:\Users\$(CurrentUser)\AppData\Roaming\QualityVault\Run\Report`.

The general structure of the directory is as follows:
```
$(RunDirectory)
 |
 |--  RunInfo.xml
 |--  TestCollection.xml
 |
 |--  <desktop-name>                  // Contains all the run data information
 |
 |--	Report                          // Contains the reports generated after the run
    	 |--  AreaReport
    	 |      |
    	 |      |-- TestInfos
    	 |      |-- TestLogs
    	 |      |-- <area>VariationReport.xml
    	 |      
     	 |--  DrtReport.xml             // Only when you run RunDrts.cmd
     	 |--  DrtReport.xsl            
     	 |--  FilteringReport.xml           
     	 |--  FilteringReport.xsl
     	 |--  InfraTrackingReport.xml
     	 |--  InfraTrackingReport.xsl
     	 |--  LabReport.xml
     	 |--  LabReport.xsl
     	 |--  MachineSummary.xml
     	 |--  MachineSummary.xml
     	 |--  Summary.xml
     	 |--  Summary.xsl
     	 |--  testResults.xml
     	 |--  VariationReport.xsl
```
- `$(RunDirectory)\<desktop-name>` : Contains all the data from the run. This directory is generated while the tests are running and support files or other data needed for tests are copied here for running that test.
- `$(RunDirectory)\Report` : Contains all the report files generated after the tests execution is complete.
- `AreaReport` : Area specific reports related to the tests.
- `AreaReport\TestInfos` : Test Infos for the failed tests are stored here.
- `AreaReport\<area>VariationReport.xml` : Area specific test run summary of the results.
- `DrtReport.xml` : Contains the result of `RunDrts.cmd` command.
- `FilteringReport.xml` : List of tests that are ignored. A test can be ignored either if it is disabled or else when it is filtered out ( this happens when we pass in arguments `/Area`, `/Subarea` or `/Name` )
- `Summary.xml` : Contains the area-wise summary of the run.
- `testResults.xml` : Contains the result of all the tests that were run. If the test failed, you can find it here. You will also get the repro arguments related to the failing test here.
 
## Running tests on locally built WPF assemblies

For running tests on locally built WPF assemblies, the rest of the process ( building and running tests will remain the same ), however, we will need to replace the installed WPF binaries with our locally built ones. Once done, run the tests as mentioned above.

### Replace locally built WPF binaries to your local SDK installation

Clone and build [WPF](https://github.com/dotnet/wpf) repo using `build.cmd` and copy the resulting assembly(-ies) to your local SDK installation

- Built assemblies are located at :
    - x86 and AnyCPU : `$(WpfRepoRoot)\artifacts\packaging\Debug\Microsoft.DotNet.Wpf.GitHub.Debug\lib\net8.0`
    - x64 : `$(WpfRepoRoot)\artifacts\packaging\Debug\x64\Microsoft.DotNet.Wpf.GitHub.Debug\lib\net8.0`
- And need to be copied to the system dotnet folder `%Program Files%\dotnet\shared\Microsoft.WindowsDesktop.App\[Version]`

If you have updated any public APIs, you'll need to overwrite the ref assemblies too:
- The ref assemblies compile to :
    - x86 and AnyCPU : `$(WpfRepoRoot)\artifacts\packaging\Debug\Microsoft.DotNet.Wpf.GitHub.Debug\ref\net8.0`
    - x64 : `$(WpfRepoRoot)\artifacts\packaging\Debug\x64\Microsoft.DotNet.Wpf.GitHub.Debug\ref\net8.0`
- These need to be copied to `%Program Files%\dotnet\packs\Microsoft.WindowsDesktop.App.Ref\[Version]`

`%Program Files%` will be the path to `Program Files` or `Program Files(x86)` depending on the build architecture.

## Debugging Tests and Test Infrastructure

If there were any failures, run the tests manually with the `/debugtests` flag using the `RunDrts.cmd` or `RunTests.cmd` script. Note that you do not run the `RunDrtsDebug` or `RunTestsDebug` script, as this will debug the test infrastructure, `QualityVault`. 

When you pass the `/debugtests` flag, a cmd window will open where you can open the test executable in Visual Studio and debug it. When the cmd pops up, you will see instructions for debugging using a few different commands, however these commands will enable you to debug the `Simple Test Invocation` executable, `sti.exe`, which simply launches the test executable you are most likely interested in debugging. Using `DrtXaml.exe` as an example, this is how you can debug the test executable. Any MSBuild style properties should be replaced with actual values:

1. `$(RepoRoot)\artifacts\test\$(Configuration)\$(Platform)\Test\RunDrts.cmd /name=DrtXaml /debugtests`
2. Enter following command into the cmd window that pops up:
`"%ProgramFiles%\Microsoft Visual Studio\2022\Preview\Common7\IDE\devenv.exe" DrtXaml.exe`
3. Once Visual Studio is open, go to `Debug-> DrtXaml Properties` and do the following:
    - Manually change the `Debugger Type` from `Auto` to `Mixed (CoreCLR)`.
    - Change the `Environment` from `Default` to a custom one that properly defines the `DOTNET_ROOT` variable so that the host is able to locate the install of `Microsoft.NETCore.App`.
      - x86 (Default): Name: `DOTNET_ROOT(x86)` Value: `$(RepoRoot).dotnet\x86`
      - x64 (/p:Platform=x64): Name: `DOTNET_ROOT` Value: `$(RepoRoot).dotnet` 
4. From there you can F5 and the test will execute.

### Debugging DRTs with -wait parameter

Some of the DRTs derive from `DrtBase` which supports passing a `-wait` parameter which stops the execution of the DRT until a debugger is attached. Once, you attach a debugger to the process the execution starts again and you can debug the test.