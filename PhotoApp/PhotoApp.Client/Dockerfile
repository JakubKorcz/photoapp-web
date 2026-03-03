FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["PhotoApp.Client/PhotoApp.Client.csproj", "PhotoApp.Client/"]
COPY ["PhotoApp.Common/PhotoApp.Common.csproj", "PhotoApp.Common/"]

RUN dotnet restore "PhotoApp.Client/PhotoApp.Client.csproj"

COPY . .

WORKDIR "/src/PhotoApp.Client"
RUN dotnet build "PhotoApp.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PhotoApp.Client.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM nginx:alpine
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .

COPY PhotoApp.Client/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80