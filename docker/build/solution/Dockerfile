# escape=`

# This Dockerfile will build the Sitecore solution and save the build artifacts
# for use in other images, such as 'cm' and 'rendering'. It does not produce a
# runnable image itself.

ARG BASE_IMAGE
ARG BUILD_IMAGE

FROM ${BUILD_IMAGE} AS nuget-prep
# Gather only artifacts necessary for NuGet restore, retaining directory structure
COPY *.sln nuget.config /nuget/
COPY src/ /temp/
RUN Invoke-Expression 'robocopy C:/temp C:/nuget/src /s /ndl /njh /njs *.csproj *.scproj packages.config'

FROM ${BUILD_IMAGE} AS builder
ARG BUILD_CONFIGURATION
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

# Create an empty working directory
WORKDIR /build

# Copy prepped NuGet artifacts, and restore as distinct layer to take better advantage of caching
COPY --from=nuget-prep ./nuget ./

# Install Azure Credential Provider
RUN Invoke-WebRequest 'https://raw.githubusercontent.com/microsoft/artifacts-credprovider/master/helpers/installcredprovider.ps1' -OutFile installcredprovider.ps1;
RUN .\installcredprovider.ps1 -Force 
RUN Remove-Item .\installcredprovider.ps1;

# Configure VSS Nuget endpoint
ARG NUGET_PREVIEW_SOURCE
ARG AZURE_PAT
ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS='{ "endpointCredentials": [{"endpoint": "'$NUGET_PREVIEW_SOURCE'", "username":"NA", "password":"'$AZURE_PAT'"}]}'

# Restore NuGet packages
RUN dotnet restore

# Copy remaining source code
COPY src/ ./src/

# Publish the Sitecore & Identity Artifacts
RUN msbuild .\src\Environment\sitecore\Mvp.Environment.Sitecore.csproj /p:Configuration=Release /restore /m /p:DeployOnBuild=true /p:PublishProfile=Local
RUN msbuild .\src\Environment\identity\Mvp.Environment.Identity.csproj /p:Configuration=Release /restore /m /p:DeployOnBuild=true /p:PublishProfile=Local

# Publish the Rendering Host Artifacts
WORKDIR .\src\Project\MvpSite\rendering\
RUN dotnet publish -c Release -o /build/rendering

#WORKDIR /build/src/Project/MvpSite/rendering/
#RUN dotnet publish -c Release -o /build/rendering --no-restore

# Start new Stage
FROM ${BASE_IMAGE}
WORKDIR /artifacts

# Copy in all artifacts for sitecore, identity & rendering host
COPY --from=builder /build/docker/deploy/sitecore ./sitecore/
COPY --from=builder /build/docker/deploy/identity/Config ./identity/Config
COPY --from=builder /build/rendering ./rendering/