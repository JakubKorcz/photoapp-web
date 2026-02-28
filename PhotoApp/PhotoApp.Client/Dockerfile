#TODO Niepoprawny jeszcze

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["PhotoApp.Client.csproj", "PhotoApp.Client/"]
COPY ["../PhotoApp.Common/PhotoApp.Common.csproj", "PhotoApp.Common/"]

RUN dotnet restore "PhotoApp.Client/PhotoApp.Client.csproj"

COPY . .

# 4. Przechodzimy do folderu klienta i budujemy
WORKDIR "/src/PhotoApp.Client"
RUN dotnet build "PhotoApp.Client.csproj" -c Release -o /app/build
RUN dotnet publish "PhotoApp.Client.csproj" -c Release -o /app/publish

# ETAP 2: Serwowanie przez Nginx
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# Kopiujemy opublikowane pliki WASM
COPY --from=build /app/publish/wwwroot .

# Kopiujemy konfigurację Nginx
# Pamiętaj, że ścieżka musi być poprawna względem folderu głównego!
COPY PhotoApp.Client/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80