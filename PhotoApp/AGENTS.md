# PhotoApp - AI Agent Guide

## Szybki Start

**Pełna dokumentacja architektury:** `docs/architecture.md`

## Struktura Projektu

```
PhotoApp/
├── PhotoApp.Api/              # Backend REST API (.NET 10)
├── PhotoApp.Front/            # Frontend Blazor (Server + WASM)
│   ├── PhotoApp.Front/        # Server host
│   └── PhotoApp.Front.Client/ # WASM client
├── PhotoApp.Common/           # Współdzielone DTO/enumy
├── PhotoApp.ServiceDefaults/  # Aspire defaults (OTel, health)
├── PhotoApp.AppHost/          # Aspire orchestrator
├── PhotoApp.Client/           # [LEGACY] Standalone WASM
├── PhotoApp.ImageProcessor/   # Go microservice (libvips)
├── docker-compose.yml         # Docker Compose (prod/dev)
└── docs/                      # Dokumentacja
```

## Uruchomienie

### Development (Aspire)
```bash
cd PhotoApp.AppHost
dotnet run
```

### Docker Compose
```bash
docker compose up -d
```

## Kluczowe Informacje

- **Framework:** .NET 10.0
- **Baza:** PostgreSQL 16
- **Storage:** MinIO (S3-compatible)
- **Frontend:** Blazor Web App (Server + WASM hybrid)
- **Auth:** JWT + Refresh Token (HttpOnly cookie) + 6-digit email code

## Znane Problemy

1. Brak `UseAuthentication()` w API (może wpływać na JWT)
2. Docker Compose używa legacy `PhotoApp.Client` zamiast `PhotoApp.Front`
3. Plik `.env` zawiera credentials - powinien być w `.gitignore`

## Komendy

```bash
# Build solution
dotnet build PhotoApp.sln

# Run migrations
dotnet ef database update --project PhotoApp.Api

# Docker
docker compose up -d
docker compose down
```

---

*Po szczegóły zobacz: `docs/architecture.md`*
