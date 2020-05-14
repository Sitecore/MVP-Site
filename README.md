# MVP Site 2020
This is the new Sitecore MVP site - build against Sitecore 10 utillising the new .NET Core development experience.

# PreRequisites
- [.NET Core (>= v 3.1)](https://dotnet.microsoft.com/download)
- [MKCert](https://github.com/FiloSottile/mkcert)
- Approx 40gb HD space

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
    127.0.0.1 mvp-site.sc.localhost
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
    .\setup.ps1 -LicenseXmlPath "C:\path\to\license.xml"
    ```
    The `setup.ps1` script will do the following for you:
    * Launch a Nuget Server container with preview packages
    * Build the solution containers
    * Launch the solution containers  (The last container may take a minute to create/start, as it waits for CM and ID to be *healthy*.)

    The sites can take a few mins to come up. Check they're listed in Traefik by hitting http://localhost:8079/dashboard

6. Run `dotnet sitecore login` to have your CLI authenticated with the MVP Sitecore instance

7. Run `dotnet sitecore ser push` to push the content items to Sitecore

8. Run `dotnet sitecore publish` to publish the content items

9. Browse the the rendering host URL in your browser

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
