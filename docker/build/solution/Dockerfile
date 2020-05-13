# escape=`

ARG BASE_IMAGE
ARG BUILD_IMAGE
ARG NETCORE_BUILD_IMAGE

FROM ${NETCORE_BUILD_IMAGE} as netcore-sdk

FROM ${BUILD_IMAGE} AS nuget-prep
# Gather only artifacts necessary for NuGet restore, retaining directory structure
COPY *.sln nuget.config /nuget/
COPY src/ /temp/
RUN Invoke-Expression 'robocopy C:/temp C:/nuget/src /s /ndl /njh /njs *.csproj *.scproj packages.config'

FROM ${BUILD_IMAGE} AS builder
ARG NETCORE_VERSION
ARG NUGET_PREVIEW_SOURCE
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

## Install netcore onto SDK image
## https://github.com/dotnet/dotnet-docker/blob/5e9b849a900c69edfe78f6e0f3519009de4ab471/3.1/sdk/nanoserver-1909/amd64/Dockerfile

# Retrieve .NET Core SDK
COPY --from=netcore-sdk ["/Program Files/dotnet/", "/Program Files/dotnet/"]

ENV `
    # Enable detection of running in a container
    DOTNET_RUNNING_IN_CONTAINER=true `
    # Enable correct mode for dotnet watch (only mode supported in a container)
    DOTNET_USE_POLLING_FILE_WATCHER=true `
    # Skip extraction of XML docs - generally not useful within an image/container - helps performance
    NUGET_XMLDOC_MODE=skip 

RUN $path = ${Env:PATH} + ';C:\Program Files\dotnet\;'; `
    setx /M PATH $path

# Trigger first run experience by running arbitrary cmd
RUN dotnet help | out-null

# Create an empty working directory
WORKDIR /build

# Copy prepped NuGet artifacts, and restore as distinct layer to take better advantage of caching
COPY --from=nuget-prep ./nuget ./

### Update Nuget source from environment variable (the running nuget preview container)
RUN dotnet nuget update source SitecorePreview -s $env:NUGET_PREVIEW_SOURCE

# Restore NuGet packages
RUN dotnet restore

# Copy remaining source code
COPY src/ ./src/

# Build the Sitecore main platform artifacts
RUN msbuild .\src\Environment\sitecore\Mvp.Environment.Sitecore.csproj /p:Configuration=Release /restore /m /p:DeployOnBuild=true /p:PublishProfile=Local

# Build the rendering host
WORKDIR .\src\Project\MvpSite\rendering\
RUN dotnet publish -c Release -o /build/rendering

FROM ${BASE_IMAGE}

WORKDIR /artifacts
COPY --from=builder /build/docker/deploy  ./sitecore/
COPY --from=builder /build/rendering ./rendering/