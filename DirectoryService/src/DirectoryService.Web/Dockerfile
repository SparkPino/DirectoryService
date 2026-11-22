ARG BUILD_CONFIGURATION=Release
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
# EXPOSE 8081 for https

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION
WORKDIR /src
COPY ["src/DirectoryService.Web/DirectoryService.Web.csproj", "src/DirectoryService.Web/"]
COPY ["src/DirectoryService.Application/DirectoryService.Application.csproj", "src/DirectoryService.Application/"]
COPY ["src/DirectoryService.Contracts/DirectoryService.Contracts.csproj", "src/DirectoryService.Contracts/"]
COPY ["src/DirectoryService.Domain/DirectoryService.Domain.csproj", "src/DirectoryService.Domain/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
COPY ["src/DirectoryService.Infrastructure.Postgres/DirectoryService.Infrastructure.Postgres.csproj", "src/DirectoryService.Infrastructure.Postgres/"]
COPY ["src/DirectoryService.Infrastructure/DirectoryService.Infrastructure.csproj", "src/DirectoryService.Infrastructure/"]
COPY ["src/DirectoryService.Presenters/DirectoryService.Presenters.csproj", "src/DirectoryService.Presenters/"]
COPY ["nuget.config", "Directory.Build.props", "./"]
 
RUN --mount=type=secret,id=github_token,required=true \
    NuGetPackageSourceCredentials_github="Username=SparkPino;Password=$(cat /run/secrets/github_token)" \
    dotnet  restore "src/DirectoryService.Web/DirectoryService.Web.csproj"
COPY . .
WORKDIR "/src/src/DirectoryService.Web"
RUN dotnet build "./DirectoryService.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build  --no-restore

FROM build AS bundle
ARG BUILD_CONFIGURATION
WORKDIR /src
RUN dotnet tool restore
RUN dotnet ef migrations bundle  \
    --project  src/DirectoryService.Infrastructure.Postgres  \
    --startup-project src/DirectoryService.Web \
    --output  /app/efbundle \
    --configuration  $BUILD_CONFIGURATION 
    

FROM base AS migrator
WORKDIR /app
COPY --from=bundle /app/efbundle .
ENTRYPOINT ["./efbundle"]

FROM build AS publish
ARG BUILD_CONFIGURATION
RUN dotnet publish "./DirectoryService.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false  --no-restore

FROM base AS final
USER root
RUN apt-get update && \
    apt-get install -y --no-install-recommends curl && \
    rm -rf /var/lib/apt/lists/* 
USER $APP_UID
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DirectoryService.Web.dll"]
