<#
.SYNOPSIS
    Downloads WPF Test binaries from dotnet-wpf-test repo
.DESCRIPTION
     Downloads x86 and x64 versions of the .NET Core SDK to $env:ProgramFiles\dotnet and $env:ProgramFiles(x86)\dotnet
     respectively.

     Downloads WPF test binaries from dnceng/dotnet-wpf-test repo. By default, the script downloads x86  and x64 Release
     versions of the binaries from master branch, and downloads and unzips them into $env:TEMP\Test\<BuildID>\ folder. 

.PARAMETER version
    Version number of the core-sdk build to install, defaults to "latest". Latest version
    number can be found at https://github.com/dotnet/core-sdk and https://dotnet.microsoft.com/download
.PARAMETER channel
    "channel" to install from, defaults to "master"
    Valid values for -channel are:
        Current - Most current release
        LTS - Long-Term Support channel (most current supported release)
        Two-part version in X.Y format representing a specific release (for example, 2.0, or 1.0)
        https://github.com/dotnet/core-sdk branch name (case-sensitive); for example, 
            release/3.0.1xx, or master (for nightly releases)
.PARAMETER Verbose 
    Displays verbose information
.PARAMETER branch
    The branch from which to download tests. Defaults to 'master'
.PARAMETER destinationFolder
    The base folder where the tests are to be downloaded. Defaults to $env:TEMP\Test
    The final folder where the tests are downloaded appends the AzDo Build ID to this path, such
    that the effective download path is something like $env:TEMP\Test\<Build ID>\
.PARAMETER pipelineDefinitionID
    The Pipeline Definition ID of the build pipline for "dotnet-wpf-test CI". It's value is 479. 
    Do not modify/override this value except for debugging purposes. 
.PARAMETER architectures
    Comma separated list of architectures. Only x86 and x64 are supported. 
    Defaults to "x86,x64". 
    Note: In the commandline, the comma separated values should be quoted like "x86, x64"
.PARAMETER configurations
    Comma separated list of Build configurations. Valid values are Release, Debug. 
    Defaults to "Release". 
    Note: In the command line, the comma seaprated values should be quoted, like "Release, Debug"
.PARAMETER unzip
    When set to $true, unzips the tests after downloading. Defaults to $true. 
.EXAMPLE
    PS> DownloadTestArtifacts.ps1
    Downloads and unzips latest test binaries from 'master' branch
.EXAMPLE
     PS> DownloadTestArtifacts.ps1 -branch release/3.0
    Downloads and unzips latest test binaries from 'release/3.0' branch
.EXAMPLE
     PS> DownloadTestArtifacts.ps1 -branch release/3.0 -architectures "x86,x64" -configurations "Debug, Release"
    Downloads and unzips latest test binaries from 'release/3.0' branch, for x86, x64 architectures and Debug, Release confiugrations
.EXAMPLE
     PS> DownloadTestArtifacts.ps1 -desitinationFolder C:\Test -architectures x86 -configurations Debug
    Downloads and unzips latest x86 Debug test binaries from master branch into C:\Test\<BuildID> directory
.EXAMPLE
     PS> DownloadTestArtifacts.ps1 -unzip $false
    Downloads the latest x86, x64 (Relase only) test binaries from master branch into $env:TEMP\Test\<Build ID>\; Does not unzip them. 
#>
[CmdletBinding(PositionalBinding=$false)]
param(
  [string][Alias('b')]
  [Parameter(HelpMessage='branch')]
  $branch = 'master',

  [string][Alias('d')]
  [Parameter(HelpMessage='Destination folder')]
  $destinationFolder = (Join-Path $env:TEMP 'Test'),

  [string][Alias('p')]
  [Parameter(HelpMessage='Pipeline Definition ID (default "dotnet-wpf-test CI"= 479)')]
  $pipelineDefinitionID = '479',

  [string][Alias('a')]
  [Parameter(HelpMessage='Download architectures, comma separated; Default = "x86,x64"')]
  $architectures = "x86, x64",

  [string][Alias('c')]
  [Parameter(HelpMessage='Download configurations, comma separated, Valid values = {Release, Debug}, Default = Release')]
  $configurations = "Release",
  
  [bool][Alias('u')]
  [Parameter(HelpMessage='Unzip tests after downloading, Default = true')]
  $unzip = $true
)


[System.Diagnostics.Stopwatch]$StopWatch = [System.Diagnostics.Stopwatch]::StartNew()

Import-Module (Join-Path $PSScriptRoot TestArtifactHelpers.psm1) -Force 

# Ensure $destinationFolder exists
Confirm-Folder $destinationFolder

# Install Azure CLI if it doesn't already exist 
Install-App -msiDownloadUrl https://aka.ms/installazurecliwindows -installerDisplayName 'Microsoft Azure CLI' -appCommand 'az' -showInstaller $true

if (-not (Get-Command 'az')) {
    Write-Warning "'az' command not available. Try restarting the poweshell host"
    Exit-PSSession
}

# Login to Azure DevOps
Write-Host "Launching browser to login to Azure DevOps..."
az login 2>&1 | Out-Null

# Ensure that azure-devops extension is added
az extension add --name azure-devops 2>&1 | Out-Null

# Get the latest successful build from definition ID 479 ("dotnet-wpf-test CI" pipeline)
# https://docs.microsoft.com/en-us/cli/azure/pipelines/runs?view=azure-cli-latest#az-pipelines-runs-list
Write-Verbose "Getting latest successful build for branch=$branch"
$latestBuild = (az pipelines runs list --org https://dev.azure.com/dnceng --project internal --pipeline-id $pipelineDefinitionID --branch $branch --result succeeded --status completed --top 1 | ConvertFrom-Json)
$count = $latestBuild.count
Write-Verbose "$count builds found."

# https://docs.microsoft.com/en-us/cli/azure/pipelines/runs/artifact?view=azure-cli-latest
$artifacts = (az pipelines runs artifact list --org https://dev.azure.com/dnceng --project internal --run-id $latestBuild.id |ConvertFrom-Json)

Write-Verbose "Downloading Tests to $destinationFolder..."
$downloadFolder = Get-Tests -arch $architectures -configurations $configurations -destinationFolder $destinationFolder -runId $latestBuild.id -unzip $unzip

Write-Host "Downloaded tests to $downloadFolder"

$StopWatch.Stop()

Write-Host 
Write-Host "Script run time:"
$StopWatch.Elapsed | Format-Table