Set-StrictMode -Version Latest

@(Get-ChildItem -Path (Join-Path $PSScriptRoot Scripts) -Include *.ps1 -File -Recurse) | ForEach-Object {
    try {
        . $_.FullName
    }
    catch {
        Write-Warning $_.Exception.Message
    }
}

function Stop-IisIfRunning {
    $iisService = Get-Service w3svc  -ErrorAction SilentlyContinue
    if ($null -eq $iisService -or $null -eq $iisService.Status -or $iisService.Status -ne 'Running') {
        Write-Host "IIS is not running on local machine..." -ForegroundColor Green
        return;
    }
    $stopIis = Confirm -Question "IIS is currently running on the local machine.`n`nWould you like to stop IIS to avoid port conflicts?" -DefaultYes
    if (!$stopIis) {
        Write-Host "Warning; if IIS use port 443, traefik will fail to start." -ForegroundColor Red
        return;
    }
    iisreset /stop
    Write-Host "IIS Stopped..." -ForegroundColor Green
}


function Install-SitecoreDockerTools {
    Import-Module PowerShellGet
    $SitecoreGallery = Get-PSRepository | Where-Object { $_.SourceLocation -eq "https://sitecore.myget.org/F/sc-powershell/api/v2" }
    if (-not $SitecoreGallery) {
        Write-Host "Adding Sitecore PowerShell Gallery..." -ForegroundColor Green 
        Register-PSRepository -Name SitecoreGallery -SourceLocation https://sitecore.myget.org/F/sc-powershell/api/v2 -InstallationPolicy Trusted
        $SitecoreGallery = Get-PSRepository -Name SitecoreGallery
    }
    
    $dockerToolsVersion = "10.2.7"
    Remove-Module SitecoreDockerTools -ErrorAction SilentlyContinue
    if (-not (Get-InstalledModule -Name SitecoreDockerTools -RequiredVersion $dockerToolsVersion -ErrorAction SilentlyContinue)) {
        Write-Host "Installing SitecoreDockerTools..." -ForegroundColor Green
        Install-Module SitecoreDockerTools -RequiredVersion $dockerToolsVersion -Scope CurrentUser -Repository $SitecoreGallery.Name
    }
    Write-Host "Importing SitecoreDockerTools..." -ForegroundColor Green
    Import-Module SitecoreDockerTools -RequiredVersion $dockerToolsVersion
}

function Get-EnvValueByKey {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] 
        $Key,        
        [ValidateNotNullOrEmpty()]
        [string] 
        $FilePath = ".env",
        [ValidateNotNullOrEmpty()]
        [string] 
        $DockerRoot = ".\"
    )
    if (!(Test-Path $FilePath)) {
        $FilePath = Join-Path $DockerRoot $FilePath
    }
    if (!(Test-Path $FilePath)) {
        return ""
    }
    select-string -Path $FilePath -Pattern "^$Key=(.+)$" | % { $_.Matches.Groups[1].Value }
}

function Remove-EnvHostsEntry {
    param (
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] 
        $Key,
        [Switch]
        $Build        
    )
    $hostName = Get-EnvValueByKey $Key
    if ($null -ne $hostName -and $hostName -ne "") {
        Remove-HostsEntry $hostName
    }
}

function Initialize-EnvFile {
    param (
        [Parameter(Mandatory = $true)]
        [string] 
        $ComposeProjectName,
        [Parameter(Mandatory = $true)]
        [string] 
        $HostDomain
    )

    if (!(Test-Path ".\.env.template")) {
        Write-Host "Could not find .\.env.template"
        exit 1
    }

    Copy-Item ".\.env.template" ".\.env" -Force
    Install-SitecoreDockerTools

    Set-EnvFileVariable "MVP_DOCKER_REGISTRY" -Value (Read-ValueFromHost -Question "Solution registry - leave blank for none, must end with '/'")

    Set-EnvFileVariable "COMPOSE_PROJECT_NAME" -Value $ComposeProjectName
    $HostDomain = Read-ValueFromHost -Question "Domain Hostname" -DefaultValue $HostDomain -Required
    Set-EnvFileVariable "HOST_LICENSE_FOLDER" -Value ".\docker\license"
    Set-EnvFileVariable "HOST_DOMAIN"  -Value $HostDomain
    Set-EnvFileVariable "CM_HOST" -Value "cm.$($HostDomain)"
    Set-EnvFileVariable "CD_HOST" -Value "cd.$($HostDomain)"
    Set-EnvFileVariable "ID_HOST" -Value "id.$($HostDomain)"

    Set-EnvFileVariable "SOLR_CORE_PREFIX_NAME" -Value (Read-ValueFromHost -Question "Solr cores name prefix" -DefaultValue $ComposeProjectName -Required)
    Set-EnvFileVariable "REPORTING_API_KEY" -Value (Get-SitecoreRandomString 128 -DisallowSpecial)
    Set-EnvFileVariable "TELERIK_ENCRYPTION_KEY" -Value (Get-SitecoreRandomString 128)
    Set-EnvFileVariable "MEDIA_REQUEST_PROTECTION_SHARED_SECRET" -Value (Get-SitecoreRandomString 64 -DisallowSpecial)
    Set-EnvFileVariable "SITECORE_IDSECRET" -Value (Get-SitecoreRandomString 64 -DisallowSpecial)
    $idCertPassword = Get-SitecoreRandomString 8 -DisallowSpecial
    Set-EnvFileVariable "SITECORE_ID_CERTIFICATE" -Value (Get-SitecoreCertificateAsBase64String -DnsName "localhost" -Password (ConvertTo-SecureString -String $idCertPassword -Force -AsPlainText))
    Set-EnvFileVariable "SITECORE_ID_CERTIFICATE_PASSWORD" -Value $idCertPassword
    Set-EnvFileVariable "SQL_SA_PASSWORD" -Value (Get-SitecoreRandomString 19 -DisallowSpecial -EnforceComplexity)
    Set-EnvFileVariable "SITECORE_VERSION" -Value (Read-ValueFromHost -Question "Sitecore image version (10.2-ltsc2019, 10.2009, 10.2-20H2)" -DefaultValue "10.2-ltsc2019" -Required)
    Set-EnvFileVariable "SITECORE_ADMIN_PASSWORD" -Value (Read-ValueFromHost -Question "Sitecore admin password" -DefaultValue "b" -Required)

    if (Confirm -Question "Would you like to adjust common environment settings?") {
        Set-EnvFileVariable "ISOLATION" -Value (Read-ValueFromHost -Question "Container isolation mode" -DefaultValue "default" -Required)
    }

    if (Confirm -Question "Would you like to adjust container memory limits?") {
        Set-EnvFileVariable "MEM_LIMIT_SQL" -Value (Read-ValueFromHost -Question "SQL Server memory limit" -DefaultValue "4GB" -Required)
        Set-EnvFileVariable "MEM_LIMIT_SOLR" -Value (Read-ValueFromHost -Question "Solr memory limit" -DefaultValue "2GB" -Required)
        Set-EnvFileVariable "MEM_LIMIT_CM" -Value (Read-ValueFromHost -Question "CM Server memory limit" -DefaultValue "6GB" -Required)
        Set-EnvFileVariable "MEM_LIMIT_RENDERING" -Value (Read-ValueFromHost -Question "CM Server memory limit" -DefaultValue "2GB" -Required)
    }
    Pop-Location
}