



# MVP Site 2020
This is the new Sitecore MVP site - build against Sitecore 10 utillising the new .NET Core development experience.

# PreRequisites
- [.NET Core (>= v 3.1) and .NET Framework 4.8](https://dotnet.microsoft.com/download)
- [MKCert](https://github.com/FiloSottile/mkcert)
- Approx 40gb HD space

### Azure Personal Access Token (Temporary step till launch)
Generate an Azure Personal Access Token (PAT) to authenticate with the private nuget feed, by following the steps [here](https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate)

# Initial Setup

1. If your local IIS is listening on port 443, you'll need to stop it for now.
    ```
    iisreset /stop
    ```

2. Add host file entries for the following
    ```
    127.0.0.1 mvp-cd.sc.localhost
    127.0.0.1 mvp-cm.sc.localhost
    127.0.0.1 mvp-id.sc.localhost
    127.0.0.1 mvp.sc.localhost
    ```

3. Create a wildcard cert for the solution. Run this PowerShell script from the solution root:
    ```ps1
    Push-Location docker\traefik\certs
    mkcert "*.sc.localhost"
    Pop-Location
    ```

4. Authenticate to the private Azure Container Repository. **DO NOT SHARE**
    ```ps1
    "7Z8HffSJJtz=pLdfMTXHLcLMn4WAgyH5" | docker login -u sitecore-mvps --password-stdin devexmvp.azurecr.io
    ```

5. Run `setup.ps1`, passing in your Sitecore license key as an argument. From the solution root:
    ```ps1
    .\setup.ps1 -LicenseXmlPath "C:\path\to\license.xml" -AzurePAT "<<INSERT_PAT>>"
    ```
    The `setup.ps1` script will do the following for you:
    * Build the solution containers
    * Launch the solution containers  (The last container may take a minute to create/start, as it waits for CM and ID to be *healthy*.)
    * Install the Sitecore CLI

    The sites can take a few mins to come up. Check they're listed in Traefik by hitting http://localhost:8079/dashboard

6. Run `dotnet sitecore login -a https://mvp-id.sc.localhost -h https://mvp-cm.sc.localhost --allow-write true` to have your CLI authenticated with the MVP Sitecore instance.
    NOTE: The first time you run you need to set the auth host and the CM host to set the environment, and the "allow-write" to make sure your user.json gets created so that serialization pushes will work.

7. Run `dotnet sitecore ser push` to push the content items to Sitecore

8. Run `dotnet sitecore publish` to publish the content items

 9. Browse the the rendering host URL in your browser (https://mvp.sc.localhost)

## Troubleshooting

A common Docker error encountered with the script is related to an error with [Windows containers accessing external networks](https://github.com/docker/for-win/issues/2760):
```
error: Unable to load the service index for source https://api.nuget.org/v3/index.json`
```
See [this issue comment](https://github.com/docker/for-win/issues/2760#issuecomment-430889666) for suggestions on resolving this issue.

If that fix doesn't work, then disabling all of your network devices except for your active one will also work (The nuclear option 😊)

# Running Side-by-side with the NewDevEx solution

Currently there is an issue attempting to run New Dev Ex & MVP solutions at the same time as the rendering hosts clash in Treafik and neither of them will resolve.

1. Configure & run the NewDevEx solution according to the instructions on that repo
2. Run the MVP site in same way as above, but include the -RunWithoutTreafik toggle.
3. Wait a couple of mins for Traefik to connect the new containers (Check they're listed in Traefik by hitting http://localhost:8079/dashboard)

# AKS

(These instructions are pretty high level, probably a good candidate to be expanded and moved to a GitHub Wiki?)

## Generate Certs

Setup `mkcert` as per instructions for local installation above. Generate certs for all 4 application instances using the following commands:

- `mkcert -cert-file k8s\specs\secrets\tls\mvp-cm\tls.crt -key-file k8s\specs\secrets\tls\mvp-cm\tls.key "cm.mvp"`
- `mkcert -cert-file k8s\specs\secrets\tls\mvp-cd\tls.crt -key-file k8s\specs\secrets\tls\mvp-cd\tls.key "cd.mvp"`
- `mkcert -cert-file k8s\specs\secrets\tls\mvp-id\tls.crt -key-file k8s\specs\secrets\tls\mvp-id\tls.key "id.mvp"`
- `mkcert -cert-file k8s\specs\secrets\tls\mvp-rendering\tls.crt -key-file k8s\specs\secrets\tls\mvp-rendering\tls.key "rendering.mvp"`

You then need to add them to you Machine -> Personal & Trusted Root certificate stores.

## Creating an AKS Instance

There is a script to create and AKS instance with the required windows node pool, to perform this action you can call

1. `az login`
2. `az account set --subscription "<<CHOSEN_SUBSCRIPTION>>"`
3. `./k8s/CreateAKS.ps1 -AzureWindowsPassword "<<CHOSEN_PASSWORD>>"` (Note, there are other params you can update to change from the default values - this will take 10-15 mins to complete)

## Starting K8s Dashboard

Once the AKS instance is up and running you can start the K8s dashboard pointing to the AKS instance using the following command

`az aks browse --resource-group MVP-Site-v2 --name MVP-Site-v2`

(If you changed from default values when creating AKS instance this command will need to be changed accordingly.)

## Deploying MVP Site to AKS

### Choose namespace (staging or prod), create and set context

- `kubectl apply -f .\k8s\specs\namespaces\namespace-staging.yaml`
- `kubectl config set-context --current --namespace=mvp-staging`
or
- `kubectl apply -f .\k8s\specs\namespaces\namespace-prod.yaml`
- `kubectl config set-context --current --namespace=mvp-prod`

### Deploy Private Registry Secrets
Our Images are stored in a private registry so we need to authenticate our AKS instance with that private registry. Details of how this is achieved can be seen [here](https://kubernetes.io/docs/tasks/configure-pod-container/pull-image-private-registry/).

Ensure your authentication details are stored in your docker config by following this [GitHub issue](https://github.com/docker/for-mac/issues/4100). - You will need to re-authenticate with the registry after performing this action.

Once all of that is setup, you can run the following command to push your auth to AKS as a secret:
- `kubectl create secret generic regcred --from-file=.dockerconfigjson="<<PATH_TO_DOCKER_CONFIG>>" --type=kubernetes.io/dockerconfigjson`

### Configuring Helm
You need to setup the Helm ServiceAccount & deploy tiller by running the following commands:

- `kubectl create serviceaccount --namespace kube-system tiller`
- `kubectl create clusterrolebinding tiller-cluster-rule --clusterrole=cluster-admin --serviceaccount=kube-system:tiller`  
- `helm init --service-account tiller --node-selectors "beta.kubernetes.io/os=linux" `
- `kubectl patch deploy --namespace kube-system tiller-deploy -p '{\"spec\":{\"template\":{\"spec\":{\"serviceAccount\":\"tiller\"}}}}'`

(You can verify this has been actioned successfully in the K8s Dashboard by changing to the `kube-system` namespace and ensuring that the `tiller-deploy` deployment is green)

### Deploy Ingress

You can install the NGINX Ingress using the following commands.

- `helm repo add stable https://kubernetes-charts.storage.googleapis.com/`
- `kubectl create namespace ingress-basic`
- `helm install --name nginx-ingress stable/nginx-ingress --namespace ingress-basic --set controller.replicaCount=2 --set controller.nodeSelector."beta\.kubernetes\.io/os=linux" --set defaultBackend.nodeSelector."beta\.kubernetes\.io/os=linux" --set-string controller.config.proxy-body-size=10m`

(You can verify this is correct in the K8s dashboard by changing to the `ingress-basic` namespace and checking that the two deployments (`nginx-ingress-controller` & `nginx-ingress-default-backend`) are both green.

### Deploy Secrets
The secrets are not included in this repo, extract the secrets from the official k8s specification download and drop them into the `/k8s/specs/secrets` folder. Ensure they are all populated with the correct values, the run the following command to push all of the secrets into AKS.

`kubectl apply -k .\k8s\specs\secrets\`

### Deploy External Services (Non production only)
Data storage containers (SQL, SOLR, Redis) are only supported in Non-Production. To install these containers run the following command:

`kubectl apply -f .\k8s\specs\external\`

(Wait for all deployments to show 'green' in the dashboard - this can take a while!)

### Deploy Sitecore application instances

Deploy the Sitecore application instances using the following command.

`kubectl apply -f .\k8s\specs\`

(Wait for all deployments to show 'green' in the dashboard - this can take a while!)

### Update local hosts file
Finally we ca get the external IP assigned by the ingress with the following command

`kubectl get service -l app=nginx-ingress --namespace ingress-basic`

Update your hosts file for the external IP for the following Host  names

- <<EXTERNAL_IP>> cm.mvp
- <<EXTERNAL_IP>> cd.mvp
- <<EXTERNAL_IP>> id.mvp
- <<EXTERNAL_IP>> rendering.mvp

### Serialisation 
- Run `dotnet sitecore login -a https://id.mvp -h https://cm.mvp --allow-write true` to authenticate your CLI with Id Server
- Run `dotnet sitecore ser push` to push the content items to Sitecore
- Run `dotnet sitecore publish` to publish the content items
- Browse the the rendering host URL in your browser (https://rendering.mvp)