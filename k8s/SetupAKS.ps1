# To configure the Kubectl context cluster:
#Log in to the Azure CLI and set a subscription.
#az login
#az account show
#az account set --subscription 

#Get the credentials for the K8s cluster that were created with the AKS cluster and save them locally.
az aks get-credentials --resource-group Josl-MVP-Site-v2 --name Josl-MVP-Site-v2

#Add an NGINX ingress controller feed to Helm. For example:
helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx

# Update your local Helm chart repository cache
helm repo update

#Use Helm to deploy the NGINX ingress controller. For example:
helm install nginx-ingress ingress-nginx/ingress-nginx --set controller.replicaCount=1 --set controller.nodeSelector."kubernetes\.io/os"=linux --set defaultBackend.nodeSelector."kubernetes\.io/os"=linux --set-string controller.config.proxy-body-size=10m --set controller.service.externalTrafficPolicy=Local

# Install cert manager
kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.8.0/cert-manager.yaml

# Setup Authentication to pull images
az aks get-credentials --resource-group Josl-MVP-Site-v2 --name Josl-MVP-Site-v2