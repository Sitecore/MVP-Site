#Requires -RunAsAdministrator
param (
    [ValidateNotNullOrEmpty()]
    [string] 
    $ComposeProjectName="sc-mvp",
    [ValidateNotNullOrEmpty()]
    [string] 
    $LicensePath = "c:\sitecore\license.xml",
    [switch]
    $InitializeEnvFile,
    [Switch]
    $Pull,
    [Switch]
    $Clean,
    [Switch]
    $StartMvpSite,
    [Switch]
    $StartSugconSites
)

Import-Module -Name (Join-Path $PSScriptRoot "docker\tools\Init-Env") -Force

Show-Logo

if (!(Test-Path ".\docker\license\license.xml")) {
    Write-Host "License.xml not found in .\docker\license\" -ForegroundColor Yellow
    
    if (!(Test-Path $LicensePath)) {
        Write-Host "Please copy a valid Sitecore license file to .\docker\license\ or supply a path to license file using the -LicensePath arg.." -ForegroundColor Red
        exit 0
    }
    
    Write-Host "Copying $($LicensePath) to .\docker\license\" -ForegroundColor Green
    Copy-Item $LicensePath ".\docker\license\license.xml"
}

Stop-IisIfRunning

$HostDomain = "$($ComposeProjectName.ToLower()).localhost"

if (!(Test-Path ".\.env") -or $InitializeEnvFile) { 
    Initialize-EnvFile -HostDomain $HostDomain -ComposeProjectName $ComposeProjectName

    # Rendering site hostnames..
    Set-EnvFileVariable "MVP_RENDERING_HOST" -Value "mvp.$($HostDomain)"
    Set-EnvFileVariable "SUGCON_EU_RENDERING_HOST" -Value "sugcon-eu.$($HostDomain)"
    Set-EnvFileVariable "SUGCON_ANZ_RENDERING_HOST" -Value "sugcon-anz.$($HostDomain)"

    # OKTA Dev stuff...
    Set-EnvFileVariable "OKTA_DOMAIN" -Value (Read-ValueFromHost -Question "OKTA Domain" -Required)
    Set-EnvFileVariable "OKTA_CLIENT_ID" -Value (Read-ValueFromHost -Question "OKTA Client ID" -Required)
    Set-EnvFileVariable "OKTA_CLIENT_SECRET" -Value (Read-ValueFromHost -Question "OKTA Client Secret" -Required)
}


if (!(Test-Path ".\docker\traefik\certs\cert.pem")) {
    Write-Host "TLS certificate for Traefik not found, generating and adding hosts file entries" -ForegroundColor Green
    $HostDomain = Get-EnvValueByKey "HOST_DOMAIN" 
    if ($HostDomain -eq "") {
        throw "Required variable 'HOST_DOMAIN' not found in .env file."
    }
    Initialize-HostNames $HostDomain

    # Rendering site hostnames..
    Add-HostsEntry "mvp.$($HostDomain)"
    Add-HostsEntry "sugcon-eu.$($HostDomain)"
    Add-HostsEntry "sugcon-anz.$($HostDomain)"

}

if ($Pull) {
    Write-Host "Pulling the latest Sitecore base images.." -ForegroundColor Magenta
    docker images --format "{{.Repository}}:{{.Tag}}" | Select-String -Pattern "scr.sitecore.com/" | % { docker image pull $($_) }
}

if ($Clean) {
    Write-Host "Cleaning content in deploy and data folders.." -ForegroundColor Magenta
    ./docker/clean.ps1
}

$composeFiles = @(".\docker-compose.yml", ".\docker-compose.override.yml")

$startAll = !$StartMvpSite -and !$StartSugconSites

if ($startAll -or $StartMvpSite) {
    $composeFiles += ".\docker-compose.mvp.yml"
} 

if ($startAll -or $StartSugconSites) {
    $composeFiles += ".\docker-compose.sugcon.yml"
} 

# Restore dotnet tool for sitecore login and serialization
dotnet tool restore

Start-Docker -Build -ComposeFiles $composeFiles

Push-Items -IdHost "https://id.$($HostDomain)" -CmHost "https://cm.$($HostDomain)"

#TODO: this will be generalized when more sugcon sites are added.
if ($startAll -or $StartMvpSite) {
    Write-Host "`nMVP site is accessible on https://mvp.$HostDomain/`n`nUse the following command to monitor:"  -ForegroundColor Magenta
    Write-PrePrompt
    Write-Host "docker logs -f mvp-rendering`n"
} 

if ($startAll -or $StartSugconSites) {
    Write-Host "`nSUGCON EU site is accessible on https://sugcon-eu.$HostDomain/`n`nUse the following command to monitor:"  -ForegroundColor Magenta
    Write-PrePrompt
    Write-Host "docker logs -f sugcon-eu-rendering`n"
} 

Write-Host "Opening cm in browser..." -ForegroundColor Green
Start-Process https://cm.$HostDomain/sitecore/