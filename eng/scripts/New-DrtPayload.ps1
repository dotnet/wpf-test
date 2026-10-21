<#
.SYNOPSIS
Copies the files needed to run the DRTs from the published test folder into a small payload.

.DESCRIPTION
The full publish\<config>\<platform>\Test folder is about 1.8 GB because it includes the feature
tests. The DRTs only need DRT\, Infra\ and Common\, plus the QualityVault scripts. The payload also
gets Invoke-Drts.ps1, so a test job only needs the payload and .NET.
See docs/drts-on-ci-implementation.md.
#>
[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [ValidateSet('x86', 'x64')]
    [string]$Platform = 'x64',

    [Parameter(Mandatory = $true)]
    [string]$OutputDirectory
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..\..')
$source = Join-Path $repoRoot "publish\$Configuration\$Platform\Test"
if (-not (Test-Path (Join-Path $source 'DRT\DrtList.xml'))) {
    throw "No DRTs found under $source. Build with -configuration $Configuration /p:Platform=$Platform first."
}

if (Test-Path $OutputDirectory) {
    Remove-Item $OutputDirectory -Recurse -Force
}
New-Item -ItemType Directory $OutputDirectory | Out-Null

foreach ($folder in 'DRT', 'Infra', 'Common', 'xsltTranform') {
    Copy-Item (Join-Path $source $folder) $OutputDirectory -Recurse
}
Get-ChildItem $source -File | Where-Object { $_.Extension -in '.cmd', '.json' -or $_.Name -eq 'DiscoveryInfoDrts.xml' } |
    Copy-Item -Destination $OutputDirectory
Copy-Item (Join-Path $PSScriptRoot 'Invoke-Drts.ps1') $OutputDirectory

$size = (Get-ChildItem $OutputDirectory -Recurse -File | Measure-Object Length -Sum).Sum / 1MB
Write-Host ("DRT payload: {0} ({1:N0} MB)" -f (Resolve-Path $OutputDirectory), $size)
