@echo off
rem MSBuild multi-threaded mode (-mt), enabled by default in Arcade for local builds, causes WPF
rem markup compilation (GenerateTemporaryTargetAssembly) to hang. Disable it until the issue is fixed.
powershell -ExecutionPolicy ByPass -NoProfile -command "& """%~dp0eng\common\Build.ps1""" -restore -build -msbuildMultiThreaded 0 %*"
