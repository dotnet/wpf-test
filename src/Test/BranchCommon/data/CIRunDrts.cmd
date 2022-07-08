@echo off
Call %~dp0\QV.cmd Run /DiscoveryInfoPath=%~dp0\DiscoveryInfoDrts.xml "/RunDirectory=%APPDATA%\QualityVault\Run" %*

if EXIST "%APPDATA%\QualityVault\Run\Report\DrtReport.xml" (
  echo To view DRT Report, run "DrtReport"
  echo start /B /WAIT "%ProgramFiles%\Internet Explorer\iexplore.exe" "%APPDATA%\QualityVault\Run\Report\DrtReport.xml" > DrtReport.cmd
  findstr /C:"Variations PassRate=\"100.00%%\"" "%APPDATA%\QualityVault\Run\Report\DrtReport.xml" > NUL
  if errorlevel 1 echo "Some Tests Failed" else echo "All Tests Passed"
  exit /b 0
)
