# Windows Presentation Foundation (WPF) Test

[![Build Status](https://dnceng-public.visualstudio.com/public/_apis/build/status/dotnet/wpf/dotnet.wpf-test%20CI)](https://dnceng-public.visualstudio.com/public/_build?definitionId=81)
[![MIT License](https://img.shields.io/badge/license-MIT-green.svg)](https://github.com/dotnet/wpf/blob/master/LICENSE.TXT)

This repository contains the source code of the tests for WPF UI Framework. It has the test infrastructure and different suites of tests ( DRTs, Microsuites and Feature Tests) for testing different areas of WPF. 

This repo is currently under the process of being open-sourced. We are tracking the progress here [Test Repository Migration](https://github.com/orgs/dotnet/projects/145)


## Getting started

* [Developer Guide](docs/developer-guide.md)
* [Building, Debugging and Testing](docs/building-running-debugging.md)
* [Test Infrastructure Notes](docs/test-infrastructure.md)
* [Test Suites and Test Categorization](docs/tests-suites.md)
* [New Test Addition Guideline](docs/test-addition-guidelines.md)


## Quickstart

In order to run the tests on your local machine,

- Build the tests with `build.cmd` script. Use `/help` parameter to check the different arguments that can be passed along with the build command.
- cd into `$(RepoRoot)\publish\test\$(Configuration)\$(Platform)\Test` and run `RunDrts.cmd` to run the DRT tests.

At the end of the run, you should see something like this:

```
  A total of 84 test Infos were processed, with the following results.
   Passed: 84
   Failed (need to analyze): 0
   Failed (with BugIDs): 0
   Ignore: 0

```

Once the tests run, the results are generated here `C:\Users\$(CurrentUser)\AppData\Roaming\QualityVault\Run\Report`. You can see the result of the run in `testResults.xml` file.

## Contributing

Some of the best ways to contribute are to try things out, file bugs and fix issues. Since not all the tests have been ported yet, we maintain an internal repo of test code along with this repo. Both core team members and external contributors shall **send pull requests here**, which will be reviewed and then backported to internal repo as well.

In order to start contributing directly to the code base, checkout the [contributing guide](docs/getting-started.md)


## Code of Conduct

This project uses the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct) to define expected conduct in our community. Instances of abusive, harassing, or otherwise unacceptable behavior may be reported by contacting a project maintainer at conduct@dotnetfoundation.org.

## License

.NET (including the WPF Test repo) is licensed under the [MIT license](LICENSE.TXT).

## .NET Foundation

.NET WPF Test is a [.NET Foundation](https://www.dotnetfoundation.org/projects) project.

See the [.NET home repo](https://github.com/Microsoft/dotnet) to find other .NET-related projects.

# Contributing

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.


