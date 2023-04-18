
## Running Tests Locally

In order to run the tests on your local machine,

- Build the tests with `build.cmd` script. Use `/help` parameter to check the different arguments that can be passed along with the build command.
- cd into `$(RepoRoot)\publish\test\$(Configuration)\$(Platform)\Test` and run `RunDrts.cmd` to run the tests. You can use `/Area` and `/Name` parameters to run tests from a specific area or with a certain name.

At the end of the run, you should see something like this:

```
  A total of 84 test Infos were processed, with the following results.
   Passed: 84
   Failed (need to analyze): 0
   Failed (with BugIDs): 0
   Ignore: 0

```

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