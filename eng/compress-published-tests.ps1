[CmdletBinding()]
param(
  [string] [Alias('c')] 
  [Parameter(HelpMessage="Build Configuration (Debug | Release)")] 
  $Configuration,
   
  [string] [Alias('p')] 
  [Parameter(HelpMessage="Build Platform (x86 | x64 | arm64)")]
  $Platform
)

Set-StrictMode -Version Latest
$ErrorActionPreference="Stop" # This means that Write-Error will terminate the script
$ProgressPreference="SilentlyContinue"

Write-Verbose Configuration=$Configuration
Write-Verbose Platform=$Platform


$repoRoot = (Get-Item $PSScriptRoot).Parent.FullName
if (-not $repoRoot) {
    Write-Error RepoRoot could not be identified
}

Write-Verbose PSScriptRoot=$PSScriptRoot


$destinationDirectory = Join-path (Join-Path $repoRoot "artifacts") "tests"
$zipFileName = "Tests." + $Configuration + "." + $Platform + ".zip"
$destination = Join-Path $destinationDirectory $zipFileName 

Write-Verbose "Destination Directory=$destinationDirectory"
Write-Verbose "Zip File Name=$zipFileName"
Write-Verbose "Destination File Full Path=$destination"


$publish = Join-Path $repoRoot "publish"
if (-not $publish) {
    Write-Error "Publish Folder could not be identified"
}

Write-Verbose "Publish Folder=$publish"


$testRoot = Join-Path (Join-Path (Join-Path $publish $Configuration) $Platform) "Test"
if (-not $testRoot) {
    Write-Error "Test folder could not be identified"
}
Write-Verbose "Test Folder=$testRoot"


if (-not (test-path -PathType Container $destinationDirectory)) {
    Write-Verbose "$destinationDirectory does not exist... creating."
    New-Item -Path $destinationDirectory -ItemType "Directory" | Out-Null 
}

Compress-Archive -Path "$testRoot\*" -DestinationPath $destination



