param(
    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [string]$Region = 'westus2',

    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [string]$ResourceGroup = 'MVP-Site-v2',

    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [string]$AksName = 'MVP-Site-v2',

    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [string]$AcrName = 'MVPSiteV2',

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
    --kubernetes-version $aksVersion `
    --location $Region `
    --windows-admin-password $AzureWindowsPassword `
    --windows-admin-username azureuser `
    --vm-set-type VirtualMachineScaleSets `
    --node-count 1 `
    --generate-ssh-keys `
    --network-plugin azure `
    --enable-addons monitoring `
    --node-resource-group "$($ResourceGroup)_AKS_BackEnd"
    --attach-acr $acrId
Write-Host "--- Complete: AKS Created ---" -ForegroundColor Green

# Add windows server nodepool
Write-Host "--- Creating Windows Server Node Pool ---" -ForegroundColor Blue
az aks nodepool add --resource-group $ResourceGroup `
    --cluster-name $AksName `
    --os-type Windows `
    --name npwin `
    --node-vm-size Standard_D8s_v3 `
    --node-count 1
Write-Host "--- Complete: Windows Server Node Pool Created ---" -ForegroundColor Green

# authenticate AKS instance
Write-Host "--- Authenticate with AKS ---" -ForegroundColor Blue
az aks get-credentials -a `
    --resource-group MVP-Site-v2 `
    --name MVP-Site-v2 `
    --overwrite-existing
Write-Host "--- Complete: Authenticated with AKS ---" -ForegroundColor Green