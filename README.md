# Windows Presentation Foundation (WPF) Test

[![Build Status](https://dnceng.visualstudio.com/public/_apis/build/status/dotnet/wpf/dotnet-wpf%20CI)](https://dnceng.visualstudio.com/public/_build/latest?definitionId=270)
[![MIT License](https://img.shields.io/badge/license-MIT-green.svg)](https://github.com/dotnet/wpf/blob/master/LICENSE.TXT)

Windows Presentation Foundation (WPF) is a UI framework for building Windows desktop applications. WPF supports a broad set of application development features, including an application model, resources, controls, graphics, layout, data binding and documents. WPF uses the Extensible Application Markup Language (XAML) to provide a declarative model for application programming.

WPF applications are based on a vector graphics architecture. This enables applications to look great on high DPI monitors, as they can be infinitely scaled. WPF also includes a flexible hosting model, which makes it straightforward to host a video in a button, for example. The visual designer provided in Visual Studio makes it easy to build WPF application, with drag-in-drop and/or direct editing of XAML markup.

See the [WPF Roadmap](roadmap.md) to learn about project priorities, status and ship dates.

[WinForms](https://github.com/dotnet/winforms) is another UI framework for building Windows desktop applications that is supported on .NET. WPF and WinForms applications only run on Windows. They are part of the `Microsoft.NET.Sdk.WindowsDesktop` SDK. You are recommended to use the most recent version of [Visual Studio](https://visualstudio.microsoft.com/downloads/) to develop WPF and WinForms applications for .NET.  

WPF for ARM64 is new for .NET 6.0 and is supported by NET 6.0 and later. 

To build the WPF repo and contribute features and fixes for .NET 6.0, the most recent [Visual Studio Preview](https://visualstudio.microsoft.com/vs/preview/) is required.  

## Getting started

* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [.NET Preview SDKs (8.0 Preview)](https://github.com/dotnet/installer)
* [Getting started instructions](Documentation/getting-started.md)
* [Contributing guide](Documentation/contributing.md)
* [Migrating .NET Framework WPF Apps to .NET Core](https://docs.microsoft.com/en-us/dotnet/desktop-wpf/migration/convert-project-from-net-framework)

## Status

- We are currently developing WPF for .NET 8. 
- We have completed publishing WPF sources. You can learn more about this at https://github.com/dotnet/wpf/issues/2554. 

See the [WPF roadmap](roadmap.md) to learn about the schedule for specific WPF components.

We have published few tests and have limited coverage for PRs at this time as a result. We will add more tests, however, it will be a progressive process. 

The Visual Studio WPF designer is now available as part of Visual Studio 2019 and later. 

## How to Engage, Contribute and Provide Feedback

Some of the best ways to contribute are to try things out, file bugs, join in design conversations, and fix issues.

* This repo defines [contributing guidelines](Documentation/contributing.md) and also follows the more general [.NET Core contributing guide](https://github.com/dotnet/runtime/blob/master/CONTRIBUTING.md).
* If you have a question or have found a bug, [file an issue](https://github.com/dotnet/wpf/issues/new).
* Use [daily builds](Documentation/getting-started.md#installation) if you want to contribute and stay up to date with the team.

### .NET Framework issues

Issues with .NET Framework, including WPF, should be filed on [VS developer community](https://developercommunity.visualstudio.com/spaces/61/index.html), 
or [Product Support](https://support.microsoft.com/en-us/contactus?ws=support).
They should not be filed on this repo.

## Relationship to .NET Framework

This code base is a fork of the WPF code in the .NET Framework. .NET Core 3.0 was released with a goal of WPF having parity with the .NET Framework version. Over time, the two implementations may diverge.

The [Update on .NET Core 3.0 and .NET Framework 4.8](https://blogs.msdn.microsoft.com/dotnet/2018/10/04/update-on-net-core-3-0-and-net-framework-4-8/) provides a good description of the forward-looking differences between .NET Core and .NET Framework.

This [update](https://devblogs.microsoft.com/dotnet/net-core-is-the-future-of-net/) states how going forward .NET Core is the future of .NET. and .NET Framework 4.8 will be the last major version of .NET Framework.


## Code of Conduct

This project uses the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct) to define expected conduct in our community. Instances of abusive, harassing, or otherwise unacceptable behavior may be reported by contacting a project maintainer at conduct@dotnetfoundation.org.

## Running tests locally

In order to run the tests on your local machine,

- Build the tests with `build.cmd` script. Use `/help` parameter to check the different arguments that can be passed along with the build command.
- cd into `$(RepoRoot)\publish\test\$(Configuration)\$(Platform)\Test` and run `RunDrts.cmd` to run the tests. You can use `/Area` and `/Name` parameters to run tests from a specific area or with a certain name.

At the end of the run, you should see something like this:

```
  A total of 98 test Infos were processed, with the following results.
   Passed: 98
   Failed (need to analyze): 0
   Failed (with BugIDs): 0
   Ignore: 0

```

If there were any failures, run the tests manually with the `/debugtests` flag using the `RunDrts.cmd` script. Note that you do not run the `RunDrtsDebug` script, as this will debug the test infrastructure, `QualityVault`. When you pass the `/debugtests` flag, a cmd window will open where you can open the test executable in Visual Studio and debug it. When the cmd pops up, you will see instructions for debugging using a few different commands, however these commands will enable you to debug the `Simple Test Invocation` executable, `sti.exe`, which simply launches the test executable you are most likely interested in debugging. Using `DrtXaml.exe` as an example, this is how you can debug the test executable. Any MSBuild style properties should be replaced with actual values:

1. `$(RepoRoot)\artifacts\test\$(Configuration)\$(Platform)\Test\RunDrts.cmd /name=DrtXaml /debugtests`
2. Enter following command into the cmd window that pops up:
`"%ProgramFiles%\Microsoft Visual Studio\2022\Preview\Common7\IDE\devenv.exe" DrtXaml.exe`
3. Once Visual Studio is open, go to `Debug-> DrtXaml Properties` and do the following:
    - Manually change the `Debugger Type` from `Auto` to `Mixed (CoreCLR)`.
    - Change the `Environment` from `Default` to a custom one that properly defines the `DOTNET_ROOT` variable so that the host is able to locate the install of `Microsoft.NETCore.App`.
      - x86 (Default): Name: `DOTNET_ROOT(x86)` Value: `$(RepoRoot).dotnet\x86`
      - x64 (/p:Platform=x64): Name: `DOTNET_ROOT` Value: `$(RepoRoot).dotnet` 
4. From there you can F5 and the test will execute.

*NOTE: Some tests require the screen resolution to be set to 1920 x 1080.*

*NOTE: This requires being run from an admin window at the moment.*

## Reporting security issues and security bugs

Security issues and bugs should be reported privately, via email, to the Microsoft Security Response Center (MSRC) <secure@microsoft.com>. You should receive a response within 24 hours. If for some reason you do not, please follow up via email to ensure we received your original message. Further information, including the MSRC PGP key, can be found in the [Security TechCenter](https://www.microsoft.com/msrc/faqs-report-an-issue).

Also see info about related [Microsoft .NET Core and ASP.NET Core Bug Bounty Program](https://www.microsoft.com/msrc/bounty-dot-net-core).

## License

.NET (including the WPF repo) is licensed under the [MIT license](LICENSE.TXT).

## .NET Foundation

.NET WPF is a [.NET Foundation](https://www.dotnetfoundation.org/projects) project.

See the [.NET home repo](https://github.com/Microsoft/dotnet) to find other .NET-related projects.

# Contributing

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.


