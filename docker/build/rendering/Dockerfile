# escape=`

ARG DEBUG_BASE_IMAGE
ARG RELEASE_BASE_IMAGE
ARG SOLUTION_IMAGE

FROM ${SOLUTION_IMAGE} as solution

# Debug image (sdk image)
FROM ${DEBUG_BASE_IMAGE} as debug
ARG ARTIFACTS_FOLDER=/artifacts/rendering/

COPY ./Development.ps1 ./tools/
WORKDIR /app
COPY --from=solution ${ARTIFACTS_FOLDER} ./
ENV ASPNETCORE_URLS "http://+:80"
EXPOSE 80
ENTRYPOINT ["pwsh", "-File", "c:\\tools\\Development.ps1"]


# release image (aspnet)
FROM ${RELEASE_BASE_IMAGE} as release
ARG ENTRYPOINT_ASSEMBLY
ARG ARTIFACTS_FOLDER=/artifacts/rendering/

ENV ENTRYPOINT_ASSEMBLY="${ENTRYPOINT_ASSEMBLY}"
WORKDIR /app
COPY --from=solution ${ARTIFACTS_FOLDER} ./
EXPOSE 80
EXPOSE 443
#ENTRYPOINT dotnet %ENTRYPOINT_ASSEMBLY%
ENTRYPOINT ["dotnet", "${%ENTRYPOINT_ASSEMBLY%}"]
#ENTRYPOINT ["dotnet", "Mvp.Project.MvpSite.dll"]