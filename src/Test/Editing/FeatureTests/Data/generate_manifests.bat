@echo off

rem Use this file to regenerate the security manifests
rem required by binaries.

rem This script requires that the client be enlisted in both
rem windowstest and windowstestdata, and is not suitable for
rem running in a distributed building lab.

setlocal

if not exist ManifestGen.exe (
  echo Please copy ManifestGen.exe to the local directory.
  echo Eg
  echo copy \\corum\public\manifestgen2.exe .
  goto :eof
)

set GEN_TOOL=ManifestGen2.exe

if "%_BuildArch%" == "x86" (
  set OBJ_DIR=obj\i386
) else (
  echo Unknown architecture %_BuildArch%
  goto :eof
)

if not exist %sdxroot%\testsrc\windowstest\client\wcptests\uis\Text\TestCases\ExeTarget\%OBJ_DIR%\WTC.Uis.TextTests.exe (
  echo The following file does not exist. Try rebuilding the tests.
  echo %sdxroot%\testsrc\windowstest\client\wcptests\uis\Text\TestCases\ExeTarget\%OBJ_DIR%\WTC.Uis.TextTests.exe
  goto :eof
)

%GEN_TOOL% Text\BVT\ExeTarget\see.manifest ^
  %sdxroot%\testsrc\windowstest\client\wcptests\uis\Text\BVT\ExeTarget\%OBJ_DIR%\WTC.Uis.TextBvt.exe ^
  %sdxroot%\testsrc\windowstest\client\wcptests\uis\Common\Library\%OBJ_DIR%\WTC.Uis.TestLib.dll
  
%GEN_TOOL% Text\TestCases\ExeTarget\see.manifest ^
  %sdxroot%\testsrc\windowstest\client\wcptests\uis\Text\TestCases\ExeTarget\%OBJ_DIR%\WTC.Uis.TextTests.exe ^
  %sdxroot%\testsrc\windowstest\client\wcptests\uis\Common\Library\%OBJ_DIR%\WTC.Uis.TestLib.dll

rem The following lines add the ControlDomainPolicy permission, to
rem work around what looks like a CLR bug.
rem rep -find:"\"Execution\"" -replace:"\"Execution,ControlDomainPolicy\"" ^
rem   Text\TestCases\ExeTarget\see.manifest ^
rem   Text\BVT\ExeTarget\see.manifest

endlocal
