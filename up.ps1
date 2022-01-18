# ENSURE ENV FILE EXISTS
if(-not (Test-Path .env)) {
    Write-Host "Docker Env file not found. Have you run init.ps1 first? Refer to README.md for details" -ForegroundColor Red
    exit
}

# Comment the following line in case if you want to manually switch to windows container!
 & "C:\Program Files\Docker\Docker\DockerCli.exe" -SwitchWindowsEngine

# Restore dotnet tool for sitecore login and serialization
dotnet tool restore

# Build all containers in the Sitecore instance, forcing a pull of latest base containers
docker-compose build
if ($LASTEXITCODE -ne 0)
{
    Write-Error "Container build failed, see errors above."
}

# Stop IIS if running in this computer
$service=get-service w3svc  -ErrorAction SilentlyContinue
if ($null -ne $service.Name) {
	if ($service.Status -eq 'Running') {        		
		$svcStatus=iisreset /stop
		Write-Host "IIS Stopped..." -ForegroundColor Green
	}
}

# Run the docker containers
docker-compose up -d

# Wait for Traefik to expose CM route
Write-Host "Waiting for CM to become available..." -ForegroundColor Green
$startTime = Get-Date
do {
    Start-Sleep -Milliseconds 100
    try {
        $status = Invoke-RestMethod "http://localhost:8079/api/http/routers/cm-secure@docker"
    } catch {
        if ($_.Exception.Response.StatusCode.value__ -ne "404") {
            throw
        }
    }
} while ($status.status -ne "enabled" -and $startTime.AddSeconds(15) -gt (Get-Date))
if (-not $status.status -eq "enabled") {
    $status
    Write-Error "Timeout waiting for Sitecore CM to become available via Traefik proxy. Check CM container logs."
}

dotnet sitecore login --cm https://mvp-cm.sc.localhost/ --auth https://mvp-id.sc.localhost/ --allow-write true
if ($LASTEXITCODE -ne 0) {
    Write-Error "Unable to log into Sitecore, did the Sitecore environment start correctly? See logs above."
}

Write-Host "Pushing latest items to Sitecore..." -ForegroundColor Green

dotnet sitecore ser push
if ($LASTEXITCODE -ne 0) {
    Write-Error "Serialization push failed, see errors above."
}

dotnet sitecore publish
if ($LASTEXITCODE -ne 0) {
    Write-Error "Item publish failed, see errors above."
}

Write-Host "Opening site..." -ForegroundColor Green

Start-Process https://mvp-cm.sc.localhost/sitecore/
Start-Process https://mvp.sc.localhost/

Write-Host ""
Write-Host "Use the following command to monitor your Rendering Host:" -ForegroundColor Green
Write-Host "docker-compose logs -f mvp-rendering"
Write-Host ""