# escape=`
ARG BASE_IMAGE
ARG HEADLESS_SERVICES_RESOURCES_IMAGE
ARG SPE_RESOURCES_IMAGE

FROM ${HEADLESS_SERVICES_RESOURCES_IMAGE} AS headless_services_resources
FROM ${SPE_RESOURCES_IMAGE} AS spe_resources
FROM $BASE_IMAGE as final

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

COPY --from=headless_services_resources C:\module\db c:\resources\headless_services_data
COPY --from=spe_resources C:\module\db c:\resources\spe_data