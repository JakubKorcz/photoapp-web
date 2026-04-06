# PhotoApp - Dokumentacja

Witaj w dokumentacji PhotoApp! Znajdziesz tutaj wszystkie informacje potrzebne do pracy nad projektem.

---

## 📚 Spis treści

### Pierwsze kroki
- [Quick Start](QUICKSTART.md) - Szybki start
- [Setup](SETUP.md) - Konfiguracja środowiska

### Architektura
- [Architecture](ARCHITECTURE.md) - Architektura systemu
- [Moduły](modules/) - Dokumentacja poszczególnych modułów

### Standardy i zasady
- [Coding Standards](CODING_STANDARDS.md) - Zasady pisania kodu (DO's & DON'Ts)

---

## 🏗️ Projekty

| Moduł | Opis | Plik dokumentacji |
|-------|------|-------------------|
| **PhotoApp.Api** | Backend API | [api.md](modules/api.md) |
| **PhotoApp.Front** | Frontend Blazor | [front.md](modules/front.md) |
| **PhotoApp.Common** | Współdzielone modele | [common.md](modules/common.md) |
| **PhotoApp.ImageProcessor** | Go microservice | [imageprocessor.md](modules/imageprocessor.md) |
| **PhotoApp.AppHost** | .NET Aspire | [apphost.md](modules/apphost.md) |
| **PhotoApp.Client** | Stary frontend WASM | [client.md](modules/client.md) (deprecated) |

---

## 🚀 Szybki start

```bash
# Klonowanie i uruchomienie
cd PhotoApp
dotnet restore
dotnet run --project PhotoApp.AppHost
```

Więcej szczegółów w [Quick Start](QUICKSTART.md)

---

## 🤖 Dla agentów AI

Ta dokumentacja została stworzona z myślą o agentach programistycznych. Przed rozpoczęciem pracy koniecznie przeczytaj:

1. **[Coding Standards](CODING_STANDARDS.md)** - Zasady pisania kodu
2. **[Architecture](ARCHITECTURE.md)** - Architektura systemu
3. Odpowiedni moduł w [modules/](modules/)

Każdy moduł zawiera:
- Opis działania
- Strukturę plików
- Kluczowe komponenty
- Przepływ danych
- Konfigurację

---

## 📞 Kontakt

W przypadku pytań sprawdź dokumentację poszczególnych modułów w folderze [modules/](modules/).
