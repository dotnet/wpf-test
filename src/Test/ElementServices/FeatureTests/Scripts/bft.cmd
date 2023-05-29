@ECHO OFF
SETLOCAL

@rem /q option performs a quick build, equivalent to "bz";

if /i "%1"=="/q" (
   set CORETESTS_BUILDFLAGS=/Za /M 4
) ELSE (
   set CORETESTS_BUILDFLAGS=/cZa /M 4
)

pushd %cd%

set ERRFILE=build%BUILD_ALT_DIR%.err

@ECHO ON

@echo.
@echo.
@echo **** Build Common\TestRuntime ****
@echo.
@cd /d %_NTDRIVE%%_NTROOT%\wpf\test\common\TestRuntime
call build %CORETESTS_BUILDFLAGS%

@ECHO OFF
if exist %cd%\%ERRFILE%  (
    echo *
    echo ******* Error Compiling TestRuntime *******
    echo Error log: %cd%\%ERRFILE%
    goto :cleanup
)
@ECHO ON

@echo.
@echo.
@echo **** Build Common\Drivers ****
@echo.
@cd /d %_NTDRIVE%%_NTROOT%\wpf\test\common\Drivers
call build %CORETESTS_BUILDFLAGS%

@ECHO OFF
if exist %cd%\%ERRFILE%  (
    echo *
    echo ******* Error Compiling Drivers *******
    echo Error log: %cd%\%ERRFILE%
    goto :cleanup
)
@ECHO ON

@echo.
@echo.
@echo **** Build core test code ****
@echo.
@cd /d %_NTDRIVE%%_NTROOT%\wpf\test\ElementServices\FeatureTests
call build %CORETESTS_BUILDFLAGS%

@ECHO OFF
if exist %cd%\%ERRFILE% (
    echo *
    echo ******* Error Building Core Test Code *******
    echo Error log: %cd%\%ERRFILE%
    goto :cleanup
)
@ECHO ON

@ECHO OFF
if NOT %ERRORLEVEL% EQU 0 (
    echo *
    echo ******* Error Occurred *******
    goto :cleanup
)

:cleanup
@ECHO OFF
echo.
popd

ENDLOCAL

