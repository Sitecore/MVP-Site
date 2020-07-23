[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$LicenseXmlPath,

    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$AzurePAT,

    [Parameter()]
    [switch]$RunWithoutTreafik
)

$ErrorActionPreference = "Stop";

if (-not (Test-Path $LicenseXmlPath))
{
    throw "Did not find $LicenseXmlPath."
}

if (Test-Path $LicenseXmlPath -PathType Leaf)
{
    # We want the folder that it's in for mounting
    $LicenseXmlPath = (Get-Item $LicenseXmlPath).Directory.FullName
}

# Override .env variables in current session
$env:HOST_LICENSE_FOLDER = $LicenseXmlPath
$env:AZURE_PAT = $AzurePAT

# Restore dotnet tool for sitecore login and serialization
dotnet tool restore

# Build all containers in the Sitecore instance, forcing a pull of latest base containers
docker-compose build
if ($LASTEXITCODE -ne 0)
{
    Write-Error "Container build failed, see errors above."
}

if ($RunWithoutTreafik)
{
    # Start container instances without traefik
    docker-compose up -d dotnetsdk solutionBuildOutput redis mssql solr id cd cm rendering 
}
else
{
    # Start all container 
    docker-compose up -d
}