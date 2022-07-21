# escape=`

ARG BUILD_IMAGE
ARG NETCORE_BUILD_IMAGE

FROM ${NETCORE_BUILD_IMAGE} as netcore-sdk
FROM ${BUILD_IMAGE}

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

# Ensure updated nuget. Depending on your Windows version, dotnet/framework/sdk:4.8 tag may provide an outdated client.
# See https://github.com/microsoft/dotnet-framework-docker/blob/1c3dd6638c6b827b81ffb13386b924f6dcdee533/4.8/sdk/windowsservercore-ltsc2019/Dockerfile#L7
ENV NUGET_VERSION 5.8.0
RUN Invoke-WebRequest "https://dist.nuget.org/win-x86-commandline/v$env:NUGET_VERSION/nuget.exe" -UseBasicParsing -OutFile "$env:ProgramFiles\NuGet\nuget.exe"

## Install netcore onto SDK image
## https://github.com/dotnet/dotnet-docker/blob/5e9b849a900c69edfe78f6e0f3519009de4ab471/3.1/sdk/nanoserver-1909/amd64/Dockerfile

# Retrieve .NET Core SDK
COPY --from=netcore-sdk ["/Program Files/dotnet/", "/Program Files/dotnet/"]

# Enable detection of running in a container
# Enable correct mode for dotnet watch (only mode supported in a container)
# Skip extraction of XML docs - generally not useful within an image/container - helps performance

ENV DOTNET_RUNNING_IN_CONTAINER=true DOTNET_USE_POLLING_FILE_WATCHER=true NUGET_XMLDOC_MODE=skip 

RUN $path = ${Env:PATH} + ';C:\Program Files\dotnet\;'; setx /M PATH $path

# Trigger first run experience by running arbitrary cmd
RUN dotnet help | out-null