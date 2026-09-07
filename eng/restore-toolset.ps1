#
# This file should be kept in sync across https://www.github.com/dotnet/wpf and dotnet-wpf-int repos. 
#

# One-time setup for Wpf's custom toolset
function InitializeWpfCustomToolset() {
  # Get all sdks listed in repo's 'global.json' file
  $msbuild_sdks = $GlobalJson.'msbuild-sdks'

  # Determine if WpfArcadeSdk is present in this repo's 'global.json' file.
  #
  # The Arcade.Wpf.Sdk will only be present in 'global.json' if it is not available in the
  # local repo (under repo_root/eng/wpfarcadesdk).  The WpfArcadeSdk will be available
  # in the internal WPF repo's 'global.json' only (dotnet-wpf-int), as it needs to resolve
  # the location during build time from the NuGet cache.  The public WPF GitHub repo has 
  # a local copy of the WPF Arcade SDK and a 'global.json' entry for the sdk is not required.
  if ('Microsoft.DotNet.Arcade.Wpf.Sdk'  -in $msbuild_sdks.PSobject.Properties.Name) {

      # Install WPF git hooks when WpfArcadeSdk is located in the NuGet cache (dotnet-wpf-int)
      if (!$ci)
      {
          $installGitHooksProject = Join-Path $ToolsetDir "wpfInstallWPFPreCommitGitHook.proj"
          '<Project Sdk="Microsoft.DotNet.Arcade.Wpf.Sdk"/>' | Set-Content $installGitHooksProject
          $installGitHooksBinLog = if ($binaryLog) { "/bl:" + (Join-Path $LogDir "InstallGitHooks.binlog") } else { "" }
          MSBuild $installGitHooksProject $installGitHooksBinlog /t:InstallWPFPreCommitGitHook /clp:ErrorsOnly`;NoSummary 
      }
  }
}

# Installs custom WPF git hook to prevent modification of generated files 
function InstallCustomWPFGitHooksFromLocalToolsPath {

  # Install the githook using the inline task if WpfArcadeSdk is located in 
  # engineering root.  This should only be the case for the public GitHub repo
  # (e.g., dotnet-wpf/eng/wpfarcadesdk.)
  $WPFArcadeSDKPath = Join-Path $EngRoot "wpfarcadesdk";

  if (Test-Path $WPFArcadeSDKPath) {

    # Install the githook using the script
    $WPFPreCommitGitHookSource = Join-Path $EngRoot "wpfarcadesdk\tools\pre-commit.githook"
    $WPFPreCommitGitHookDest = Join-Path $RepoRoot ".git\hooks\pre-commit"

    if (-not (Test-Path $WPFPreCommitGitHookSource)) {
        Write-Host "WPF PreCommit GitHook file is missing: $WPFPreCommitGitHookSource"
        ExitWithExitCode 1
    }

    Write-Host "Detecting WPF Git hooks..."

    if (-not (Test-Path $WPFPreCommitGitHookDest)) {
         Write-Host "Installing WPF Git pre-commit hook..."
         try {
            Copy-Item -Path $WPFPreCommitGitHookSource -Destination $WPFPreCommitGitHookDest 
         }
         catch {
          Write-Host "Error: WPF Git pre-commit hook installation failed!"
          Write-Host $_
          Write-Host $_.Exception
          Write-Host $_.ScriptStackTrace
          ExitWithExitCode 1
        }
    }
    else {
     Write-Host "Detected existing WPF Git pre-commit hook."
    }
  }
  else
  {
      Write-Host "InstallCustomWPFGitHooks: WpfArcadeSdk was not available in repo's engineering root.";
  }
}

InitializeWpfCustomToolset

if (!$ci)
{
    InstallCustomWPFGitHooksFromLocalToolsPath 
}

. $PsScriptRoot\common\init-tools-native.ps1 -InstallDirectory $PSScriptRoot\..\.tools\native -GlobalJsonFile $PSScriptRoot\..\global.json

