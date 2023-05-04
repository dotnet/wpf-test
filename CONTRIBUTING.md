# Contribution to WPF Test

You can contribute to WPF Test with issues and PRs. Simply filing issues for problems you encounter is a great way to contribute. Contributing implementations is greatly appreciated.

## Reporting Issues

We always welcome bug reports and overall feedback. Here are a few tips on how you can make reporting your issue as effective as possible.

Test failures can mean either of two things: 
- There is a bug in WPF product itself.
- Or, the tests are failing due to test infra issues ( tests may not have been updated to reflect new behaviour in WPF, test infra is failing, updates in drivers and dependencies).

For any such case, please [file an issue]() in this repository. Depending on the nature of failure, the issue may be transferred to dotnet/wpf repo.

### Finding Existing Issues

Before filing a new issue, please search our [open issues](https://github.com/dotnet/wpf-test/issues) to check if it already exists.

If you do find an existing issue, please include your own feedback in the discussion. Do consider upvoting (👍 reaction) the original post, as this helps us prioritize popular issues in our backlog.

### Writing a Good Bug Report

Good bug reports make it easier for maintainers to verify and root cause the underlying problem. The better a bug report, the faster the problem will be resolved. Ideally, a bug report should contain the following information:

* A high-level description of the problem.
* A _minimal reproduction_, i.e. the smallest size of code/configuration required to reproduce the wrong behavior. Here, we expect the command that is used for running the tests.
* Information on the environment: OS/distro, CPU arch, SDK version, Screen Resolution, Multiple Screen Setup, RDP Connection etc.
* In case of test failures, please add the /Area, /SubArea, /Name of the tests. 

When ready to submit a bug report, please use the [Bug Report issue template]().


## Contributing Changes

Project maintainers will merge changes that improve the product significantly.

The [Pull Request Guide](docs/workflow/pr-guide.md) and [Copyright](docs/project/copyright.md) docs define additional guidance.

### DOs and DON'Ts

Please do:

* **DO** follow our [coding style]() (C# code-specific).
* **DO** give priority to the current style of the project or file you're changing even if it diverges from the general guidelines.
* **DO** keep the discussions focused. When a new or related topic comes up
  it's often better to create new issue than to side track the discussion.
* **DO** clearly state on an issue that you are going to take on implementing it.
* **DO** blog and tweet (or whatever) about your contributions, frequently!

Please do not:

* **DON'T** make PRs for style changes.
* **DON'T** surprise us with big pull requests. Instead, file an issue and start
  a discussion so we can agree on a direction before you invest a large amount
  of time.
* **DON'T** commit code that you didn't write. If you find code that you think is a good fit to add to the WPF Test repo, file an issue and start a discussion before proceeding.
* **DON'T** submit PRs that alter licensing related files or headers. If you believe there's a problem with them, file an issue and we'll be happy to discuss it.

### Suggested Workflow

We use and recommend the following workflow:

1. Create an issue for your work.
    - You can skip this step for trivial changes.
    - Reuse an existing issue on the topic, if there is one.
    - Get agreement from the team and the community that your proposed change ( like addition of new tests, removing some old tests, etc. ) is a good one.
    - Clearly state that you are going to take on fixing an issue, if that's the case. You can request that the issue be assigned to you. Note: The issue filer and the implementer don't have to be the same person.
2. Create a personal fork of the repository on GitHub (if you don't already have one).
3. In your fork, create a branch off of main (`git checkout -b mybranch`).
    - Name the branch so that it clearly communicates your intentions, such as issue-123 or githubhandle-issue.
    - Branches are useful since they isolate your changes from incoming changes from upstream. They also enable you to create multiple PRs from the same fork.
4. Make and commit your changes to your branch.
    - [Building, Running and Debugging Tests](docs/building-running-debugging.md) explains how to build and test.
    - Please follow our [Commit Messages](#commit-messages) guidance.
5. Build the repository with your changes.
    - Make sure that the builds are clean.
    - Make sure that the tests are all passing, including your new tests.
6. Create a pull request (PR) against the dotnet/wpf repository's **main** branch.
    - State in the description what issue or improvement your change is addressing.
    - Check if all the Continuous Integration checks are passing.
7. Wait for feedback or approval of your changes from the team.
    - Details about the pull request [review procedure](docs/pr-guide.md).
8. When team has signed off, and all checks are green, your PR will be merged.

### Help Wanted (Up for Grabs)

The team marks the most straightforward issues as [help wanted](https://github.com/dotnet/wpf-test/labels/help%20wanted). This set of issues is the place to start if you are interested in contributing but new to the codebase.

### Commit Messages

Please format commit messages as follows (based on [A Note About Git Commit Messages](http://tbaggery.com/2008/04/19/a-note-about-git-commit-messages.html)):

```
Summarize change in 50 characters or less

Provide more detail after the first line. Leave one blank line below the
summary and wrap all lines at 72 characters or less.

If the change fixes an issue, leave another blank line after the final
paragraph and indicate which issue is fixed in the specific format
below.

Fix #42
```

Also do your best to factor commits appropriately, not too large with unrelated things in the same commit, and not too small with the same small change applied N times in N different commits.

### Contributor License Agreement

You must sign a [.NET Foundation Contribution License Agreement (CLA)](https://cla.dotnetfoundation.org) before your PR will be merged. This is a one-time requirement for projects in the .NET Foundation. You can read more about [Contribution License Agreements (CLA)](http://en.wikipedia.org/wiki/Contributor_License_Agreement) on Wikipedia.

The agreement: [net-foundation-contribution-license-agreement.pdf](https://github.com/dotnet/home/blob/master/guidance/net-foundation-contribution-license-agreement.pdf)

You don't have to do this up-front. You can simply clone, fork, and submit your pull-request as usual. When your pull-request is created, it is classified by a CLA bot. If the change is trivial (for example, you just fixed a typo), then the PR is labelled with `cla-not-required`. Otherwise it's classified as `cla-required`. Once you signed a CLA, the current and all future pull-requests will be labelled as `cla-signed`.

### File Headers

The following file header is the used for files in this repo. Please use it for new files.

```
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
```

Whenever adding a new file, please ensure that you have added the following header on `.cs` and `.cpp` files.

### PR - CI Process

The [dotnet continuous integration](https://dev.azure.com/dnceng-public/public/_build) (CI) system will automatically perform the required builds and run tests (including the ones you are expected to run) for PRs. Builds and test runs must be clean or have bugs properly filed against flaky/unexpected failures that are unrelated to your change.

If the CI build fails for any reason, the PR issue will link to the `Azure DevOps` build with further information on the failure.

### PR Feedback

Microsoft team and community members will provide feedback on your change. Community feedback is highly valued. You will often see the absence of team feedback if the community has already provided good review feedback.

One or more Microsoft team members will review every PR prior to merge. They will often reply with "LGTM, modulo comments". That means that the PR will be merged once the feedback is resolved. "LGTM" == "looks good to me".

There are lots of thoughts and [approaches]() for how to efficiently discuss changes. It is best to be clear and explicit with your feedback. Please be patient with people who might not understand the finer details about your approach to feedback.
