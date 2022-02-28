Set-StrictMode -Version Latest
function Start-Docker {
    param(
        [ValidateNotNullOrEmpty()]
        [string] 
        $DockerRoot = ".\",
        [Switch]
        $Build,
        [string]
        $MemoryLimit = "8GB",
        [String[]]
        $ComposeFiles = @(".\docker-compose.yml", ".\docker-compose.override.yml")
    )

    $fileArgs =  ($ComposeFiles | %{ "-f" + "$_" })
    if ($Build) {
       & "docker-compose" $fileArgs "build" "-m" "$MemoryLimit"
    }
    & "docker-compose" $fileArgs "up" "-d"
}

function Stop-Docker {
    param(
        [Switch]$PruneSystem
    )
    if (Test-Path ".\docker-compose.yml") {
        docker-compose down --remove-orphans

        if ($PruneSystem) {
            docker system prune -f
        }
    }
    Pop-Location
}