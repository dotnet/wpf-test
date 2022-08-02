@echo off
setlocal

set loop=5

if NOT "%1%"=="" (
    set loop=%1%
)
@echo on

ECHO WARMING UP...
@echo off
DrtInkCanvas.exe /Suite InkCanvasAPITests
DrtInkCanvas.exe /Suite InkCanvasEditingTests
DrtInkCanvas.exe /Suite InkCanvasDataBindingTests
@echo on

ECHO RUNNING InkCollectionPerfTest...
Call rundrts.exe /loop %loop% /Drt "DrtInkCanvas.exe /suite InkCollectionPerfTest /Verbose"

ECHO RUNNING SelectionAPIPerfTest...
Call rundrts.exe /loop %loop% /Drt "DrtInkCanvas.exe /suite SelectionAPIPerfTest /Verbose"

ECHO RUNNING SelectionMovingPerfTest...
Call rundrts.exe /loop %loop% /Drt "DrtInkCanvas.exe /suite SelectionMovingPerfTest /Verbose"

ECHO RUNNING LassoSelectionPerfTest...
Call rundrts.exe /loop %loop% /Drt "DrtInkCanvas.exe /suite LassoSelectionPerfTest /Verbose"

ECHO RUNNING PointErasePerfTest...
Call rundrts.exe /loop %loop% /Drt "DrtInkCanvas.exe /suite PointErasePerfTest /Verbose"

ECHO RUNNING StrokeErasePerfTest...
Call rundrts.exe /loop %loop% /Drt "DrtInkCanvas.exe /suite StrokeErasePerfTest /Verbose"
