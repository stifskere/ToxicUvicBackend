FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ToxicUvicBackend.csproj", "./"]
RUN dotnet restore "ToxicUvicBackend.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "ToxicUvicBackend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ToxicUvicBackend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
EXPOSE 10876
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToxicUvicBackend.dll"]
