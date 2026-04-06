# Setup - Konfiguracja środowiska

Szczegółowa instrukcja konfiguracji środowiska deweloperskiego.

---

## Wymagania systemowe

| Narzędzie | Wersja minimalna | Rekomendowana |
|-----------|-------------------|---------------|
| .NET SDK | 10.0 (Preview) | 10.0 (Preview) |
| Docker | 24.0+ | 25.0+ |
| Go | 1.21+ | 1.22+ |
| Git | 2.30+ | Najnowsza |

### Instalacja .NET 10.0

```bash
# Windows
winget install Microsoft.DotNet.SDK.Preview --version 10.0.0-*

# macOS
brew install --cask dotnet-sdk@10.0

# Linux
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 10.0
```

### Instalacja Docker

```bash
# Ubuntu
sudo apt-get update
sudo apt-get install docker.io docker-compose

# Windows/Mac
# Pobierz Docker Desktop z https://www.docker.com/products/docker-desktop
```

---

## 🛠️ Konfiguracja projektu

### Krok 1: Clone i restore

```bash
git clone <repo-url>
cd PhotoApp
dotnet restore
```

### Krok 2: Weryfikacja .NET

```bash
dotnet --version
# Powinno pokazać: 10.0.0-preview.x
```

### Krok 3: Uruchomienie

```bash
dotnet run --project PhotoApp.AppHost
```

---

## ⚙️ Konfiguracja (appsettings)

### API Configuration

`PhotoApp.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=photoapp;Username=postgres;Password=postgres"
  },
  "MinioSettings": {
    "Endpoint": "localhost:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "UseSSL": false
  },
  "AppSettings": {
    "Token": "YourSecretKeyAtLeast32CharactersLong!",
    "Issuer": "PhotoApp.Api",
    "Audience": "PhotoApp.Client",
    "Email": "your-email@gmail.com",
    "EmailPassword": "your-app-password"
  }
}
```

### Frontend Configuration

`PhotoApp.Front/appsettings.Development.json`:

```json
{
  "VITE_API_URL": "http://localhost:5001"
}
```

---

## 📧 Konfiguracja Gmail (opcjonalnie)

### Generowanie App Password

1. Wejdź na https://myaccount.google.com/security
2. Włącz 2-Step Verification
3. Wejdź w App Passwords
4. Wygeneruj nowe hasło dla aplikacji

### Użyj hasła aplikacji

```json
"EmailPassword": "xxxx xxxx xxxx xxxx"  // 16-znakowe hasło
```

---

## 🐳 Konfiguracja Docker (opcjonalnie)

### docker-compose.override.yml

```yaml
services:
  postgres:
    environment:
      POSTGRES_PASSWORD: postgres
      POSTGRES_USER: postgres
      POSTGRES_DB: photoapp

  minio:
    environment:
      MINIO_ROOT_USER: minioadmin
      MINIO_ROOT_PASSWORD: minioadmin
```

---

## 🔧 VS Code (opcjonalnie)

### Rekomendowane rozszerzenia

- C# (Microsoft)
- .NET Install Tool
- GitLens
- Docker
- Prettier

### .vscode/tasks.json

```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "Build API",
      "command": "dotnet",
      "args": ["build", "PhotoApp/PhotoApp.Api/PhotoApp.Api.csproj"],
      "group": "build"
    },
    {
      "label": "Run AppHost",
      "command": "dotnet",
      "args": ["run", "--project", "PhotoApp/PhotoApp.AppHost"],
      "group": "test"
    }
  ]
}
```

---

## 🧪 Weryfikacja instalacji

### Test 1: .NET

```bash
dotnet --version
dotnet --list-sdks
```

### Test 2: Docker

```bash
docker --version
docker ps  # powinno być puste lub containery działają
```

### Test 3: Build projektu

```bash
cd PhotoApp
dotnet build
# Powinno zakończyć się bez błędów
```

### Test 4: Uruchomienie

```bash
dotnet run --project PhotoApp.AppHost
# Sprawdź http://localhost:15081
```

---

## 🚨 Rozwiązywanie problemów

### "command not found: dotnet"

```bash
# Dodaj do PATH
export PATH="$PATH:$HOME/.dotnet"
source ~/.bashrc
```

### "connection refused" (PostgreSQL/MinIO)

```bash
# Uruchom Docker Desktop
# Sprawdź czy kontenery działają
docker ps
```

### "permission denied" (Linux)

```bash
sudo usermod -aG docker $USER
# Wyloguj się i zaloguj ponownie
```

---

## 📦 Struktura folderów

```
PhotoApp/
├── docs/                 # Ta dokumentacja
├── PhotoApp.sln         # Solution file
├── PhotoApp.Api/        # Backend
├── PhotoApp.Front/      # Frontend
├── PhotoApp.Common/     # Shared
├── PhotoApp.ImageProcessor/  # Go
├── PhotoApp.AppHost/    # Aspire
└── docker-compose.yml   # Docker config
```

---

## 📚 Następne kroki

- [Quick Start](QUICKSTART.md) - Szybki start
- [Architecture](ARCHITECTURE.md) - Architektura
- [Coding Standards](CODING_STANDARDS.md) - Zasady kodu
