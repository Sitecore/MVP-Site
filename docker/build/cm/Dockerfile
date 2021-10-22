# escape=`

ARG BASE_IMAGE
ARG SOLUTION_IMAGE
ARG TOOLS_RESOURCES_IMAGE
ARG DEVEX_RESOURCES_IMAGE
ARG JSS_RESOURCES_IMAGE
ARG SPE_IMAGE

FROM ${SOLUTION_IMAGE} as solution
FROM ${TOOLS_RESOURCES_IMAGE} as tooling
FROM ${DEVEX_RESOURCES_IMAGE} AS devex_resources
FROM ${JSS_RESOURCES_IMAGE} AS jss_resources
FROM ${SPE_IMAGE} as spe_resources
FROM ${BASE_IMAGE}

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

WORKDIR C:\inetpub\wwwroot

# Copy developer tools and entrypoint
COPY --from=tooling C:\tools C:\tools

# Copy the Sitecore Management Services Module
COPY --from=devex_resources C:\module\cm\content C:\inetpub\wwwroot

# Copy and init the JSS / Headless Services Module
COPY --from=jss_resources C:\module\cm\content C:\inetpub\wwwroot
COPY --from=jss_resources C:\module\tools C:\module\tools
RUN C:\module\tools\Initialize-Content.ps1 -TargetPath C:\inetpub\wwwroot; `
    Remove-Item -Path C:\module -Recurse -Force;

# Copy solution website files
COPY --from=solution /artifacts/sitecore/ ./

# Add SPE Module
COPY --from=spe_resources C:\module\cm\content C:\inetpub\wwwroot