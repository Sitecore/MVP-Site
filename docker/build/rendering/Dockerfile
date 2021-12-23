# escape=`

# This is an example Dockerfile for an ASP.NET Core Rendering Host.
# We use build stages to enable 'dotnet watch' during development, so
# that changes to your rendering code can be quickly tested, including in
# the Experience Editor. Be sure to watch the container logs in case of build errors.

ARG DEBUG_BASE_IMAGE
ARG RELEASE_BASE_IMAGE
ARG SOLUTION_IMAGE

FROM ${DEBUG_BASE_IMAGE} as debug
ARG ENTRYPOINT_PROJECT_FOLDER

WORKDIR /solution/src

ENV ENTRYPOINT_PROJECT_PATH="Project/${ENTRYPOINT_PROJECT_FOLDER}/rendering"
RUN echo $env:ENTRYPOINT_PROJECT_PATH
ENTRYPOINT dotnet watch -v --project .\$env:ENTRYPOINT_PROJECT_PATH run --no-launch-profile

FROM ${SOLUTION_IMAGE} as solution
FROM ${RELEASE_BASE_IMAGE} as release

ARG ENTRYPOINT_ASSEMBLY_NAME
ARG ARTIFACTS_FOLDER=/artifacts/rendering/

ENV ENTRYPOINT_ASSEMBLY="${ENTRYPOINT_ASSEMBLY_NAME}.dll"

WORKDIR /app
COPY --from=solution ${ARTIFACTS_FOLDER} ./
EXPOSE 80
EXPOSE 443
ENTRYPOINT dotnet $env:ENTRYPOINT_ASSEMBLY