# Homemade dotnet "watch" script, watches the deploy folder for changes to assembly files. 
# Stops the running app, syncs the changed files and starts the app up again. No  Author: @anderslaub

[CmdletBinding()]
param(
    [ValidateScript( { Test-Path $_ -PathType 'Container' })] 
    [string]$Path='C:\Deploy',
    [ValidateScript( { Test-Path $_ -PathType 'Container' })] 
    [string]$Destination='C:\App',
    [Parameter(Mandatory = $false)]
    [int]$SleepMilliseconds = 1000
)

$ErrorActionPreference = "STOP"

function Get-Timestamp {
    "$(Get-Date -Format "HH:mm:ss:fff")"
}

function Stop-DotNetApp {
    param(
        [Parameter(Mandatory = $true)]
        $AppProcessId
    )
   Stop-Process -Id $AppProcessId -ErrorAction SilentlyContinue
   Write-Host ("$(Get-Timestamp): App process stopped, waiting $($SleepMilliseconds)ms for file lock release..") -ForegroundColor Magenta
   Start-Sleep -Milliseconds $SleepMilliseconds # Give the app some time to shutdown and release file locks.
}

function Start-DotNetApp  {
    param(
        [Parameter(Mandatory = $true)]
        $AppJobName
    )
    $AppProcess = Start-Process "dotnet" -WorkingDirectory C:\app\ -NoNewWindow -Args $AppJobName -PassThru
    Write-Host ("$(Get-Timestamp): App '$($AppJobName)' started in process '$($AppProcess.Id)'..") -ForegroundColor Green
    $AppProcess.Id
}


function Sync-Delta {
    param(
        [Parameter(Mandatory = $true)]
        $FilePath,
        [Parameter(Mandatory = $true)]
        $Source,
        [Parameter(Mandatory = $true)]
        $Destination
    )

    if ($FilePath.StartsWith($Destination)) {
        Remove-Item $FilePath -Force  -ErrorVariable Exc -ErrorAction SilentlyContinue | Out-Null
        if ($Exc) {
            $Exc
        }
        return;
    }

    $DestinationPath = $FilePath.Replace($Source, $Destination)
    If (-not (Test-Path $DestinationPath)) {
        New-Item -ItemType File -Path $DestinationPath -Force | Out-Null
    }

    Copy-Item -Path $FilePath -Destination $DestinationPath -Force -ErrorAction SilentlyContinue -ErrorVariable Exc | Out-Null
    if ($Exc) {
        $Exc
    }
}

function Get-WatchedFiles {
    param (
        [Parameter(Mandatory = $true)]
        $RootFolder
    )
    if (Test-Path $RootFolder\wwwroot\) {
        return (Get-ChildItem $RootFolder\*.dll,$RootFolder\*.json) + (Get-ChildItem $RootFolder\wwwroot\*.* -Recurse)
    }
    Get-ChildItem $RootFolder\*.dll,$RootFolder\*.json
}

function Sync {
    param(
        [Parameter(Mandatory = $true)]
        $Path,
        [Parameter(Mandatory = $true)]
        $Destination,
        [Parameter(Mandatory = $true)]
        $LastRunSourceFiles,
        [Parameter(Mandatory = $true)]
        $AppProcessId
    )

    $files = Get-WatchedFiles -RootFolder $Path

    $SourceFiles = $files | ForEach-Object { Get-FileHash -Path $_.FullName }
    if ($null -eq $SourceFiles -or $SourceFiles.Count -eq 0) {
        return $null
    }

    $DestinationFiles = Get-WatchedFiles -RootFolder $Destination | ForEach-Object { Get-FileHash -Path $_.FullName }
    
    if ($null -eq $DestinationFiles) {
        $Deltas = $SourceFiles
    }
    else {
        $Deltas = @(Compare-Object -ReferenceObject $SourceFiles -DifferenceObject $DestinationFiles -Property Hash -PassThru)
    }
    $SourceDeltas = @(Compare-Object -ReferenceObject $SourceFiles.Hash -DifferenceObject $LastRunSourceFiles.Hash -PassThru)
    $SourceFilesStable = $SourceDeltas.Count -eq 0

    if ($Deltas.Count -gt 0 -and $SourceFilesStable) {
        Write-Host ("$(Get-Timestamp): Changes detected. Stopping app.") -ForegroundColor Yellow
        
        Stop-DotNetApp $AppProcessId
        $SyncErrors = $Deltas | % { Sync-Delta $_.Path $Path $Destination }
        if ($null -ne $SyncErrors -band $SyncErrors.Count -gt 0) {
            Write-Host ("$(Get-Timestamp): Errors occurred while syncing:`n`t$(($SyncErrors -join "`n`t"))") -ForegroundColor Red
        }
        Write-Host ("$(Get-Timestamp): Syncing done. Restarting app.") -ForegroundColor Yellow

        $AppProcessId = Start-DotNetApp $ENV:ENTRYPOINT_ASSEMBLY
    }
    @{ 
        SourceFiles = $SourceFiles 
        AppProcessId = $AppProcessId
    }
}

Write-Host ("$(Get-Timestamp): Sitecore Development ENTRYPOINT, starting...")

if ($null -eq $ENV:ENTRYPOINT_ASSEMBLY -or !(Test-Path "C:\App\$($ENV:ENTRYPOINT_ASSEMBLY)")) {
    Write-Host ("$(Get-Timestamp): ENTRYPOINT_ASSEMBLY environment variable is not set or the dll does not exist. Exiting..") -ForegroundColor Red
    Exit 1
}

try {
    Write-Host ("$(Get-Timestamp): Changes on '$($Path)', will deploy to '$($Destination)'...")
    $LastRunSourceFiles = Get-WatchedFiles -RootFolder $Destination | ForEach-Object { Get-FileHash -Path $_.FullName }
    $ProcessId = Start-DotNetApp -AppJobName $ENV:ENTRYPOINT_ASSEMBLY
    Start-Sleep -Milliseconds $SleepMilliseconds
    
    while ($true) {    
        $SyncState = Sync -Path $Path -Destination $Destination -LastRunSourceFiles $LastRunSourceFiles -AppProcessId $ProcessId
        if ($null -ne $SyncState) {
            $LastRunSourceFiles = $SyncState.SourceFiles
            $ProcessId = $SyncState.AppProcessId
        }
        Start-Sleep -Milliseconds $SleepMilliseconds
    }
}
catch {
    Write-Host ("$(Get-Timestamp): $($_.Exception.Message)") -ForegroundColor Red
    Write-Host ("$(Get-Timestamp): $($_.Exception.StackTrace)") -ForegroundColor Red
    Exit 1
}
finally {
    Write-Host ("$(Get-Timestamp): Development.ps1 stopped..") -ForegroundColor Magenta
}
