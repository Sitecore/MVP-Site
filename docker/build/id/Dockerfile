# escape=`

ARG BASE_IMAGE
ARG SOLUTION_IMAGE

FROM ${SOLUTION_IMAGE} as solution
FROM ${BASE_IMAGE}

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

WORKDIR C:\Identity

# Copy identity artifacts
COPY --from=solution /artifacts/identity/ ./

ENTRYPOINT ["dotnet", "Sitecore.IdentityServer.Host.dll"]