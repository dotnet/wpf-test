@echo off
echo starting tests...
Call %~dp0\QV.cmd Run /DiscoveryInfoPath=%~dp0\DiscoveryInfo.xml /RunDirectory="%APPDATA%\QualityVault\Run" %*
echo done with tests
if EXIST "%APPDATA%\QualityVault\Run\Report" (
    echo RunTests.cmd command executed
    taskkill /F /IM QualityVaultFrontEnd.exe
    echo "Any remaining QVFrontEnd processes killed."
)