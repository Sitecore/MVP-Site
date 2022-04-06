Import-Module -Name (Join-Path $PSScriptRoot "docker\tools\Init-Env") -Force

Show-Logo

if (!(Confirm -Question "This will stop running containers and run a docker system prune.`n`nAre you sure you want to do that?")) {
    Write-Host "Okay, nevermind then.." -ForegroundColor Cyan
    exit 0
}

if ( !(Test-Path ".\docker")) {
    Write-Host "Eh, no docker folder in solution root (?) - there's nothing to stop.." -ForegroundColor Cyan
    exit 0
}

Stop-Docker -TakeDown -PruneSystem

Write-Host "Job's done.." -ForegroundColor Green