# escape=`

ARG BASE_IMAGE
ARG JSS_RESOURCES_IMAGE
ARG SPE_IMAGE

FROM ${JSS_RESOURCES_IMAGE} AS jss_resources
FROM ${SPE_IMAGE} as spe_resources
FROM ${BASE_IMAGE}

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

# Copy and init the JSS / Headless Services Module
COPY --from=jss_resources C:\module\db C:\jss_data
RUN C:\DeployDatabases.ps1 -ResourcesDirectory C:\jss_data; `
    Remove-Item -Path C:\jss_data -Recurse -Force;

# Add SPE Module
COPY --from=spe_resources C:\module\db C:\spe_data
RUN C:\DeployDatabases.ps1 -ResourcesDirectory C:\spe_data; `
    Remove-Item -Path C:\spe_data -Recurse -Force;