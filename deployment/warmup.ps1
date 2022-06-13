[CmdletBinding()]
param (
    [Parameter(Mandatory=$False, HelpMessage="Base URL to visit pages from.")]    
    [Alias("dn")]
    [string]$DomainName=$($env:STAGING_CM_HOST),
		
    [Parameter(Mandatory=$False, HelpMessage="Username if a login is required.")]
    [Alias("u")]
    [string]$UserName=$($env:STAGING_APPLICATION_USER_NAME), 

    [Parameter(Mandatory=$False, HelpMessage="Password if a login is required.")]
    [Alias("p")]
    [string]$Password=$($env:STAGING_ADMIN_PASSWORD)
)

##########################################################################

Write-Host "Getting Login Page $DomainName"
Write-Host "Getting username $UserName"
Write-Host "Getting password $Password"
$config = Get-Content -Raw -Path "$($PSScriptRoot)\warmup-config.json" | ConvertFrom-Json

##########################################################################
# FUNCTIONS
##########################################################################

Function Get-AuthenticatedSession {
  param(
    [Parameter(Mandatory=$true,Position=0)]
    [string]$domainName,
    [Parameter(Mandatory=$true,Position=1)]
    [object]$authenticationDetails,
    [Parameter(Mandatory=$true,Position=2)]
    [string]$username,
    [Parameter(Mandatory=$true,Position=3)]
    [string]$password
  )

  # Login - to create web session with authorisation cookies
  $loginPage = "$domainName$($authenticationDetails.url)"

  Write-Host "Getting Login Page $loginPage"

  try{
    $login = Invoke-WebRequest $loginPage -SessionVariable webSession -TimeoutSec 600
  }catch{
    Write-Host "First attempt failed with $($_.Exception.Response.StatusCode.value__) , retrying"
    $login = Invoke-WebRequest $loginPage -SessionVariable webSession -TimeoutSec 600
  }
  
  Write-Host "Got Login Page, filling out form"

  $form = $login.forms[0]
  $form.fields["$($authenticationDetails.userNameField)"] = $username
  $form.fields["$($authenticationDetails.passwordField)"] = $password
   
  Write-Host "logging in"
  
  $request = Invoke-WebRequest -Uri $loginPage -WebSession $webSession -Method POST -Body $form  -TimeoutSec 600 | Out-Null
  
  $webSession
  
  Write-Host "login done"
}

Function RequestPage {
	param(
		[Parameter(Mandatory=$true,Position=0)]
		[string]$url,
		[Parameter(Mandatory=$true,Position=1)]
		[object]$webSession
	)
	Get-Date
	Write-Host "requesting $url ..."
	try { $request = Invoke-WebRequest $url -WebSession $webSession -TimeoutSec 60000 } catch {
      $status = $_.Exception.Response.StatusCode.Value__
	  if ($status -ne 200){
		Write-Host "ERROR Something went wrong while requesting $url" -foregroundcolor red
	  }
	}
	
    Write-Host $request

	Get-Date
	Write-Host "Done"
	Write-Host ""
}

##########################################################################

$session = Get-AuthenticatedSession $DomainName $config.authenticationDetails $UserName $Password

foreach ($page in $config.urls) {
	RequestPage "$DomainName$($page.url)" $session
}

##########################################################################