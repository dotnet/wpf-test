#
# TestArtifactHelpers.psm1 - Helpers used by DownloadTestArtifacts.ps1
#

Function Get-CamelCase {
    param(
        [string]$str
    )
    $textInfo = (Get-Culture).TextInfo
    return $textInfo.ToTitleCase($str)
}

Function Get-LowerCase {
    param(
        [string]$str
    )
    $textInfo = (Get-Culture).TextInfo
    return $textInfo.ToLower($str)
}

Function Confirm-Folder {
    param (
        [string]$folder
    )

    if ((Test-Path $folder) -and (-not ((Get-Item $folder) -is [System.IO.DirectoryInfo]))) {
        Write-Verbose "Removing File $folder..."
        Remove-Item $folder | Out-Null
    }

    if (-not (Test-Path $folder)) {
        Write-Verbose "Creating $folder..."
        New-Item -ItemType Directory $folder | Out-Null
    }
}

Function Install-App {
    param(
        [string]$msiDownloadUrl, # Download url, e.g., https://aka.ms/installazurecliwindows
        [string]$installerDisplayName, # Display name in ARP, e.g., 'Azure CLI For Windows'
        [string]$appCommand=$null, # Command that is installed by the software., E.g., 'az'
        [bool]$showInstaller=$false
    )
    $needsInstall = $true 

    # Test if app with $installerDisplayName is installed
    $uninstallEntry = 
        ((Get-ItemProperty HKLM:\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*) + (Get-ItemProperty HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*))  | 
            Where-Object { $_ -and $_.DisplayName -and $_.DisplayName.Contains($installerDisplayName) } 

    if ($uninstallEntry -ne $null)  {
        # App is already installed installed. 

        if (-not [string]::IsNullOrEmpty($appCommand)) {
            # See if the command installed by the app can be found
            if ((Get-Command -Name $appCommand -ErrorAction SilentlyContinue) -ne $null) {
                $needsInstall = $false
            }
        } else {
            $needsInstall = $false
        }
    }

    if ($needsInstall) {
        # Download the app
        # First create a temp MSI path 
        $tempFile = New-TemporaryFile
        $tempFile = Rename-Item -Path $tempFile -NewName ([System.IO.Path]::GetFileName([System.IO.Path]::ChangeExtension($tempFile, '.msi'))) -PassThru
        Remove-Item $tempFile 

        Write-Verbose "Downloading $installerDisplayName..."
        Invoke-WebRequest -Uri $msiDownloadUrl -OutFile $tempFile
        Write-Verbose "Installing $installerDisplayName..."
        
        $argumentList = "/I $tempFile"
        if ($showInstaller) {
            $argumentList = $argumentList + " /passive"
        } else {
            $argumentList = $argumentList + " /quiet"
        }

        Start-Process -Verb RunAs msiexec.exe -Wait -ArgumentList $argumentList
    }
}


Function Get-Tests {
    param(
        [string]$arch, # Comma separated list of architectures x86, x64 etc.
        [string]$configurations, # Comma separated list of configurations Release, Debug etc. 
        [string]$destinationFolder, # Destination Folder
        [string]$runId, # Build ID
        [bool]$unzip=$true # Unzip tests after downloading when $true
    )

    # This workflow will allow parallel downloading of zip files
    WorkFlow Download-TestArtifacts{
        param(
            [string[]]$configurations,
            [string[]]$architectures,
            [string]$downloadFolder,
            [string]$runID,
            [bool]$unzip
        )

        ForEach -Parallel ($arch in $architectures){
            ForEach -Parallel ($config in $configurations) {
                az pipelines runs artifact download --org https://dev.azure.com/dnceng --project internal --run-id $runId --artifact-name "Tests.$config.$arch.zip" --path $downloadFolder 2>&1
                if ($unzip) {
                    $zipFile = Join-Path $downloadFolder "Tests.$config.$arch.zip"
                    $expandedFolder = Join-Path $downloadFolder "Tests.$config.$arch"
                    Confirm-Folder $expandedFolder

                    Write-Verbose "Extracting $zipFile to $expandedFolder..."
                    Expand-Archive $zipFile -DestinationPath $expandedFolder
                }
            }
        }
    }

    Write-Verbose "Configurations: $configurations; Architectures: $arch"

    [string[]]$configs = $configurations.Split(',') |% { Get-CamelCase $_.Trim() }
    [string[]]$archs = $arch.Split(',') |% { Get-LowerCase $_.Trim() }

    # Create a download folder using the Build ID
    $downloadFolder = Join-Path $destinationFolder $runId
    Write-Verbose "Artifacts will be downloaded to $downloadFolder"

    if (test-path $downloadFolder) {
        Write-Verbose "$downloadFolder already exists - removing"
        Remove-Item $downloadFolder -Recurse
    }

    if (-not (test-path $downloadFolder)) {
       Write-Verbose "Creating $downloadFolder"
       New-Item $downloadFolder -ItemType Directory |Out-Null
    }

    Write-Verbose "Architectures to download: $archs" 
    Write-Verbose "Configurations to download: $configs"
    Download-TestArtifacts -configurations $configs -architectures $archs -downloadFolder $downloadFolder -runID $runId -unzip $unzip


    return $downloadFolder
}


