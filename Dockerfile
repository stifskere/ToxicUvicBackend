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
EXPOSE 10001
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToxicUvicBackend.dll"]

ENV MYSQL_SERVER=${MYSQL_SERVER}
ENV MYSQL_DATABASE=${MYSQL_DATABASE}
ENV MYSQL_USERNAME=${MYSQL_USERNAME}
ENV MYSQL_PASSWORD=${MYSQL_PASSWORD}
