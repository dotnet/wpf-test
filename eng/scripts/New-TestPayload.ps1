<#
.SYNOPSIS
Splits the published feature tests into one small payload per test folder, for the tests that
match the given keywords (for example MicroSuite).

.DESCRIPTION
The full publish\<config>\<platform>\Test folder is about 1.8 GB. A test only needs Infra\,
Common\ and its own FeatureTests\<folder>, which holds its assemblies, support files and
deployments. This script:

1. Runs QualityVault discovery (no tests run, no elevation needed) on DiscoveryInfo.xml with the
   given filters, to find the enabled tests and the FeatureTests folders their support files use.
2. Creates <OutputDirectory>\<folder> for each such folder, with Infra\, Common\, the QualityVault
   scripts, the FeatureTests folder, Invoke-Drts.ps1 and a DiscoveryInfo.xml that only has the
   targets in that folder. Tests that use several folders put those folders in one payload.
3. Writes the Azure DevOps job matrix (one job per payload) to <OutputDirectory>\matrix.json and,
   with -OutputVariable, to a pipeline output variable.

Run a payload with:
  <payload>\Invoke-Drts.ps1 -DiscoveryInfo DiscoveryInfo.xml -Keywords MicroSuite
See docs/drts-on-ci-implementation.md (MicroSuites).

.EXAMPLE
eng\scripts\New-TestPayload.ps1 -Keywords MicroSuite -OutputDirectory artifacts\TestPayload
#>
[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [ValidateSet('x86', 'x64')]
    [string]$Platform = 'x64',

    [Parameter(Mandatory = $true)]
    [string]$OutputDirectory,

    # QualityVault /Keywords= filter, for example MicroSuite.
    [Parameter(Mandatory = $true)]
    [string]$Keywords,

    # Comma-separated test areas (as in the test report, for example Controls or XamlV4), matched
    # exactly (case-insensitive). Payloads with no test in these areas are left out. Blank for all.
    [string]$Area,

    # .NET installation to run QualityVault discovery on. Defaults to the repo's .dotnet folder.
    [string]$DotNetRoot,

    # Name of the pipeline output variable to set with the matrix as JSON.
    [string]$OutputVariable
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$source = Join-Path $repoRoot "publish\$Configuration\$Platform\Test"
$discoveryInfoPath = Join-Path $source 'DiscoveryInfo.xml'
if (-not (Test-Path $discoveryInfoPath)) {
    throw "Not found: $discoveryInfoPath. Build with -configuration $Configuration /p:Platform=$Platform first."
}

if (-not $DotNetRoot) { $DotNetRoot = Join-Path $repoRoot '.dotnet' }
$env:DOTNET_ROOT = (Resolve-Path $DotNetRoot).Path
$env:DOTNET_MULTILEVEL_LOOKUP = '0'

# 1. Discover the tests. QualityVault writes ProfilingReport.txt to the current directory.
$discoveryDirectory = Join-Path ([IO.Path]::GetTempPath()) "TestPayloadDiscovery_$PID"
if (Test-Path $discoveryDirectory) { Remove-Item $discoveryDirectory -Recurse -Force }
New-Item -ItemType Directory $discoveryDirectory | Out-Null
$frontEnd = Join-Path $source 'Infra\QualityVaultFrontEnd.exe'
Push-Location $discoveryDirectory
try {
    $runDirectory = Join-Path $discoveryDirectory 'Run'
    & $frontEnd RegisterForDistribution "/RunDirectory=$runDirectory" /DistributionKey=M1 | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "QualityVault RegisterForDistribution failed with exit code $LASTEXITCODE." }
    & $frontEnd DiscoverAndDistribute "/DiscoveryInfoPath=$discoveryInfoPath" "/RunDirectory=$runDirectory" "/Keywords=$Keywords" | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "QualityVault DiscoverAndDistribute failed with exit code $LASTEXITCODE." }
    $collection = [xml](Get-Content (Join-Path $runDirectory 'M1\TestCollection.xml') -Raw)
}
finally {
    Pop-Location
    Remove-Item $discoveryDirectory -Recurse -Force -ErrorAction SilentlyContinue
}

# FeatureTests folder names as they are on disk; paths in DiscoveryInfo.xml differ in case.
$folderNames = @{}
foreach ($folder in Get-ChildItem (Join-Path $source 'FeatureTests') -Directory) {
    $folderNames[$folder.Name.ToLowerInvariant()] = $folder.Name
}
function Get-FeatureFolder([string]$path) {
    $parts = $path -split '[\\/]'
    if ($parts.Count -ge 2 -and $parts[0] -eq 'FeatureTests') {
        return $folderNames[$parts[1].ToLowerInvariant()]
    }
    return $null
}

