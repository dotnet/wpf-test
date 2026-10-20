# TEMPORARY - M0 probe for running DRTs on build agents. See docs/drts-on-ci-implementation.md.
# Logs elevation, desktop session, screen resolution and signs of agent reuse. Never fails the job.
[CmdletBinding()]
param(
  [string] $Label = 'probe',
  [string] $OutputDirectory = $env:BUILD_ARTIFACTSTAGINGDIRECTORY
)

$ErrorActionPreference = 'Continue'

function Section([string] $name) { Write-Host ''; Write-Host "===== $name =====" }
function Try-Run([string] $name, [scriptblock] $block) {
  try { & $block } catch { Write-Host "$name failed: $($_.Exception.Message)" }
}

Section 'Identity and elevation'
Try-Run 'identity' {
  $id = [Security.Principal.WindowsIdentity]::GetCurrent()
  $principal = New-Object Security.Principal.WindowsPrincipal($id)
  Write-Host "User:            $($id.Name)"
  Write-Host "IsAdministrator: $($principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator))"
  whoami /groups | Select-String 'Mandatory Label' | ForEach-Object { Write-Host "Integrity:       $($_.Line.Trim())" }
  $uac = Get-ItemProperty 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System' -ErrorAction SilentlyContinue
  Write-Host "EnableLUA:       $($uac.EnableLUA)"
  Write-Host "ConsentPromptBehaviorAdmin: $($uac.ConsentPromptBehaviorAdmin)"
}

Section 'Session and desktop'
Try-Run 'session' {
  Write-Host "SessionId:       $([Diagnostics.Process]::GetCurrentProcess().SessionId)"
  Write-Host "UserInteractive: $([Environment]::UserInteractive)"
  query session 2>&1 | ForEach-Object { Write-Host "  $_" }
  Get-CimInstance Win32_Process -Filter "Name='explorer.exe'" | ForEach-Object {
    $owner = Invoke-CimMethod -InputObject $_ -MethodName GetOwner
    Write-Host "explorer.exe pid=$($_.ProcessId) session=$($_.SessionId) owner=$($owner.Domain)\$($owner.User)"
  }
}

Section 'Screen'
Try-Run 'screen' {
  Add-Type -AssemblyName System.Windows.Forms, System.Drawing
  foreach ($s in [System.Windows.Forms.Screen]::AllScreens) {
    Write-Host "Screen $($s.DeviceName): $($s.Bounds.Width)x$($s.Bounds.Height) primary=$($s.Primary) bpp=$($s.BitsPerPixel)"
  }
  if ($OutputDirectory) {
    $b = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds
    $bmp = New-Object System.Drawing.Bitmap($b.Width, $b.Height)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.CopyFromScreen($b.Location, [System.Drawing.Point]::Empty, $b.Size)
    New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null
    $file = Join-Path $OutputDirectory "screenshot-$Label.png"
    $bmp.Save($file, [System.Drawing.Imaging.ImageFormat]::Png)
    $g.Dispose(); $bmp.Dispose()
    Write-Host "Screenshot saved: $file"
  }
}

Section 'Paths and .NET'
Try-Run 'paths' {
  Write-Host "APPDATA:         $env:APPDATA"
  Write-Host "ProgramFiles:    $env:ProgramFiles"
  Write-Host "DOTNET_ROOT:     $env:DOTNET_ROOT"
  $shared = Join-Path $env:ProgramFiles 'dotnet\shared'
  foreach ($fx in 'Microsoft.NETCore.App', 'Microsoft.WindowsDesktop.App') {
    $dir = Join-Path $shared $fx
    $versions = if (Test-Path $dir) { (Get-ChildItem $dir -Directory).Name -join ', ' } else { '(none)' }
    Write-Host "Global $fx`: $versions"
  }
  Write-Host "Existing %ProgramFiles%\QualityVault: $(Test-Path (Join-Path $env:ProgramFiles 'QualityVault'))"
  Write-Host "Existing %APPDATA%\QualityVault:      $(Test-Path (Join-Path $env:APPDATA 'QualityVault'))"
}

Section 'Machine and agent reuse'
Try-Run 'machine' {
  $os = Get-CimInstance Win32_OperatingSystem
  Write-Host "ComputerName:    $env:COMPUTERNAME"
  Write-Host "OS:              $($os.Caption) $($os.Version)"
  Write-Host "OS installed:    $($os.InstallDate.ToUniversalTime().ToString('u'))"
  Write-Host "Last boot:       $($os.LastBootUpTime.ToUniversalTime().ToString('u'))"
  Write-Host "Now:             $([DateTime]::UtcNow.ToString('u'))"
  Write-Host "Agent:           name=$env:AGENT_NAME id=$env:AGENT_ID home=$env:AGENT_HOMEDIRECTORY"
  Get-Process Agent.Listener, Agent.Worker -ErrorAction SilentlyContinue | ForEach-Object {
    Write-Host "$($_.ProcessName) started: $($_.StartTime.ToUniversalTime().ToString('u'))"
  }
  if ($env:PIPELINE_WORKSPACE) {
    Write-Host "Agent work folders: $((Get-ChildItem (Split-Path $env:PIPELINE_WORKSPACE) -Directory -ErrorAction SilentlyContinue).Name -join ', ')"
  }

  $markerDir = Join-Path $env:ProgramData 'WpfDrtProbe'
  New-Item -ItemType Directory -Force -Path $markerDir | Out-Null
  $existing = Get-ChildItem $markerDir -File -ErrorAction SilentlyContinue
  if ($existing) {
    Write-Host "AGENT REUSED: found $($existing.Count) marker(s) from earlier jobs:"
    $existing | ForEach-Object { Write-Host "  $($_.Name): $(Get-Content $_.FullName -Raw)" }
  } else {
    Write-Host 'No markers from earlier jobs: this machine looks fresh.'
  }
  $marker = Join-Path $markerDir "$env:BUILD_BUILDID-$env:SYSTEM_JOBID.txt"
  "build=$env:BUILD_BUILDID job=$env:SYSTEM_JOBDISPLAYNAME label=$Label time=$([DateTime]::UtcNow.ToString('u'))" | Set-Content $marker
}

exit 0
