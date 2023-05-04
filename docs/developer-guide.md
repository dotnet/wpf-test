# Developer Guide

The following document describes the setup and workflow that is recommended for working on the wpf-test project. It assumes that you have read the [contributing guide](contributing.md).

## Machine Setup

WPF Test requires the following workloads and components be selected when installing Visual Studio 2022 (17.0.0):

* Required Workloads: [wpf-test.vsconfig](wpf-test.vsconfig)
    *  Also see [Import or export installation configurations](https://docs.microsoft.com/en-us/visualstudio/install/import-export-installation-configurations?view=vs-2019)

PS : *Currently, this file is a copy of the wpf.vsconfig file in WPF repo. Hence, anyone that has setup there machines for building WPF from source will also be able to build this repo. However, in future there can be some changes to this file.*

## Workflow

We use the following workflow for building tests and running them against WPF assemblies ( both SDK and locally built ).

You first need to [fork][fork] then [clone][clone] this Windows Forms repository. This is a one-time task. This is a one-time task.

1. [Build](building-running-debugging.md#building-tests) the repository.
2. [Run](building-running-debugging.md#running-tests) the tests.
3. [Debug](building-running-debugging.md#debugging-tests-and-test-infrastructure) the tests.

## More Information

--