$areaFilter = @(if ($Area) { $Area.Split(',') | ForEach-Object { $_.Trim() } | Where-Object { $_ } })
$tests = @($collection.SelectNodes('//TestRecord[@ExecutionEnabled="True"]') | ForEach-Object {
    $info = $_.TestInfo
    $folders = @($info.SelectNodes('SupportFiles/TestSupportFile') | ForEach-Object { Get-FeatureFolder $_.Source } |
        Where-Object { $_ } | Sort-Object -Unique)
    if ($folders.Count -eq 0) {
        throw "Test $($info.Area).$($info.Name) uses no FeatureTests folder, so it cannot be assigned to a payload."
    }
    [pscustomobject]@{ Area = $info.Area; Name = $info.Name; Folders = $folders }
} | Where-Object { $areaFilter.Count -eq 0 -or $areaFilter -contains $_.Area })

if ($tests.Count -eq 0) {
    Write-Host "##vso[task.logissue type=error]No enabled tests match: Keywords='$Keywords' Area='$Area'."
    exit 1
}

# Group the folders: tests that use several folders put them in the same payload.
$groups = New-Object System.Collections.ArrayList
foreach ($test in $tests) {
    $merged = @($test.Folders)
    foreach ($group in @($groups)) {
        if (@($group | Where-Object { $merged -contains $_ }).Count -gt 0) {
            $merged += $group
            $groups.Remove($group)
        }
    }
    [void]$groups.Add(@($merged | Sort-Object -Unique))
}

# 2. Create the payloads.
if (Test-Path $OutputDirectory) { Remove-Item $OutputDirectory -Recurse -Force }
New-Item -ItemType Directory $OutputDirectory | Out-Null
$sharedFiles = @(Get-ChildItem $source -File | Where-Object {
    $_.Name -notlike 'DiscoveryInfo*.xml' -and $_.Name -ne 'ProfilingReport.txt'
})

$matrix = [ordered]@{}
function Copy-Folder([string]$from, [string]$to) {
    # robocopy exit codes below 8 mean success.
    robocopy $from $to /E /MT:16 /NFL /NDL /NJH /NJS /NP | Out-Null
    if ($LASTEXITCODE -ge 8) { throw "robocopy $from $to failed with exit code $LASTEXITCODE." }
}
foreach ($group in $groups | Sort-Object { $_[0] }) {
    $name = $group[0]
    $payload = Join-Path $OutputDirectory $name
    foreach ($folder in 'Infra', 'Common') {
        Copy-Folder (Join-Path $source $folder) (Join-Path $payload $folder)
    }
    foreach ($folder in $group) {
        Copy-Folder (Join-Path $source "FeatureTests\$folder") (Join-Path $payload "FeatureTests\$folder")
    }
    $sharedFiles | Copy-Item -Destination $payload
    Copy-Item (Join-Path $PSScriptRoot 'Invoke-Drts.ps1') $payload

    $discoveryInfo = [xml](Get-Content $discoveryInfoPath -Raw)
    foreach ($target in @($discoveryInfo.SelectNodes('//Targets/Target'))) {
        if ($group -notcontains (Get-FeatureFolder $target.GetAttribute('Path'))) {
            [void]$target.ParentNode.RemoveChild($target)
        }
    }
    $discoveryInfo.Save((Join-Path $payload 'DiscoveryInfo.xml'))

    $groupTests = @($tests | Where-Object { @($_.Folders | Where-Object { $group -contains $_ }).Count -gt 0 })
    $areas = @($groupTests.Area | Sort-Object -Unique)
    $size = (Get-ChildItem $payload -Recurse -File | Measure-Object Length -Sum).Sum / 1MB
    # Matrix keys become part of the job name: letters, digits and underscores, starting with a letter.
    $key = $name -replace '[^A-Za-z0-9_]', '_'
    if ($key -notmatch '^[A-Za-z]') { $key = "Payload$key" }
    $matrix[$key] = [ordered]@{
        TestPayload = $name
        TestAreas   = $areas -join ','
        TestCount   = "$($groupTests.Count)"
        PayloadMB   = "{0:N0}" -f $size
    }
}

$total = (Get-ChildItem $OutputDirectory -Recurse -File | Measure-Object Length -Sum).Sum / 1MB
Write-Host ("$($tests.Count) tests in $($matrix.Count) payloads ({0:N0} MB) under $((Resolve-Path $OutputDirectory).Path):" -f $total)
foreach ($entry in $matrix.GetEnumerator()) {
    Write-Host ("  {0,-20} {1,3} tests {2,6} MB  {3}" -f $entry.Value.TestPayload, $entry.Value.TestCount, $entry.Value.PayloadMB, $entry.Value.TestAreas)
}

# 3. The job matrix.
$json = $matrix | ConvertTo-Json -Compress
Set-Content (Join-Path $OutputDirectory 'matrix.json') $json
if ($OutputVariable) {
    Write-Host "##vso[task.setvariable variable=$OutputVariable;isOutput=true]$json"
}
# Do not leak robocopy's exit code (1 means files were copied).
exit 0
