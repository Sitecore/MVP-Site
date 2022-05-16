param(
    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [string]$Region = 'uksouth',

    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [string]$ResourceGroup = 'Josl-MVP-Site-v2',

    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [string]$AksName = 'Josl-MVP-Site-v2',

    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [string]$AcrName = 'JoslMVPSiteV2',

    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$AzureWindowsPassword
)

# Setup CLI & Parameters for AKS creation
Write-Host "--- Setting up CLI & Params ---" -ForegroundColor Blue
az extension add --name aks-preview
$aksVersion = $(az aks get-versions -l $Region --query 'orchestrators[-1].orchestratorVersion' -o tsv)
Write-Host "--- Complete: CLI & Params Configured ---" -ForegroundColor Green

# create AKS instance
Write-Host "--- Creating AKS Instance ---" -ForegroundColor Blue
az aks create --resource-group $ResourceGroup `
    --name $AksName `
    --kubernetes-version 1.21.9 `
    --location $Region `
    --windows-admin-password $AzureWindowsPassword `
    --windows-admin-username azureuser `
    --vm-set-type VirtualMachineScaleSets `
    --node-count 2 `
    --generate-ssh-keys `
    --network-plugin azure `
    --enable-addons monitoring `
    --node-resource-group "$($ResourceGroup)_AKS_BackEnd" `
    --attach-acr $AcrName
Write-Host "--- Complete: AKS Created ---" -ForegroundColor Green

# Add windows server nodepool
Write-Host "--- Creating Windows Server Node Pool ---" -ForegroundColor Blue
az aks nodepool add --resource-group $ResourceGroup `
    --cluster-name $AksName `
    --os-type Windows `
    --name npwin `
    --node-vm-size Standard_DS2_v2 `
    --node-count 3
Write-Host "--- Complete: Windows Server Node Pool Created ---" -ForegroundColor Green

# authenticate AKS instance
Write-Host "--- Authenticate with AKS ---" -ForegroundColor Blue
az aks get-credentials -a `
    --resource-group Josl-MVP-Site-v2 `
    --name Josl-MVP-Site-v2 `
    --overwrite-existing
Write-Host "--- Complete: Authenticated with AKS ---" -ForegroundColor Green