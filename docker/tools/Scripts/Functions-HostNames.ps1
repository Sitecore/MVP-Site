#Requires -RunAsAdministrator

Set-StrictMode -Version Latest

function Initialize-HostNames {
    param (
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] 
        $HostDomain
    )
    Write-Host "Adding hosts file entries..." -ForegroundColor Green

    Add-HostsEntry "cm.$($HostDomain)"
    Add-HostsEntry "cd.$($HostDomain)"
    Add-HostsEntry "id.$($HostDomain)"
    
    if (!(Test-Path ".\docker\traefik\certs\cert.pem")) {
        Add-SelfSignedCertificate -FullHostName $hostDomain
    }
}

function Add-SelfSignedCertificate { 
    Param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] 
        $FullHostName,
        $CertificatesVolumeFolder = ".\docker\traefik\certs"
    )
    Push-Location $CertificatesVolumeFolder
    try {
        $mkcert = ".\mkcert.exe"
        if ($null -ne (Get-Command mkcert.exe -ErrorAction SilentlyContinue)) {
            $mkcert = "mkcert"
        }
        elseif (-not (Test-Path $mkcert)) {
            Write-Host "Downloading and installing mkcert certificate tool..." -ForegroundColor Green 
            Invoke-WebRequest "https://github.com/FiloSottile/mkcert/releases/download/v1.4.1/mkcert-v1.4.1-windows-amd64.exe" -UseBasicParsing -OutFile mkcert.exe
            if ((Get-FileHash mkcert.exe).Hash -ne "1BE92F598145F61CA67DD9F5C687DFEC17953548D013715FF54067B34D7C3246") {
                Remove-Item mkcert.exe -Force
                throw "Invalid mkcert.exe file"
            }
        }
        Write-Host "Generating Traefik TLS certificate..." -ForegroundColor Green
        & $mkcert -install
        & $mkcert -key-file key.pem -cert-file cert.pem "*.$($FullHostName)"
    }
    catch {
        Write-Host "An error occurred while attempting to generate TLS certificate: $_" -ForegroundColor Red
    }
    finally {
        Pop-Location
    }
}





