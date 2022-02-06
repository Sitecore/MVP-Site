# escape=`

# This is an example Dockerfile for an ASP.NET Core Rendering Host.
# We use build stages to enable 'dotnet watch' during development, so
# that changes to your rendering code can be quickly tested, including in
# the Experience Editor. Be sure to watch the container logs in case of build errors.

ARG DEBUG_BASE_IMAGE
ARG RELEASE_BASE_IMAGE
ARG SOLUTION_IMAGE

FROM ${SOLUTION_IMAGE} as solution
FROM ${DEBUG_BASE_IMAGE} as debug
ARG ARTIFACTS_FOLDER=/artifacts/rendering/

COPY ./Development.ps1 ./
WORKDIR /app
COPY --from=solution ${ARTIFACTS_FOLDER} ./
ENV ASPNETCORE_URLS "http://+:80"
EXPOSE 80
ENTRYPOINT ["pwsh", "-File", "c:\\Development.ps1"]


FROM ${RELEASE_BASE_IMAGE} as release
ARG ENTRYPOINT_ASSEMBLY
ARG ARTIFACTS_FOLDER=/artifacts/rendering/

ENV ENTRYPOINT_ASSEMBLY="${ENTRYPOINT_ASSEMBLY}"
WORKDIR /app
COPY --from=solution ${ARTIFACTS_FOLDER} ./
EXPOSE 80
EXPOSE 443
ENTRYPOINT dotnet %ENTRYPOINT_ASSEMBLY%