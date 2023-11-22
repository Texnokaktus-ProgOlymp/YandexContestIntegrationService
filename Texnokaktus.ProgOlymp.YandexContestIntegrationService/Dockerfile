FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Texnokaktus.ProgOlymp.YandexContestIntegrationService/Texnokaktus.ProgOlymp.YandexContestIntegrationService.csproj", "Texnokaktus.ProgOlymp.YandexContestIntegrationService/"]
RUN dotnet restore "Texnokaktus.ProgOlymp.YandexContestIntegrationService/Texnokaktus.ProgOlymp.YandexContestIntegrationService.csproj"
COPY . .
WORKDIR "/src/Texnokaktus.ProgOlymp.YandexContestIntegrationService"
RUN dotnet build "Texnokaktus.ProgOlymp.YandexContestIntegrationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Texnokaktus.ProgOlymp.YandexContestIntegrationService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Texnokaktus.ProgOlymp.YandexContestIntegrationService.dll"]
