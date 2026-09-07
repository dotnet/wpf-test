@echo off
rem Same as eng\common\cibuild.cmd, except that -publish is not passed unconditionally.
rem This repo produces no packages or blobs, and since Arcade 11 the Publish.proj
rem 'Execute' target always invokes PushToBuildStorage, which fails with
rem "ItemsToPush is not specified." when there is nothing to publish.
rem Internal builds pass -publish explicitly through $(_PublishArgs).
powershell -ExecutionPolicy ByPass -NoProfile -command "& """%~dp0..\common\Build.ps1""" -restore -build -test -sign -pack -ci %*"
