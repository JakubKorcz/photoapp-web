# PhotoApp - Dokumentacja Architektury

## 1. Przegląd Architektury

PhotoApp to aplikacja do zarządzania zdjęciami oparta na architekturze rozproszonych mikrousług. System składa się z backendu .NET 10, frontendu Blazor, procesora obrazów w Go oraz infrastruktury (PostgreSQL + MinIO).

### Diagram ogólny

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              FRONTEND (Blazor)                               │
│                                                                              │
│   ┌──────────────────┐              ┌──────────────────────────────────┐   │
│   │  PhotoApp.Front  │  references  │   PhotoApp.Front.Client          │   │
│   │  (Server Host)   │◄─────────────│   (Blazor WASM)                  │   │
│   │  - SSR           │              │   - Interaktywne komponenty      │   │
│   │  - API Proxy     │              │   - Dashboard, projekty          │   │
│   │  - Auth Handler  │              │                                  │   │
│   └────────┬─────────┘              └──────────────────────────────────┘   │
│            │                                                                 │
└────────────┼─────────────────────────────────────────────────────────────────┘
             │ HTTP (ApiConnection)
             ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                           BACKEND (.NET 10)                                  │
│                                                                              │
│   ┌──────────────────────────────────────────────────────────────────────┐  │
│   │                        PhotoApp.Api                                   │  │
│   │  - REST API (Controllers: Auth, Project, Media, Storage, Account)   │  │
│   │  - JWT Authentication + Refresh Tokens                              │  │
│   │  - Entity Framework Core + PostgreSQL                               │  │
│   │  - MinIO Client (presigned URLs)                                    │  │
│   │  - Auto-migration na starcie                                        │  │
│   └────────┬──────────────────────────────┬──────────────────────────────┘  │
│            │                              │                                  │
└────────────┼──────────────────────────────┼──────────────────────────────────┘
             │                              │
             ▼                              ▼
┌────────────────────────┐    ┌─────────────────────────────────────────────┐
│     PostgreSQL 16      │    │              MinIO (S3-compatible)          │
│  - Baza danych         │    │  - Przechowywanie zdjęć (original/preview/ │
│  - Users, Projects,    │    │    thumbnail w osobnych bucketach)         │
│    Folders, Media,     │    │  - Presigned URLs (upload/download)        │
│    RefreshTokens       │    │  - Console UI na porcie 9001               │
└────────────────────────┘    └─────────────────────────────────────────────┘
                                         ▲
                                         │
