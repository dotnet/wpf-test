<#
.SYNOPSIS
Runs the DRTs with QualityVault and returns a non-zero exit code when any DRT fails.

.DESCRIPTION
Used by eng/pipelines/templates/drt-stages.yml and locally. Run it elevated: QualityVault copies
itself into %ProgramFiles%. See docs/drts-on-ci-implementation.md.

Exit codes: 0 when every DRT that ran passed, 1 when a DRT failed or none ran, 2 when QualityVault
produced no report.

.EXAMPLE
eng\scripts\Invoke-Drts.ps1 -TestRoot publish\Release\x64\Test -Area Controls -DotNetRoot .dotnet
#>
[CmdletBinding()]
param(
    # Folder that contains QV.cmd, DiscoveryInfoDrts.xml, DRT\, Infra\ and Common\.
    # Defaults to this script's folder, which is where New-DrtPayload.ps1 puts it.
    [string]$TestRoot,

    # QualityVault run directory. The report is written to <RunDirectory>\Report.
    [string]$RunDirectory = (Join-Path $env:TEMP 'QualityVault\Run'),

    # .NET installation to run the DRTs on. When empty, the current environment is used.
    [string]$DotNetRoot,

    # QualityVault filters. Blank values are ignored.
    [string]$Area,
    [string]$Name,
    [string]$Keywords,
    [string]$Priority,

    # Rerun each failing DRT once.
    [switch]$RerunFailures,

    # Run only the quarantined DRTs (Disabled="true" in DrtList.xml).
    [switch]$Disabled
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

# $PSScriptRoot is not set in parameter defaults on Windows PowerShell 5.1.
if (-not $TestRoot) { $TestRoot = $PSScriptRoot }
$TestRoot = (Resolve-Path $TestRoot).Path
$qv = Join-Path $TestRoot 'QV.cmd'
$discoveryInfo = Join-Path $TestRoot 'DiscoveryInfoDrts.xml'
foreach ($path in $qv, $discoveryInfo) {
    if (-not (Test-Path $path)) { throw "Not found: $path. Is -TestRoot a DRT payload or publish\<config>\<platform>\Test folder?" }
}

if ($DotNetRoot) {
    $env:DOTNET_ROOT = (Resolve-Path $DotNetRoot).Path
    $env:DOTNET_MULTILEVEL_LOOKUP = '0'
}
Write-Host "DOTNET_ROOT=$env:DOTNET_ROOT"

# Server Manager opens at logon on Windows Server build agents and can take focus from UI tests.
$serverManager = Get-Process ServerManager -ErrorAction SilentlyContinue
if ($serverManager) {
    Write-Host 'Closing Server Manager.'
    $serverManager | Stop-Process -Force
    Start-Sleep -Seconds 2
}

if (Test-Path $RunDirectory) {
    Remove-Item $RunDirectory -Recurse -Force
}

$qvArgs = @('Run', "/DiscoveryInfoPath=$discoveryInfo", "/RunDirectory=$RunDirectory")
$filters = [ordered]@{ Area = $Area; Name = $Name; Keywords = $Keywords; Priority = $Priority }
foreach ($filter in $filters.GetEnumerator()) {
    if ($filter.Value -and $filter.Value.Trim()) {
        $qvArgs += "/$($filter.Key)=$($filter.Value.Trim())"
    }
}
if ($RerunFailures) { $qvArgs += '/RerunFailures' }
if ($Disabled) { $qvArgs += '/Disabled=true' }

Write-Host "QV.cmd $($qvArgs -join ' ')"
# DrtAdaptor resolves DRT\ relative to the current directory.
Push-Location $TestRoot
try {
    & $qv @qvArgs
    Write-Host "QualityVault exit code: $LASTEXITCODE (does not reflect test results)"
}
finally {
    Pop-Location
}

$report = Join-Path $RunDirectory 'Report'
$xunit = Join-Path $report 'testResults.xml'
if (-not (Test-Path $xunit)) {
    Write-Host "##vso[task.logissue type=error]QualityVault produced no report at $xunit."
    exit 2
}

$tests = @(([xml](Get-Content $xunit -Raw)).SelectNodes('//test'))
$failed = @($tests | Where-Object { $_.result -ne 'Pass' -and $_.result -ne 'Skip' })
Write-Host ''
Write-Host "DRTs run: $($tests.Count), failed: $($failed.Count). Report: $report"
foreach ($test in $failed) {
    Write-Host "##vso[task.logissue type=error]$($test.name) failed ($($test.result))."
}

if ($tests.Count -eq 0) {
    Write-Host '##vso[task.logissue type=error]No DRTs ran. Check the filters.'
    exit 1
}
if ($failed.Count -gt 0) { exit 1 }
exit 0
