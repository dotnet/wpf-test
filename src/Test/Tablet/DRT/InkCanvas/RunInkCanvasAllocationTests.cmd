@echo off
setlocal

set RESULTPATH=C:\
set PREFIX=

if NOT "%1%"=="" (
    set RESULTPATH=%1%
)

if NOT "%2%"=="" (
    set RESULTPATH=%RESULTPATH%\%2%
)

@echo on

ECHO WARMING UP...
@echo off
DrtInkCanvas.exe /Suite InkCanvasAPITests
DrtInkCanvas.exe /Suite InkCanvasEditingTests
DrtInkCanvas.exe /Suite InkCanvasDataBindingTests
@echo on

ECHO RUNNING InkCollectionPerfTest...
call clrprofiler -o %RESULTPATH%InkCollectionPerfTest.log -p DrtInkCanvas.exe /suite InkCollectionPerfTest

ECHO RUNNING SelectionAPIPerfTest...
call clrprofiler -o %RESULTPATH%SelectionAPIPerfTest.log -p DrtInkCanvas.exe /suite SelectionAPIPerfTest

ECHO RUNNING SelectionMovingPerfTest...
call clrprofiler -o %RESULTPATH%SelectionMovingPerfTest.log -p DrtInkCanvas.exe /suite SelectionMovingPerfTest

ECHO RUNNING LassoSelectionPerfTest...
call clrprofiler -o %RESULTPATH%LassoSelectionPerfTest.log -p DrtInkCanvas.exe /suite LassoSelectionPerfTest

ECHO RUNNING PointErasePerfTest...
call clrprofiler -o %RESULTPATH%PointErasePerfTest.log -p DrtInkCanvas.exe /suite PointErasePerfTest

ECHO RUNNING StrokeErasePerfTest...
call clrprofiler -o %RESULTPATH%StrokeErasePerfTest.log -p DrtInkCanvas.exe /suite StrokeErasePerfTest
