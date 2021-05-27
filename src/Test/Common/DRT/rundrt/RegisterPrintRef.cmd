@if "%_echo%"=="" echo off
setlocal

@echo RegisterPrintRef.cmd: Configures printing binaries (msxml6.dll) [Current directory = %cd%]

set ARG=%1

if "%ARG%" == "/checkdrt" goto checkconfigdrt

REM Invalid command line.
echo Unrecognized command line argument '%1' while trying to install msxml6
echo    /checkdrt - Checks if msxml6 is set up properly
exit /b 1


:checkconfigdrt

@echo Checking to be sure msxml6 is set up properly

set _XmlDocVersionRegPath=CLSID\{2933BF90-7B36-11d2-B20E-00C04F983E60}\VersionList
reg query HKEY_CLASSES_ROOT\%_XmlDocVersionRegPath% | findstr /C:msxml6.dll
if ERRORLEVEL 1 echo %~n0: FAILED - msxml6.dll not registered properly, run wpfSetup.exe to install DRT prerequisites & exit /b 1

echo RegisterPrintRef.cmd: SUCCESS (check config)
exit /b 0