┌────────────────────────────────────────┴────────────────────────────────────┐
│                     ImageProcessor (Go + libvips)                           │
│  - POST /import - przyjmuje obraz, konwertuje do WebP                     │
│  - Resize do max 1600px (zachowuje proporcje)                              │
│  - Zapisuje do ./output/ (integracja z MinIO w toku)                       │
└─────────────────────────────────────────────────────────────────────────────┘
```

### Lista serwisów

| Serwis | Technologia | Port (dev) | Opis |
|--------|-------------|------------|------|
| PhotoApp.Api | .NET 10 Web API | 5000/8080 | Backend REST API |
| PhotoApp.Front | .NET 10 Blazor Server | dynamiczny | Frontend server host |
| PhotoApp.ImageProcessor | Go 1.21 + libvips | 8082 | Przetwarzanie obrazów |
| PostgreSQL | postgres:16-alpine | 5432 | Baza danych |
| MinIO | minio/minio:latest | 9000/9001 | Object storage |
| PgAdmin | (via Aspire) | dynamiczny | Admin panel dla PostgreSQL |

---

## 2. Projekty w Solution

### 2.1 PhotoApp.Api

**Ścieżka:** `PhotoApp.Api/`
**Typ:** ASP.NET Core Web API
**Framework:** .NET 10.0

Backend REST API - centralny punkt systemu. Obsługuje autentykację, zarządzanie projektami, dostęp do storage i metadane mediów.

#### Kontrolery i endpointy

| Kontroler | Ścieżka | Opis |
|-----------|---------|------|
| `UsersAuthController` | `/auth/*` | Rejestracja, logowanie (2FA email), refresh token, logout |
| `ProjectController` | `/project/*` | CRUD projektów, pobieranie drzewa folderów |
| `MediaController` | `/media/*` | Zarządzanie metadanymi mediów |
| `ObjectStorageAccessController` | `/storage/*` | Presigned URLs (upload/download/delete) |
| `AccountController` | `/account/*` | Informacje o koncie (memory info - stub) |

#### Flow autentykacji

```
1. POST /auth/login       → walidacja credentials → wysłanie 6-cyfrowego kodu na email
2. POST /auth/login/{code} → weryfikacja kodu → JWT access token + refresh token (HttpOnly cookie)
3. GET /auth/refresh/{username} → odświeżenie tokena na podstawie refresh cookie
4. DELETE /auth/logout    → usunięcie refresh cookie
```

#### Encje (AppDbContext)

| Encja | Tabela | Opis |
|-------|--------|------|
| `User` | Users | Konta użytkowników (username, email, password hash, email code) |
| `Project` | Projects | Projekty fotograficzne |
| `ProjectFolder` | Folders | Hierarchiczna struktura folderów (self-referencing) |
| `Media` | Medias | Metadane zdjęć (typ, object key, wymiary, rozmiar) |
| `ProjectWebDesign` | WebDesignes | Ustawienia designu (font, cover photo) |
| `RefreshToken` | RefreshTokens | Tokeny odświeżania JWT |

#### Kluczowe zależności

- **PostgreSQL** - connection string: `ConnectionStrings:DefaultConnection`
- **MinIO** - konfiguracja: `MinioSettings:Endpoint`, `AccessKey`, `SecretKey`, `UseSSL`
- **JWT** - konfiguracja: `AppSettings:Token`, `Issuer`, `Audience`
- **Gmail SMTP** - `AppSettings:Email`, `EmailPassword`

#### Zmienne środowiskowe

| Zmienna | Opis |
|---------|------|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string |
| `MinioSettings__Endpoint` | Adres MinIO (np. `minio:9000`) |
| `MinioSettings__AccessKey` | MinIO access key |
| `MinioSettings__SecretKey` | MinIO secret key |
| `MinioSettings__UseSSL` | Czy używać SSL dla MinIO |
| `AppSettings__Token` | JWT signing key (min. 32 znaki) |
| `AppSettings__Issuer` | JWT issuer |
| `AppSettings__Audience` | JWT audience |
| `AppSettings__Email` | Gmail address |
| `AppSettings__EmailPassword` | Gmail app password |

---

### 2.2 PhotoApp.Front

**Ścieżka:** `PhotoApp.Front/PhotoApp.Front/`
**Typ:** ASP.NET Core Blazor Web App (Server Host)
**Framework:** .NET 10.0

Server-side host aplikacji Blazor. Odpowiada za:
- **SSR (Server-Side Rendering)** - początkowe renderowanie stron
- **API Proxy** - wszystkie wywołania do API przechodzą przez serwer (nie bezpośrednio z przeglądarki)
- **Authentication** - przechowywanie tokenów w `ProtectedLocalStorage` i dołączanie do requestów

#### Strony (Server-side)

| Ruta | Plik | Opis |
|------|------|------|
| `/home` | `Home.razor` | Landing page marketingowy |
| `/login`, `/signup` | `LoginPage.razor` | Strona logowania/rejestracji |
| `/validation` | `Validation.razor` | Weryfikacja email (kod 6-cyfrowy) |
| `/gallery/{Id:guid}` | `RenderPage.razor` | Publiczna galeria (WIP) |
| `/Error` | `Error.razor` | Strona błędu |
| `/not-found` | `NotFound.razor` | Strona 404 |

#### Komunikacja z API

Komunikacja odbywa się przez klasę `ApiConnection` (partial class):

```
ApiConnection (core)
├── ApiConnection_Users.cs      → Login, Register, Verify, CheckActivity
├── ApiConnection_Project.cs    → CreateProject, GetAllProjects, GetProject
├── ApiConnection_Account.cs    → GetMemoryInfo
└── ApiConnection_Media.cs      → (stub)
```

Wszystkie wywołania przechodzą przez `AuthenticationHeaderHandler` (DelegatingHandler), który:
1. Czyta token z `ProtectedLocalStorage` (klucz: `authToken`)
2. Dołącza go jako `Authorization: Bearer <token>`

#### Konfiguracja HttpClient

```csharp
builder.Services.AddHttpClient("PhotoApp.Api", client =>
{
    var baseUrl = builder.Configuration["VITE_API_URL"] ?? "http://photoapp-api";
    client.BaseAddress = new Uri(baseUrl);
})
.AddHttpMessageHandler<AuthenticationHeaderHandler>();
```

#### Render Modes

Projekt rejestruje oba tryby interaktywne:
- `AddInteractiveServerComponents()` - SignalR-based interactivity
- `AddInteractiveWebAssemblyComponents()` - WASM-based interactivity

Komponenty mogą wybierać tryb per-component via `@rendermode InteractiveServer` lub `@rendermode InteractiveWebAssembly`.

---

### 2.3 PhotoApp.Front.Client

**Ścieżka:** `PhotoApp.Front/PhotoApp.Front.Client/`
**Typ:** Blazor WebAssembly (Client Library)
**Framework:** .NET 10.0

Klient WebAssembly - kompilowany do WASM i pobierany przez przeglądarkę. Zawiera interaktywne komponenty dashboardu.

#### Strony (Client-side)

| Ruta | Plik | Opis |
|------|------|------|
| `/`, `/{ProjectId:guid}` | `Main.razor` | Główny dashboard aplikacji |

#### Komponenty

| Komponent | Opis |
|-----------|------|
| `ContentArea.razor` | Przełącznik między NewGallery a ProjectDetail |
| `NewGalleryPage.razor` | Ekran "Dodaj nową galerię" |
| `NewGalleryBaseInformationPopup.razor` | Formularz tworzenia projektu |
| `ProjectDetailPage.razor` | Szczegóły projektu (zakładki) |
| `ProjectFolderStructureView.razor` | Drzewo folderów |
| `MainNavMenu.razor` | Boczna nawigacja |
| `UpperMenu.razor` | Górny pasek (mail, sort, memory, logout) |

#### Relacja z PhotoApp.Front

```
PhotoApp.Front (Server)
    │
    ├── Referencje PhotoApp.Front.Client
    ├── Skanuje oba assembly w Routes.razor
    ├── Serwuje WASM payload do przeglądarki
    └── Hostuje ApiConnection (server-side)

PhotoApp.Front.Client (WASM)
    │
    ├── Zawiera interaktywne komponenty
    ├── Pobierany przez przeglądarkę
    └── Aktualnie renderuje się server-side (SSR)
```

**Uwaga:** Komponenty w Front.Client injectują `ApiConnection`, ale ta klasa jest zdefiniowana w projekcie serwerowym. Obecnie działa to tylko dlatego, że komponenty renderują się server-side.

---

### 2.4 PhotoApp.Common

**Ścieżka:** `PhotoApp.Common/`
**Typ:** Class Library (zero dependencies)
**Framework:** .NET 10.0

Współdzielona biblioteka kontraktów - DTO, enumy, modele formularzy, definicje błędów. Referencjonowana przez wszystkie projekty.

#### DTO (Data Transfer Objects)

| Klasa | Opis |
|-------|------|
| `ProjectBaseInformationDto` | Podstawowe info projektu (nazwa, data, język, hasło) |
| `ProjectDto` | Pełny projekt (z folderami, design settings) |
| `FolderDto` | Folder z podfolderami i mediami (rekurencyjny) |
| `MediaDto` | Medium (nazwa, typ, object key, wymiary) |
| `UserModelDto` | Model logowania/rejestracji |
| `ServerAuthResponse` | Odpowiedź auth (access token + username) |
| `DesignSettingsDto` | Ustawienia designu (font, cover photo) |

#### Enumy

| Enum | Wartości | Opis |
|------|----------|------|
| `Language` | `UA`, `PL`, `EN` | Języki interfejsu |
| `PhotoType` | `Original`, `Preview`, `Thumbnail` | Typy zdjęć (= nazwy bucketów w MinIO) |
| `SystemRole` | `Admin`, `Member`, `Guest` | Role autoryzacyjne |
| `Device` | `Mobile`, `Desktop` | Typ urządzenia (dla responsywnego designu) |
| `FontWeight` | `Light`, `Regular`, `Medium`, `Bold`, `Black` | Grubość fontu |
| `Layout` | `Left`, `Center`, `Right` | Wyrównanie tekstu |
| `ExpiryDateSelectOption` | `Custom`, `TwoWeeks`, `OneMonth`, `TwoMonths` | Opcje daty wygaśnięcia projektu |

#### ErrorDictionary

Statyczna klasa mapująca kody błędów na komunikaty (po polsku):
- `ERR001` → Nieprawidłowe dane logowania
- `ERR002` → Konto zablokowane
- `ERR003` → Sesja wygasła

#### Projekty referencjujące

- PhotoApp.Api
- PhotoApp.Front
- PhotoApp.Front.Client
- PhotoApp.Client (legacy)

---

### 2.5 PhotoApp.ServiceDefaults

**Ścieżka:** `PhotoApp.ServiceDefaults/`
**Typ:** Aspire Shared Project
**Framework:** .NET 10.0

Współdzielona konfiguracja infrastruktury dla serwisów .NET - OpenTelemetry, health checks, service discovery, HTTP resilience.

#### AddServiceDefaults()

Konfiguruje cztery filary:

1. **OpenTelemetry**
   - Logging (OTLP exporter)
   - Metrics (ASP.NET Core, HttpClient, .NET Runtime)
   - Tracing (z wyłączeniem `/health` i `/alive`)

2. **Health Checks**
   - `/health` - readiness check (wszystkie checki)
   - `/alive` - liveness check (tylko tag `"live"`)

3. **Service Discovery**
   - `AddServiceDiscovery()` - rozwiązywanie nazw serwisów

4. **HTTP Resilience**
   - `AddStandardResilienceHandler()` - Polly (retry, circuit breaker, timeout)

#### MapDefaultEndpoints()

Mapuje endpointy health check (tylko w Development):
- `GET /health` - readiness
- `GET /alive` - liveness

#### Projekty referencjujące

- PhotoApp.Api
- PhotoApp.Front

---

### 2.6 PhotoApp.AppHost

**Ścieżka:** `PhotoApp.AppHost/`
**Typ:** .NET Aspire App Host
**SDK:** Aspire.AppHost.Sdk 13.1.0

Orkiestrator deweloperski - uruchamia wszystkie serwisy jednym `dotnet run`.

#### Zasoby provisionowane

| Zasób | Typ | Opis |
|-------|-----|------|
| `postgres` | PostgreSQL | Baza danych + `.AddDatabase("photoapp")` |
| PgAdmin | Container | Admin panel (`.WithPgAdmin()`) |
| `minio` | MinIO | Object storage + Console UI |
| `photoapp-api` | .NET Project | Backend API |
| `photoapp-front` | .NET Project | Frontend Blazor |

#### Zależności i WaitFor

```csharp
var postgres = builder.AddPostgres("postgres").AddDatabase("photoapp");
var minio = builder.AddMinIO("minio").WithConsoleUI().WithDataVolume("minio-data");

var api = builder.AddProject<Projects.PhotoApp_Api>("photoapp-api")
    .WithReference(postgres).WaitFor(postgres)
    .WithReference(minio).WaitFor(minio)
    .WithEnvironment("MinioSettings__Endpoint", minio.GetEndpoint("default"))
    // ... inne env vars

var front = builder.AddProject<Projects.PhotoApp_Front>("photoapp-front")
    .WithReference(api.GetEndpoint("http"))  // VITE_API_URL
    .WithExternalHttpEndpoints();
```

#### Zmienne środowiskowe (via Aspire)

**Do API:**
- `MinioSettings__Endpoint` - dynamicznie z `minio.GetEndpoint()`
- `MinioSettings__AccessKey` - z `minio.GetUsername()`
- `MinioSettings__SecretKey` - z `minio.GetPassword()`
- `AppSettings__Token`, `Issuer`, `Audience` - hardcoded

**Do Front:**
- `VITE_API_URL` - dynamicznie z `api.GetHttpEndpoint()`

---

### 2.7 PhotoApp.Client (Legacy)

**Ścieżka:** `PhotoApp.Client/`
**Typ:** Standalone Blazor WebAssembly
**Framework:** .NET 10.0

**UWAGA:** Projekt legacy, **nie jest w solution** (`.sln`). Zastąpiony przez `PhotoApp.Front` + `PhotoApp.Front.Client`.

#### Różnice względem nowego Front

| Aspekt | Client (Legacy) | Front (Nowy) |
|--------|-----------------|--------------|
| Architektura | Standalone WASM | Server + WASM hybrid |
| Hosting | nginx (static files) | ASP.NET Core server |
| API Connection | W przeglądarce | Server-side proxy |
| Auth Handler | No-op (nie działa) | Functional (ProtectedLocalStorage) |
| MudBlazor | 9.1.0 | 9.2.0 |
| W solution | Nie | Tak |

#### Dockerfile

Używa nginx do serwowania statycznych plików WASM. Port 80 w kontenerze, mapowany na 8080 w docker-compose.

**Status:** Do usunięcia/migracji docker-compose na Front.

---

### 2.8 PhotoApp.ImageProcessor

**Ścieżka:** `PhotoApp.ImageProcessor/`
**Typ:** Go Microservice
**Framework:** Go 1.21 + bimg (libvips)

Mikrousługa do przetwarzania obrazów - konwersja do WebP, resize.

#### Endpoint

```
POST /import
Body: raw image bytes
Response: processed image saved to ./output/
```

#### Logika przetwarzania

1. Odczytaj metadane obrazu (orientacja)
2. Jeśli portrait (height > width): resize do 1600px height
3. Jeśli landscape: resize do 1600px width
4. Konwertuj do WebP (quality 82, strip metadata)
5. Zapisz do `./output/photo_<timestamp>.webp`

#### Dockerfile

Multi-stage build:
- Builder: `golang:1.26-alpine` + `vips-dev`
- Runtime: `alpine:latest` + `vips`

#### Status

**WIP** - aktualnie zapisuje lokalnie, nie uploaduje do MinIO. Zmienna `Api__BaseUrl` jest ustawiona w docker-compose, ale nie używana w kodzie.

---

## 3. Orkiestracja: .NET Aspire

### Czym jest .NET Aspire?

.NET Aspire to zestaw narzędzi do orkiestracji aplikacji rozproszonych w środowisku deweloperskim. Zapewnia:
- **Dashboard** z OpenTelemetry (traces, metrics, logs)
- **Service Discovery** - rozwiązywanie nazw serwisów
- **Provisioning** - automatyczne uruchamianie zależności (Postgres, MinIO, etc.)
- **Health Checks** - readiness/liveness probes
- **HTTP Resilience** - retry, circuit breaker via Polly

### Jak działa AppHost

`PhotoApp.AppHost` to punkt wejścia dla `dotnet run`. Definiuje zasoby i ich zależności w kodzie C#:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Infrastruktura
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("photoapp");

var minio = builder.AddMinIO("minio")
    .WithConsoleUI()
    .WithDataVolume("minio-data");

// Aplikacje
var api = builder.AddProject<Projects.PhotoApp_Api>("photoapp-api")
    .WithReference(postgres).WaitFor(postgres)
    .WithReference(minio).WaitFor(minio);

var front = builder.AddProject<Projects.PhotoApp_Front>("photoapp-front")
    .WithReference(api)
    .WithExternalHttpEndpoints();

builder.Build().Run();
```

### Zalety Aspire

| Cecha | Opis |
|-------|------|
| **WaitFor()** | Blokuje start serwisu aż zależności są healthy |
| **Auto connection strings** | Aspire wstrzykuje connection strings automatycznie |
| **Dashboard OTel** | Wizualizacja traces, metrics, logs w czasie rzeczywistym |
| **PgAdmin/MinIO Console** | Narzędzia admin dostępne jednym kliknięciem |
| **Service Discovery** | `http://photoapp-api` rozwiązuje do właściwego adresu |

### Ograniczenia Aspire

- **Tylko dev** - nie jest przeznaczony do produkcji
- **Brak ImageProcessor** - Go microservice nie jest zintegrowany
- **Hardcoded secrets** - JWT secret w kodzie (do poprawy)

### Uruchomienie

```bash
cd PhotoApp.AppHost
dotnet run
```

Dashboard Aspire otworzy się automatycznie w przeglądarce.

---

## 4. Orkiestracja: Docker Compose

### Czym jest Docker Compose?

Docker Compose to narzędzie do definiowania i uruchamiania aplikacji wielokontenerowych. Plik `docker-compose.yml` deklaruje serwisy, sieci, wolumeny i zależności.

### Plik docker-compose.yml

**Lokalizacja:** `PhotoApp/docker-compose.yml`

#### Serwisy

| Serwis | Obraz/Build | Porty | Opis |
|--------|-------------|-------|------|
| `photo-app-client` | Build z `PhotoApp.Client/Dockerfile` | 8080:80 | Legacy frontend (nginx) |
| `photo-app-api` | Build z `PhotoApp.Api/Dockerfile` | 5000:8080, 8081:8081 | Backend API |
| `photo-app-imageprocessor` | Build z `PhotoApp.ImageProcessor/Dockerfile` | 8082:8080 | Go image processor |
| `postgres` | `postgres:16-alpine` | 5432:5432 | Baza danych |
| `minio` | `minio/minio:latest` | 9000:9000, 9001:9001 | Object storage |

#### Sieci

| Sieć | Opis |
|------|------|
| `app-network` | Komunikacja aplikacji (API, Client, ImageProcessor, MinIO) |
| `db-network` | Izolacja bazy danych (tylko API <-> Postgres) |

#### Wolumeny

| Wolumen | Montowanie | Serwis |
|---------|------------|--------|
| `postgres_data` | `/var/lib/postgresql/data` | postgres |
| `minio_data` | `/data` | minio |

#### Health Checks

**PostgreSQL:**
```yaml
test: ["CMD-SHELL", "pg_isready -U postgres -d photoapp"]
interval: 10s, timeout: 5s, retries: 5
```

**MinIO:**
```yaml
test: ["CMD", "curl", "-f", "http://localhost:9000/minio/health/live"]
interval: 30s, timeout: 20s, retries: 3
```

#### Zależności

```yaml
photo-app-api:
  depends_on:
    postgres:
      condition: service_healthy
    minio:
      condition: service_healthy

photo-app-imageprocessor:
  depends_on:
    minio:
      condition: service_healthy

photo-app-client:
  depends_on:
    - photo-app-api
```

#### Plik .env

**Lokalizacja:** `PhotoApp/.env`

```env
ASPNETCORE_ENVIRONMENT=Production
API_PUBLIC_URL=http://localhost:5000
POSTGRES_DB=photoapp
POSTGRES_USER=<twoj-user>
POSTGRES_PASSWORD=<twoje-haslo>
MINIO_ROOT_USER=<twoj-user>
MINIO_ROOT_PASSWORD=<twoje-haslo>
EMAIL_ADDRESS=<twoj-email>
EMAIL_PASSWORD=<twoj-app-password>
```

**UWAGA:** Plik `.env` zawiera credentials - **musi** być w `.gitignore`!

### Uruchomienie

```bash
cd PhotoApp
docker compose up -d
```

### Zatrzymanie

```bash
docker compose down        # zachowuje wolumeny
docker compose down -v     # usuwa wolumeny
```

---

## 5. Porównanie: Aspire vs Docker Compose

| Wymiar | .NET Aspire | Docker Compose |
|--------|-------------|----------------|
| **Przeznaczenie** | Development | Development + Production |
| **Frontend** | PhotoApp.Front (nowy) | PhotoApp.Client (legacy) |
| **ImageProcessor** | Brak | Tak |
| **PgAdmin** | Tak (auto) | Nie |
| **MinIO Console** | Tak (auto) | Tak (port 9001) |
| **Secrets** | Hardcoded w C# | Plik `.env` |
| **Connection strings** | Auto-inject via service discovery | Ręczna konstrukcja w YAML |
| **Dependency ordering** | `WaitFor()` (application-level) | `depends_on` + healthcheck (container-level) |
| **Sieci** | Flat (Aspire-managed) | Segmentowane (app-network + db-network) |
| **Wolumeny** | `minio-data` (named) | `postgres_data` + `minio_data` (named) |
| **Port mapping** | Dynamiczny | Statyczny (8080, 5000, 8082, 5432, 9000) |
| **Restart policy** | Zarządzany przez Aspire | `unless-stopped` |
| **Email config** | Nie skonfigurowane | Via `.env` |
| **Build** | `dotnet run` (bez Docker build) | Multi-stage Docker builds |

### Kiedy co stosować?

| Scenariusz | Rekomendacja |
|------------|--------------|
| **Lokalny development** | Aspire - dashboard OTel, PgAdmin, szybki start |
| **Testowanie pełnego stacku** | Docker Compose - zawiera ImageProcessor |
| **Produkcja (single server)** | Docker Compose - sprawdzony, prosty |
| **Produkcja (Azure)** | Azure Container Apps (via `azd` + Aspire manifest) |

---

## 6. Diagram Zależności Projektów

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           PhotoApp.Common                                    │
│                    (DTO, Enumy, Kontrakty)                                   │
└──────────────────────────────────┬──────────────────────────────────────────┘
                                   │
        ┌──────────────────────────┼──────────────────────────┐
        │                          │                          │
        ▼                          ▼                          ▼
┌───────────────┐        ┌─────────────────┐        ┌─────────────────┐
│ PhotoApp.Api  │        │ PhotoApp.Front  │        │ PhotoApp.Client │
│               │        │                 │        │    (Legacy)     │
└───────┬───────┘        └────────┬────────┘        └─────────────────┘
        │                         │
        │                         │ references
        │                         ▼
        │                ┌─────────────────────┐
        │                │ PhotoApp.Front.Client│
        │                │    (WASM)            │
        │                └─────────────────────┘
        │
        │ references
        ▼
┌─────────────────────────┐
│ PhotoApp.ServiceDefaults │
│   (OTel, Health, etc.)  │
└─────────────────────────┘
        ▲
        │ references
        │
┌───────┴───────┐
│ PhotoApp.Front │ (również referencjuje)
└───────────────┘

┌─────────────────────────┐
│   PhotoApp.AppHost      │
│   (Aspire Orchestrator) │
│                         │
│   references:           │
│   - PhotoApp.Api        │
│   - PhotoApp.Front      │
└─────────────────────────┘

┌─────────────────────────┐
│ PhotoApp.ImageProcessor │
│      (Go, standalone)   │
│                         │
│   Brak referencji .NET  │
└─────────────────────────┘
```

---

## 7. Znane Problemy i TODO

### Do naprawienia

| Problem | Opis | Priorytet |
|---------|------|-----------|
| **Brak UseAuthentication()** | W `Program.cs` API jest `UseAuthorization()` ale brakuje `UseAuthentication()` - JWT może nie działać poprawnie | Wysoki |
| **Legacy Client w Docker Compose** | docker-compose używa `PhotoApp.Client` zamiast `PhotoApp.Front` | Średni |
| **Brak ImageProcessor w Aspire** | Go microservice nie jest zintegrowany z AppHost | Średni |
| **Credentials w .env** | Plik `.env` zawiera hasła, powinien być w `.gitignore` | Wysoki |
| **JWT secret hardcoded** | W AppHost.cs JWT secret jest w kodzie źródłowym | Średni |
| **Brak Dockerfile dla Front** | PhotoApp.Front nie ma Dockerfile, nie można go zbudować w docker-compose | Średni |

### WIP (Work in Progress)

| Obszar | Status |
|--------|--------|
| **ImageProcessor → MinIO** | Processor zapisuje lokalnie, nie uploaduje do MinIO |
| **Render Modes** | Żaden komponent nie używa jawnie `@rendermode` |
| **ApiConnection w Front.Client** | Komponenty injectują ApiConnection, ale klasa jest w server project |
| **ProjectWebDesignRepository** | Stub - `CretateWebDesign()` zwraca null |
| **TokenService** | Pusta klasa |
| **AccountController.GetMemory** | Zwraca hardcoded values |

### Rekomendacje

1. **Utwórz `.env.example`** - template bez credentials
2. **Dodaj `.env` do `.gitignore`** - jeśli jeszcze nie jest
3. **Ujednolić frontend** - migracja docker-compose na PhotoApp.Front
4. **Dodaj ImageProcessor do Aspire** - via `builder.AddDockerfile()`
5. **Użyj User Secrets** - `dotnet user-secrets` dla JWT secret i email credentials
6. **Napraw UseAuthentication()** - dodać przed `UseAuthorization()` w API

---

## 8. Słownik Terminów

| Termin | Opis |
|--------|------|
| **Aspire AppHost** | Projekt orkiestratora deweloperskiego (.NET Aspire) |
| **ServiceDefaults** | Współdzielona konfiguracja OTel, health checks, resilience |
| **Service Discovery** | Mechanizm rozwiązywania nazw serwisów (np. `http://photoapp-api`) |
| **Presigned URL** | Tymczasowy URL z podpisem do bezpośredniego dostępu do MinIO |
| **ProtectedLocalStorage** | Serwerowe szyfrowane przechowywanie danych w przeglądarce |
| **SSR** | Server-Side Rendering - renderowanie HTML na serwerze |
| **WASM** | WebAssembly - kod .NET kompilowany do uruchomienia w przeglądarce |
| **libvips** | Biblioteka do szybkiego przetwarzania obrazów (używana przez ImageProcessor) |
| **OTel** | OpenTelemetry - standard observability (traces, metrics, logs) |
| **OTLP** | OpenTelemetry Protocol - protokół eksportu telemetrii |

---

*Dokumentacja wygenerowana: 2026-06-14*
*Wersja projektu: .NET 10.0*
