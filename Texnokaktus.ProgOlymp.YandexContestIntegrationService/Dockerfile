FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG GHPAT
ARG VERSION
WORKDIR /src
COPY ["Texnokaktus.ProgOlymp.YandexContestIntegrationService/Texnokaktus.ProgOlymp.YandexContestIntegrationService.csproj", "Texnokaktus.ProgOlymp.YandexContestIntegrationService/"]
COPY ["Texnokaktus.ProgOlymp.Identity/Texnokaktus.ProgOlymp.Identity.csproj", "Texnokaktus.ProgOlymp.Identity/"]
COPY ["Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess/Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.csproj", "Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess/"]
COPY ["Texnokaktus.ProgOlymp.YandexContestIntegrationService.Domain/Texnokaktus.ProgOlymp.YandexContestIntegrationService.Domain.csproj", "Texnokaktus.ProgOlymp.YandexContestIntegrationService.Domain/"]
COPY ["Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic/Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic.csproj", "Texnokaktus.ProgOlymp.YandexContestIntegrationService.Logic/"]
COPY ["Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient/Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient.csproj", "Texnokaktus.ProgOlymp.YandexContestIntegrationService.YandexClient/"]
COPY ["Texnokaktus.ProgOlymp.YandexContestIntegrationService.Options/Texnokaktus.ProgOlymp.YandexContestIntegrationService.Options.csproj", "Texnokaktus.ProgOlymp.YandexContestIntegrationService.Options/"]
COPY ["modules/Common.Contracts/Texnokaktus.ProgOlymp.Common.Contracts/Texnokaktus.ProgOlymp.Common.Contracts.csproj", "modules/Common.Contracts/Texnokaktus.ProgOlymp.Common.Contracts/"]
RUN dotnet nuget add source --username kav128 --password $GHPAT --store-password-in-clear-text --name "GitHub TexnoKaktus-Progolymp" https://nuget.pkg.github.com/Texnokaktus-ProgOlymp/index.json
RUN dotnet restore "Texnokaktus.ProgOlymp.YandexContestIntegrationService/Texnokaktus.ProgOlymp.YandexContestIntegrationService.csproj"
COPY . .
WORKDIR "/src/Texnokaktus.ProgOlymp.YandexContestIntegrationService"
RUN dotnet build "Texnokaktus.ProgOlymp.YandexContestIntegrationService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Texnokaktus.ProgOlymp.YandexContestIntegrationService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:Version=$VERSION

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Texnokaktus.ProgOlymp.YandexContestIntegrationService.dll"]
