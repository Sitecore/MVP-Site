# ⚠️⚠️⚠️ Warning

This repository is just a demonstration on how to build various things.

The actual code for the [Sitecore MVP site](https://mvp.sitecore.com), for [SUGCON](https://www.sugcon.events/), for [SUGCON Europe](https://europe.sugcon.events/) and for [SUGCON Australia and New Zealand](https://anz.sugcon.events/) is [here](https://github.com/Sitecore/XM-Cloud-Introduction)

# 🥇 Old Sitecore MVP Program Site

This is the repository for the old [Sitecore MVP site](https://mvp.sitecore.com). It was built using Sitecore 10.2.

# 💗 Contributions

The Sitecore MVP site is an Open Source project and as such we welcome community contributions, though Sitecore has a _"No commitment"_ approach to this repository.  Please read the [Code of Conduct](./CODE_OF_CONDUCT.md) and [Contribution Guide](./CONTRIBUTING.md) before participating

# ✋ PreRequisites
- [.NET Core (>= v 3.1) and .NET Framework 4.8](https://dotnet.microsoft.com/download)
- Approx 40gb HD space
- [Okta Developer Account](https://developer.okta.com/signup/)
- Valid Sitecore license
# 💻 Initial Setup

1. 🏃‍♂️ Run the Start-Environment script from an _elevated_ PowerShell terminal 

    ```ps1
    .\Start-Environment -LicensePath "C:\path\to\license.xml"
    ```
   _Note:_  The LicensePath argument only has to be used on the initial run of the script. The license file must be named `license.xml`, the script copies it to the folder `.\docker\license` where it also can be placed or updated manually.  

   > You **must** use an elevated/Administrator PowerShell terminal  
   > [Windows Terminal](https://github.com/microsoft/terminal/releases) looks best but the built-in Windows Powershell 5.1 terminal works too.

2. ☕ Follow the on screen instructions.  

   _Note:_ that you will be asked to fill in the following values with your Okta developer account details:
      - OKTA_DOMAIN (*must* include protocol, e.g. `OKTA_DOMAIN=https://dev-your-id.okta.com`)
      - OKTA_CLIENT_ID
      - OKTA_CLIENT_SECRET  
   [Sign up for an Okta Developer Account](https://developer.okta.com/signup/)

   _If the wizard is aborted prematurely or if the container build fails then use the `-InitializeEnvFile` switch to re-run the full wizard._

    ```ps1
    .\Start-Environment.ps1 -InitializeEnvFile
    ```  

3. 🔑 When prompted, log into Sitecore via your browser, and accept the device authorization.  

4. 🚀 Wait for the startup script to open a browser tab with the Sitecore Launchpad.  

5. 🛑 To Stop the environment again  
   
   ```ps1
   .\Stop-Environment.ps1
   ```  

### 🎭 Site switches

If you only want to start either the MVP or the SUGCON rendering container(s), you can use one of the following switch args  
* `-StartSugconSites` 
* `-StartMvpSite`  

If none of these are passed to the script all rendering containers are started.

_Example:_

```ps1
.\Start-Environment -StartSugconSites
```  

## ⚠️ Troubleshooting

If have issues running the site locally, please refer to the [Troubleshooting page](https://github.com/Sitecore/MVP-Site/wiki/Troubleshooting) on the wiki before opening an issue.
