FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG VERSION
ARG TARGETOS
ARG TARGETARCH
WORKDIR /src
COPY ["Directory.Packages.props", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["NuGet.Config", "./"]
COPY ["src/Texnokaktus.ProgOlymp.YandexContestIntegrationService/Texnokaktus.ProgOlymp.YandexContestIntegrationService.csproj", "src/Texnokaktus.ProgOlymp.YandexContestIntegrationService/"]
RUN --mount=type=secret,id=github_credentials,required=true \
    NuGetPackageSourceCredentials_github="$(cat /run/secrets/github_credentials)" \
    dotnet restore "src/Texnokaktus.ProgOlymp.YandexContestIntegrationService/Texnokaktus.ProgOlymp.YandexContestIntegrationService.csproj" --configfile NuGet.Config --runtime $TARGETOS-$TARGETARCH
COPY . .
WORKDIR "/src/src/Texnokaktus.ProgOlymp.YandexContestIntegrationService"
RUN dotnet build "./Texnokaktus.ProgOlymp.YandexContestIntegrationService.csproj" --no-restore -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Texnokaktus.ProgOlymp.YandexContestIntegrationService.csproj" --no-restore -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:Version=$VERSION

FROM base AS final
WORKDIR /app
ENV SERVICE_NAME=YandexContestIntegrationService
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Texnokaktus.ProgOlymp.YandexContestIntegrationService.dll"]
