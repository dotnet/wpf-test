@if %_echo%.==. echo off

setlocal

REM --------- Prepare 
	REM ---------- Instrument the PresentationFramework.dll
	bbcover /i PresentationFramework.dll /o PresentationFramework.dll.instrumented /ProbeOpt
	
	REM ---------- Save original dll and replace it by instrumented one
	copy /Y PresentationFramework.dll PresentationFramework.dll.original
	copy /Y PresentationFramework.dll.instrumented PresentationFramework.dll

	REM ---------- Start covermon service
	net start covermon
	REM ---------- Clean data cash
	covercmd /reset	

REM --------- Run test suites

	DrtEditing.exe

REM --------- Analize and clean ----------
	REM ---------- Save coverage data (PresentationFramework.dll\*.covdata)
	covercmd /save
	REM ---------- Stop covermon service
	net stop covermon
	
	REM ---------- Restore original dll
	copy /Y PresentationFramework.dll.original PresentationFramework.dll

	REM ---------- Reprt coverage data (PresentationFramework.dll.cov.mixed.xml)
	bbcovrpt /i PresentationFramework.dll /devreport

	REM --------- Run Coverage test
	CoverageFilter PresentationFramework.dll.cov.mixed.xml -oc:PresentationFramework.dll.cov.mixed.filtered.xml -x:System.Xml.Serialization -p:10 -b:%_NTBINDIR%\windows\wcp\devtest\drts\Editing\DrtCoverage\DrtEditingCoverage.bsl -ob:%_NTBINDIR%\windows\wcp\devtest\drts\Editing\DrtCoverage\DrtEditingCoverage.out -f:@%_NTBINDIR%\windows\wcp\devtest\drts\Editing\DrtCoverage\DrtEditingSources.txt

	REM --------- Check Success ----------
	fc %_NTBINDIR%\windows\wcp\devtest\drts\Editing\DrtCoverage\DrtEditingCoverage.bsl %_NTBINDIR%\windows\wcp\devtest\drts\Editing\DrtCoverage\DrtEditingCoverage.out >nul
	if errorlevel 1 (
		echo ATTENTION: Coverage results has been changed
		echo Use the following command to inspect current coverage results:
		echo 	sleuthdte.hta %_NTBINDIR%\windows\wcp\DevTest\objchk\i386\PresentationFramework.dll.cov.mixed.filtered.xml
		windiff %_NTBINDIR%\windows\wcp\devtest\drts\Editing\DrtCoverage\DrtEditingCoverage.bsl %_NTBINDIR%\windows\wcp\devtest\drts\Editing\DrtCoverage\DrtEditingCoverage.out
	)

:end
endlocal
