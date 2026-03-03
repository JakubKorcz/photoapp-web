FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PhotoApp.Api/PhotoApp.Api.csproj", "PhotoApp.Api/"]
COPY ["PhotoApp.Common/PhotoApp.Common.csproj", "PhotoApp.Common/"]
RUN dotnet restore "./PhotoApp.Api/PhotoApp.Api.csproj"
COPY . .
WORKDIR "/src/PhotoApp.Api"
RUN dotnet build "./PhotoApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PhotoApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PhotoApp.Api.dll"]