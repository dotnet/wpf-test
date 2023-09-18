@echo off

@set FXCTOOL="C:\Program Files\Microsoft DirectX SDK (March 2009)\Utilities\bin\x86\fxc.exe"
@set BYTECODEDIR=.

call :Compile solid

goto Success

:Compile 
        %FXCTOOL% /nologo /T ps_2_0 /E PS /Fo%BYTECODEDIR%\%1.ps %1.fx
        exit/b 0

:Success
        echo Completed.
        pause
        exit/b 0
