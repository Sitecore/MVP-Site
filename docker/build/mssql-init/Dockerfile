# escape=`
ARG BASE_IMAGE
ARG HEADLESS_SERVICES_RESOURCES_IMAGE

FROM ${HEADLESS_SERVICES_RESOURCES_IMAGE} AS headless_services_resources
FROM $BASE_IMAGE as final

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

COPY --from=headless_services_resources C:\module\db c:\resources\jss_data

ENTRYPOINT .\DeployDatabases.ps1 -ResourcesDirectory c:\resources\jss_data -SqlServer $env:SQL_SERVER -SqlAdminUser $env:SQL_ADMIN_LOGIN -SqlAdminPassword $env:SQL_ADMIN_PASSWORD -SkipStartingServer