<#
.SYNOPSIS
Builds the Azure DevOps job matrix for the DRT pipeline: one job per DRT area.

.DESCRIPTION
Reads DrtList.xml and returns one matrix entry per area (the Team attribute) that has at least one
DRT to run on the given platform. Areas whose DRTs are all quarantined (Disabled="true") are left
out, so no job runs zero tests. See docs/drts-on-ci-implementation.md (M2).

In a pipeline, -OutputVariable sets an output variable with the matrix as JSON, for use in
`strategy: matrix: $[ ... ]`. Locally, the matrix is written to the output.

.EXAMPLE
eng\scripts\Get-DrtMatrix.ps1 -Platform x64 -Area 'AppModel,Editing'
#>
[CmdletBinding()]
param(
    [string]$DrtList,

    [ValidateSet('x64', 'x86')]
    [string]$Platform = 'x64',

    # Comma-separated area names, matched exactly (case-insensitive). Blank for all areas.
    [string]$Area,

    # Comma-separated substrings of DRT names, as for QualityVault /Name=. A DRT's name is
    # <executable without extension>(<args>), for example DrtControls(-suite ToolTip).
    # Areas with no matching DRT are left out. Blank for all DRTs.
    [string]$Name,

    # Use the quarantined DRTs instead of the enabled ones.
    [switch]$Disabled,

    # Name of the pipeline output variable to set. When empty, the matrix is only written to the output.
    [string]$OutputVariable
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

if (-not $DrtList) {
    $DrtList = Join-Path $PSScriptRoot '..\..\src\Test\BranchCommon\data\DrtList.xml'
}

function Split-List([string]$value) {
    if (-not $value) { return @() }
    return @($value.Split(',') | ForEach-Object { $_.Trim() } | Where-Object { $_ })
}

$architecture = @{ x64 = 'amd64'; x86 = 'x86' }[$Platform]
$areaFilter = @(Split-List $Area)
$nameFilter = @(Split-List $Name)

$drts = ([xml](Get-Content (Resolve-Path $DrtList).Path -Raw)).SelectNodes('//Drt') | ForEach-Object {
    $exe = $_.GetAttribute('Executable')
    [pscustomobject]@{
        Area         = $_.GetAttribute('Team')
        Name         = [IO.Path]::GetFileNameWithoutExtension($exe) + '(' + $_.GetAttribute('Args') + ')'
        Architecture = $_.GetAttribute('Architecture')
        Disabled     = $_.GetAttribute('Disabled') -eq 'true'
    }
}

$allAreas = @($drts.Area | Sort-Object -Unique)
$unknown = @($areaFilter | Where-Object { $allAreas -notcontains $_ })
if ($unknown) {
    throw "Unknown area(s): $($unknown -join ', '). Areas in $(Split-Path $DrtList -Leaf): $($allAreas -join ', ')."
}

$selected = @($drts | Where-Object {
    $drt = $_
    ($drt.Disabled -eq [bool]$Disabled) -and
    ($drt.Architecture -eq 'all' -or ($drt.Architecture -split '\|') -contains $architecture) -and
    ($areaFilter.Count -eq 0 -or $areaFilter -contains $drt.Area) -and
    ($nameFilter.Count -eq 0 -or @($nameFilter | Where-Object { $drt.Name.IndexOf($_, [StringComparison]::OrdinalIgnoreCase) -ge 0 }).Count -gt 0)
})

if ($selected.Count -eq 0) {
    Write-Host "##vso[task.logissue type=error]No DRTs match: Platform=$Platform Area='$Area' Name='$Name' Disabled=$([bool]$Disabled)."
    exit 1
}

# Matrix keys become part of the job name, which must start with a letter and contain only
# letters, digits and underscores.
$matrix = [ordered]@{}
foreach ($group in $selected | Group-Object Area | Sort-Object Name) {
    $key = $group.Name -replace '[^A-Za-z0-9_]', '_'
    if ($key -notmatch '^[A-Za-z]') { $key = "Area$key" }
    $matrix[$key] = [ordered]@{ DrtArea = $group.Name; DrtCount = "$($group.Count)" }
}

Write-Host "$($selected.Count) DRTs in $($matrix.Count) jobs:"
foreach ($entry in $matrix.GetEnumerator()) {
    Write-Host ("  {0,-26} {1,3} DRTs" -f $entry.Value.DrtArea, $entry.Value.DrtCount)
}

$json = $matrix | ConvertTo-Json -Compress
if ($OutputVariable) {
    Write-Host "##vso[task.setvariable variable=$OutputVariable;isOutput=true]$json"
}
else {
    $json
}
