

# Setup Parameters for AKS creation
Write-Host "--- Setting up Params ---" -ForegroundColor Blue
$region = 'westus2'
$resourceGroup = 'MVP-Site-v2'
$aksName = 'MVP-Site-v2'
$acrName = 'MVPSiteV2'
$aksVersion = $(az aks get-versions -l westus2 --query 'orchestrators[-1].orchestratorVersion' -o tsv)
Write-Host "--- Complete: Params Configured ---" -ForegroundColor Green


# create AKS instance
Write-Host "--- Creating AKS Instance ---" -ForegroundColor Blue
az aks create --resource-group $resourceGroup --name $aksName --kubernetes-version $aksVersion --location $region --generate-ssh-keys --enable-addons monitoring,kube-dashboard
Write-Host "--- Complete: AKS Created ---" -ForegroundColor Green


# link AKS to ACR
Write-Host "--- Linking AKS to ACR ---" -ForegroundColor Blue
$clientID = $(az aks show --resource-group $resourceGroup --name $aksName --query "servicePrincipalProfile.clientId" --output tsv)
$acrId = $(az acr show --name $acrName --resource-group $resourceGroup --query "id" --output tsv)
az role assignment create --assignee $clientID --role acrpull --scope $acrId
Write-Host "--- Complete: AKS & ACR Linked ---" -ForegroundColor Green