[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$LicenseXmlPath,

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

# Start up the preview nuget server. You wouldn't have this in a "real" solution.
docker container ps -q -f name=sitecore-nuget-preview | ForEach-Object { docker container rm --force $_ }
docker container run -d -p 8010:80 --name sitecore-nuget-preview --network nat --rm devexmvp.azurecr.io/sitecore-nuget-preview:latest

# Restore dotnet tool for sitecore login and serialization
dotnet tool restore

# Build all containers in the Sitecore instance, forcing a pull of latest base containers
docker-compose build
if ($LASTEXITCODE -ne 0)
{
    Write-Error "Container build failed, see errors above."
}

# Override .env variables in current session
$env:HOST_LICENSE_FOLDER = $LicenseXmlPath

if ($RunWithoutTreafik)
{
    # Start container instances without traefik
    docker-compose up -d solutionBuildOutput redis mssql solr id cd cm rendering
}
else
{
    # Start all container 
    docker-compose up -d
}