# 🥇 MVP Site 2020

This is the new Sitecore MVP site - build against Sitecore 10.1 utillising the new .NET Core development experience.

🚧🚧🚧 This is a work in progress and as such will probably contain some errors, bugs etc that will _hopefully_ be fixed before go live 🚧🚧🚧

# 💗 Contributions

The Sitecore MVP site is an Open Source project and as such we welcome community contributions, though Sitecore has a _"No commitment"_ approach to this repository.  Please read the [Code of Conduct](./CODE_OF_CONDUCT.md) and [Contribution Guide](./CONTRIBUTING.md) before participating

# ✋ PreRequisites
- [.NET Core (>= v 3.1) and .NET Framework 4.8](https://dotnet.microsoft.com/download)
- [MKCert](https://github.com/FiloSottile/mkcert)
- Approx 40gb HD space

# 💻 Initial Setup

1. If your local IIS is listening on port 443, you'll need to stop it for now.
    ```
    iisreset /stop
    ```

2. Before you can run the solution, you will need to prepare the following
   for the Sitecore container environment:
   * A valid/trusted wildcard certificate for `*.sc.localhost`
   * Hosts file entries for
     * mvp-cd.sc.localhost
     * mvp-cm.sc.localhost
     * mvp-id.sc.localhost
     * mvp.sc.localhost
    
   * Required environment variable values in `.env` for the Sitecore instance
     * (Can be done once, then checked into source control.)

   See Sitecore Containers documentation for more information on these
   preparation steps. The provided `init.ps1` will take care of them,
   but **you should review its contents before running.**

   > You must use an elevated/Administrator Windows PowerShell 5.1 prompt for
   > this command, PowerShell 7 is not supported at this time.

    ```ps1
    .\init.ps1 -InitEnv -LicenseXmlPath "C:\path\to\license.xml" -AdminPassword "DesiredAdminPassword"
    ```

3. After completing this environment preparation, run the startup script
   from the solution root:
    ```ps1
    .\up.ps1
    ```

4. When prompted, log into Sitecore via your browser, and
   accept the device authorization.

5. Wait for the startup script to open browser tabs for the rendered site
   and Sitecore Launchpad.

## ⚠️ Troubleshooting

If have issues running the site locally, please refer to the [Troubleshooting page](https://github.com/Sitecore/MVP-Site/wiki/Troubleshooting) on the wiki before opening an issue.
