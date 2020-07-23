# escape=`

# This is an example Dockerfile for an ASP.NET Core Rendering Host.
# We use build stages to enable 'dotnet watch' during development, so
# that changes to your rendering code can be quickly tested, including in
# the Experience Editor. Be sure to watch the container logs in case of build errors.

ARG DEBUG_BASE_IMAGE
ARG RELEASE_BASE_IMAGE
ARG SOLUTION_IMAGE

FROM ${DEBUG_BASE_IMAGE} as debug

# Install Azure Credential Provider
RUN Invoke-WebRequest 'https://raw.githubusercontent.com/microsoft/artifacts-credprovider/master/helpers/installcredprovider.ps1' -OutFile installcredprovider.ps1;
RUN .\installcredprovider.ps1 -Force 
RUN Remove-Item .\installcredprovider.ps1;

# Configure VSS Nuget endpoint
ARG NUGET_PREVIEW_SOURCE
ARG AZURE_PAT
ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS='{ "endpointCredentials": [{"endpoint": "'$NUGET_PREVIEW_SOURCE'", "username":"NA", "password":"'$AZURE_PAT'"}]}'

WORKDIR /solution/src
ENTRYPOINT ["dotnet", "watch", "-v", "--project", ".\\Project\\MvpSite\\rendering", "run", "--no-launch-profile"]

FROM ${SOLUTION_IMAGE} as solution
FROM ${RELEASE_BASE_IMAGE} as release
WORKDIR /app
COPY --from=solution /artifacts/rendering/ ./
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Mvp.Project.MvpSite.dll"]