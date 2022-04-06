Set-StrictMode -Version Latest

function Push-Items {
    param(
            [Parameter(Mandatory = $true)]
            [ValidateNotNullOrEmpty()]
            [string] 
            $IdHost,
            [Parameter(Mandatory = $true)]
            [ValidateNotNullOrEmpty()]
            [string] 
            $CmHost
    )
    
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

    dotnet sitecore login --cm $CmHost --auth $IdHost --allow-write true
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
}