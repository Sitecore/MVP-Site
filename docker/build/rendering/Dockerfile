ARG SOLUTION_IMAGE
ARG NETCORE_RUNTIME_IMAGE

FROM ${SOLUTION_IMAGE} as solution

FROM ${NETCORE_RUNTIME_IMAGE} as runtime
WORKDIR /app

COPY --from=solution /artifacts/rendering/ ./

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "Mvp.Project.MvpSite.dll"]