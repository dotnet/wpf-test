@echo off

rem The following lists files to be copied and can be invoked
rem with build_docs.bat -ListFiles

if "%1" == "-ListFiles" (
  echo PresentationFramework.dll
  echo PresentationCore.dll
  echo WindowsBase.dll
  echo UIAutomationClient.dll
  echo UIAutomationTypes.dll
  goto :eof
)

if "%1" == "-ListTools" (
  echo AutomationFramework.dll
  echo AutoData.dll
  goto :eof
)

setlocal

set CMD_OBJ_DIR=obj%BUILD_ALT_DIR%\%build.arch%

echo Using object dir CMD_OBJ_DIR:   %CMD_OBJ_DIR%
echo Expecting something similar to: objchk\i386

set DEPLOY_DIR=%_NTTREE%\nttest\windowstest\client\tools\utils\CSDocFormatter
set DOC_FORMATTER_TOOL=%DEPLOY_DIR%\CSDocFormatterCmd.exe
set TRANSFORM_TOOL=%DEPLOY_DIR%\CSXmlTransform.exe
set SPLIT_TOOL=%DEPLOY_DIR%\CSFileSplit.exe
set BIN_DIR=%SystemDrive%\work\wtc.docs

set TOOLS_FOUND=1

echo Note that this needs to run in a Razzle console.
echo Verifying that all tools have been built and are available...

if exist %DOC_FORMATTER_TOOL% (
  echo Documentation formatter tool found: %DOC_FORMATTER_TOOL%
) else (
  echo The documentation formatter tool was not found at this location.
  echo.
  echo %DOC_FORMATTER_TOOL%
  set TOOLS_FOUND=0
)

if exist %TRANSFORM_TOOL% (
  echo Transform tool found: %TRANSFORM_TOOL%
) else (
  echo The transformation tool was not found at this location.
  echo.
  echo %TRANSFORM_TOOL%
  set TOOLS_FOUND=0
)

if exist %SPLIT_TOOL% (
  echo XML splitting tool found: %SPLIT_TOOL%
) else (
  echo The splitting tool was not found at this location.
  echo.
  echo %SPLIT_TOOL%
  set TOOLS_FOUND=0
)

if %TOOLS_FOUND% == 0 (
  echo.
  echo Please build from the following directory to get all tools.
  echo %sdxroot%\testsrc\windowstest\client\tools\utils\CSDocFormatter
  goto :eof
)

if exist %BIN_DIR% (
  echo Removing %BIN_DIR%...
  rmdir /q /s %BIN_DIR%
)

echo Creating %BIN_DIR%...
mkdir %BIN_DIR%

rem Until we get a real deployment, copy required files locally.
for /F "usebackq" %%i in (`build_docs.bat -ListFiles`) do (
  if not exist %_NTTREE%\%%i (
    echo WCP files currently need to be copied to the following directory.
    echo %BIN_DIR%
    echo.
    echo File not found: %_NTTREE%\%%i
    echo.
    if exist %sdxroot%\work\%%i (
      echo Note that you have a copy in %sdxroot%\work\%%i
      echo.
    )
    goto :eof
  )
)
for /F "usebackq" %%i in (`build_docs.bat -ListFiles`) do (
  xcopy %_NTTREE%\%%i %BIN_DIR%
)

rem Until we get a real deployment, copy required files locally.
for /F "usebackq" %%i in (`build_docs.bat -ListTools`) do (
  if not exist %_NTTREE%\NTTest\WindowsTest\Client\Tools\%%i (
    echo WCP files currently need to be copied to the following directory.
    echo %BIN_DIR%
    echo.
    echo File not found: %_NTTREE%\NTTest\WindowsTest\Client\Tools\%%i
    echo.
    if exist %sdxroot%\work\%%i (
      echo Note that you have a copy in %sdxroot%\work\%%i
      echo.
    )
    goto :eof
  )
)

for /F "usebackq" %%i in (`build_docs.bat -ListTools`) do (
  xcopy %_NTTREE%\NTTest\WindowsTest\Client\Tools\%%i %BIN_DIR%
)

echo Copying test library and documentation files...
xcopy %_NTTREE%\NTTest\WindowsTest\Client\WcpTests\Uis\Common\EditingTestLib.dll %BIN_DIR%
xcopy %OBJECT_ROOT%\testsrc\windowstest\client\wcptests\uis\common\library\%CMD_OBJ_DIR%\EditingTestLib.xml %BIN_DIR%

echo Copying ClientTestLibrary...
xcopy %_NTTREE%\NTTest\WindowsTest\Client\WcpTests\Common\ClientTestLibrary.dll %BIN_DIR%

rem set SKIP_DOCS=1
rem set SKIP_FORMATTING=1

cd /d %sdxroot%\testsrc\windowstest\client\wcptests\uis\Common\Library\docs

if exist reflected.xml (
  if not defined SKIP_FORMATTING (
    del reflected.xml
  )    
)

echo Building documentation...
if not defined SKIP_DOCS (
%DOC_FORMATTER_TOOL% -doc:%BIN_DIR%\EditingTestLib.xml ^
  -assembly:%BIN_DIR%\EditingTestLib.dll -out:reflected.xml ^
  -includeXslt:off
)

if not exist reflected.xml (
  echo.
  echo Error: %CD%\reflected.xml was not created.
  echo.
  goto :cleanup
)

if not defined SKIP_FORMATTING (
  if exist files.xml (
    del files.xml
  )
  echo Formatting documentation into files...
  %TRANSFORM_TOOL% reflected.xml multifile.xsl files.xml
  if not exist files.xml (
    echo.
    echo Error: %CD%\files.xml was not created.
    echo.
    goto :cleanup
  )
)

echo Splitting documentation into files...
if exist files (
  rmdir /q /s files
) 
mkdir files

pushd files
%SPLIT_TOOL% ..\files.xml
popd

if not exist files\EditingTestLibAssembly.htm (
  echo.
  echo Error: %CD%\files\EditingTestLibAssembly.htm was not created.
  echo.
  goto :cleanup
)

echo Files built at %CD%\files.
echo.
echo Copying files to file share.
if exist \\Microsoft\public (
  echo Deleting existing files...
  del /q \\Microsoft\public\common-lib-docs\*
  
  echo Publishing new files...
  mkdir \\Microsoft\public\common-lib-docs
  xcopy files\* \\Microsoft\public\common-lib-docs
  echo The documentation can be browser on the net with the following command.
  echo \\Microsoft\public\common-lib-docs\EditingTestLibAssembly.htm
) else (
  echo Share \\Microsoft\public not found. Files not published.
)

echo The documentation can be browsed with the following command.
echo start files\EditingTestLibAssembly.htm

start files\EditingTestLibAssembly.htm

:cleanup
if exist %BIN_DIR% (
  echo Deleting directory with work binaries: %BIN_DIR%
  rmdir /q /s %BIN_DIR%
)

endlocal
