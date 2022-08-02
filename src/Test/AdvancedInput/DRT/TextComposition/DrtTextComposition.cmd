@echo off

echo register DrtTextService
regsvr32 -s DrtTextService.dll

echo run DrtTextComposition.exe
DrtTextComposition.exe -verbose > DRTTextComposition.log
if %errorlevel% NEQ 0 goto ErrorExit
echo unregister DrtTextService
regsvr32 -s -u DrtTextService.dll

echo Save log to track missing char investigation even though it is not failed.
echo DrtTextComposition now has the workaround.
if not "%DRT_LOGS%" == "" copy DRTTextComposition.log %DRT_LOGS%\DRTTextComposition.log
exit /b 0

:ErrorExit
echo unregister DrtTextService
regsvr32 -s -u DrtTextService.dll
if not "%DRT_LOGS%" == "" copy DRTTextComposition.log %DRT_LOGS%\DRTTextComposition.log
exit /b 1
