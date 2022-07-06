copy xsltTranform xsltTranform.ps1 /ZQ
powershell -ExecutionPolicy ByPass -NoProfile -command "& """%~dp0xsltTranform.ps1""" -xmlDir %APPDATA%\QualityVault\Run\Report -htmlDir %APPDATA%\QualityVault\Run\HtmlReport"
del xsltTranform.ps1 /FQ

echo HTMLReports at %APPDATA%\QualityVault\Run\HtmlReport
exit /b %ErrorLevel%
