# Quick Start - Szybki start

Szybki przewodnik uruchomienia projektu PhotoApp.

---

## Wymagania wstępne

| Narzędzie | Wersja |
|-----------|--------|
| .NET SDK | 10.0 (Preview) |
| Docker | 24.0+ |
| Go | 1.21+ (opcjonalnie, dla ImageProcessor) |

---

## ⚡ Uruchomienie w 3 krokach

### Krok 1: Pobranie i restore

```bash
cd PhotoApp
dotnet restore
```

### Krok 2: Uruchomienie przez AppHost

```bash
dotnet run --project PhotoApp.AppHost
```

### Krok 3: Otwórz przeglądarkę

| Usługa | Adres |
|--------|-------|
| **Dashboard Aspire** | http://localhost:15081 |
| **Frontend** | http://localhost:15080 |
| **API** | http://localhost:15000 |
| **Swagger API** | http://localhost:15000/swagger |
| **MinIO Console** | http://localhost:9001 |
| **pgAdmin** | http://localhost:5050 |

---

## 🔧 Uruchomienie poszczególnych modułów

### Tylko Backend (API)

```bash
cd PhotoApp/PhotoApp.Api
dotnet run
# → http://localhost:5001
```

### Tylko Frontend

```bash
cd PhotoApp/PhotoApp.Front/PhotoApp.Front
dotnet run
# → http://localhost:5002
```

### ImageProcessor (Go)

```bash
cd PhotoApp/PhotoApp.ImageProcessor
go run main.go
# → http://localhost:8080
```

---

## 🐳 Uruchomienie przez Docker

```bash
cd PhotoApp
docker-compose up --build
```

---

## ✅ Weryfikacja działania

### 1. Sprawdź API

```bash
curl http://localhost:15000/swagger
```

### 2. Sprawdź Frontend

Otwórz w przeglądarce: http://localhost:15080

### 3. Sprawdź MinIO

- Console: http://localhost:9001
- Login: `minioadmin` / `minioadmin`

---

## 🔍 Rozwiązywanie problemów

### "dotnet not found"
```bash
# Zainstaluj .NET 10.0 SDK
# https://dotnet.microsoft.com/download/dotnet/10.0
```

### "Connection refused"
Upewnij się że Docker Desktop jest uruchomiony i kontenery działają:
```bash
docker ps
```

### "Database not found"
Migruj bazę:
```bash
cd PhotoApp/PhotoApp.Api
dotnet ef database update
```

---

## 📚 Następne kroki

- [Setup](SETUP.md) - Szczegółowa konfiguracja
- [Architecture](ARCHITECTURE.md) - Architektura systemu
- [Coding Standards](CODING_STANDARDS.md) - Zasady pisania kodu
