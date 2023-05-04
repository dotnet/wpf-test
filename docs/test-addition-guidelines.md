# Test Addition Guidelines

In this doc, we will try to give you a brief overview of the guidelines for adding tests. Depending on the area, for which you want to write the tests, there can be different ways of writing the tests.



## Adding Tests for new public API 

Writing tests for new public API changes is tricky because the installers/SDKs available in public feeds may not have the APIs that are being added. Hence, resulting in build failure. We need to build the test repo against the locally built WPF assemblies ( containing our API changes ).

### Building test repo using locally built WPF assemblies

1. **Build WPF binaries and copy your binaries into the local SDK**

    Build WPF repo using `build.cmd` and copy the resulting assembly(-ies) to your local SDK installation

    - Built assemblies are located at :
        - x86 and AnyCPU : `$(RepoRoot)\artifacts\packaging\Debug\Microsoft.DotNet.Wpf.GitHub.Debug\lib\net8.0`
        - x64 : `$(RepoRoot)\artifacts\packaging\Debug\x64\Microsoft.DotNet.Wpf.GitHub.Debug\lib\net8.0`
    - And need to be copied to the system dotnet folder `%Program Files%\dotnet\shared\Microsoft.WindowsDesktop.App\[Version]`

    If you have updated any public APIs, you'll need to overwrite the ref assemblies too:
    - The ref assemblies compile to :
        - x86 and AnyCPU : `$(RepoRoot)\artifacts\packaging\Debug\Microsoft.DotNet.Wpf.GitHub.Debug\ref\net8.0`
        - x64 : `$(RepoRoot)\artifacts\packaging\Debug\x64\Microsoft.DotNet.Wpf.GitHub.Debug\ref\net8.0`
    - These need to be copied to `%Program Files%\dotnet\packs\Microsoft.WindowsDesktop.App.Ref\[Version]`

    `%Program Files%` will be the path to `Program Files` or `Program Files(x86)` depending on the build architecture.

2. **Set environment variable**

    In order to use a local installation of .NET, we need to set the following environment variable with the path of the local installation of .NET
    ```
        $env:DotNetCoreSdkDir = "%PROGRAM_FILES%\dotnet\" 
    ```
3. **Update global.json**

Along with setting the environment variable, we need to remove the runtimes section from `global.json` and modify the `dotnet` parameter to the one installed on your system.
```diff
{
    "tools": {
-        "dotnet": "8.0.100-preview.3.23178.7",
+        "dotnet": "8.0.100-preview.1.23115.2",
-        "runtimes": {
-            "dotnet": [
-                "2.1.7",
-                "$(MicrosoftNETCoreAppVersion)"
-            ]
-        },
        "vs": {
            "version": "16.8"
        }
    ...
```
4. **Build the test repo for verification** 

    Run the build and once it is complete, open the binlog is produced, look for any compile task to verify which reference assemblies are being used for the build. The binlog is generated at `$(RepoRoot)\artifacts\log\Debug\$(Platform)\Build.binlog` 

    You can download [MSBuild Binary and Structured Log Viewer](https://msbuildlog.com/) to view the binlogs. 


Now, you can add your tests, build it and run locally.