# escape=`

ARG BASE_IMAGE
ARG SOLUTION_IMAGE
ARG TOOLS_RESOURCES_IMAGE
ARG DEVEX_RESOURCES_IMAGE
ARG HEADLESS_SERVICES_RESOURCES_IMAGE
ARG SPE_RESOURCES_IMAGE

FROM ${SOLUTION_IMAGE} as solution
FROM ${TOOLS_RESOURCES_IMAGE} as tooling
FROM ${DEVEX_RESOURCES_IMAGE} AS devex_resources
FROM ${HEADLESS_SERVICES_RESOURCES_IMAGE} AS headless_services_resources
FROM ${SPE_RESOURCES_IMAGE} AS spe_resources
FROM ${BASE_IMAGE}

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

WORKDIR C:\inetpub\wwwroot

# Copy developer tools and entrypoint
COPY --from=tooling C:\tools C:\tools

# Copy the Sitecore Management Services Module
COPY --from=devex_resources C:\module\cm\content C:\inetpub\wwwroot

# Copy and init the JSS / Headless Services Module
COPY --from=headless_services_resources C:\module\cm\content C:\inetpub\wwwroot
COPY --from=headless_services_resources C:\module\tools C:\module\tools
RUN C:\module\tools\Initialize-Content.ps1 -TargetPath C:\inetpub\wwwroot; `
    Remove-Item -Path C:\module -Recurse -Force;

# Copy SPE assets
COPY --from=spe_resources C:\module\cm\content C:\inetpub\wwwroot

# Copy solution website files
COPY --from=solution /artifacts/platform/ ./