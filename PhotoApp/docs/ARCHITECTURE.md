# Architecture - Architektura systemu

Dokumentacja architektury aplikacji PhotoApp.

---

## 🏗️ Ogólna struktura

```
┌─────────────────────────────────────────────────────────────────┐
│                      PhotoApp Platform                          │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────┐    ┌──────────────┐    ┌──────────────┐    │
│  │   Frontend   │◄──►│     API      │◄──►│    MinIO     │    │
│  │  (Blazor)    │    │  (.NET 10)   │    │   (S3)       │    │
│  └──────────────┘    └──────┬───────┘    └──────────────┘    │
│                             │                                   │
│                      ┌──────▼───────┐                         │
│                      │  PostgreSQL  │                         │
│                      │   (EF Core)   │                         │
│                      └───────────────┘                         │
│                                                                 │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │              ImageProcessor (Go)                         │  │
│  │  - Resize images to max 1600px                          │  │
│  │  - Convert to WebP                                       │  │
│  │  - Strip metadata                                        │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🔄 Przepływ danych

### Rejestracja użytkownika

```
User → Frontend → API → PostgreSQL
                ↓
           Email (Gmail SMTP)
                ↓
         Access Token (JWT)
```

### Tworzenie projektu

```
User → Frontend → API → PostgreSQL (Project, Folder)
                ↓
            MinIO (upload URL)
                ↓
         Frontend → MinIO (upload)
```

### Wyświetlanie galerii

```
User → Frontend → API → PostgreSQL (Project, Media metadata)
                ↓
            MinIO (download URL)
                ↓
         Frontend → MinIO (thumbnail)
```

---

## 📦 Moduły

| Moduł | Technologia | Opis |
|-------|-------------|------|
| **PhotoApp.Api** | .NET 10, C# | Backend API |
| **PhotoApp.Front** | Blazor, MudBlazor | Frontend UI |
| **PhotoApp.Common** | C# | Współdzielone DTO/Enums |
| **PhotoApp.ImageProcessor** | Go | Processing obrazów |
| **PhotoApp.AppHost** | .NET Aspire | Orchestracja |
| **PhotoApp.ServiceDefaults** | .NET | Współdzielone konfiguracje |

---

## 🗂️ Struktura bazy danych

### Tabele

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│    User     │     │   Project   │     │  Media      │
├─────────────┤     ├─────────────┤     ├─────────────┤
│ Id (PK)     │◄───►│ Id (PK)     │◄───►│ Id (PK)     │
│ Username    │     │ ProjectName │     │ Name        │
│ Email       │     │ Username    │     │ ObjectKey   │
│ PasswordHash│     │ MainFolderId│     │ Type        │
│ IsActive    │     │ CreatedAt   │     │ SizeBytes   │
└─────────────┘     └──────┬──────┘     └──────┬──────┘
                            │                    │
                     ┌──────▼──────┐      ┌──────▼──────┐
                     │ProjectFolder│      │ProjectFolder│
                     ├─────────────┤      ├─────────────┤
                     │ Id (PK)     │      │ Id (PK)     │
                     │ Name        │      │ Name        │
                     │ ParentFolder│      │ ParentFolder│
                     │ ProjectId   │      │ ProjectId   │
                     └─────────────┘      └─────────────┘
```

### Relacje

- **User** → many → **Project**
- **Project** → one → **ProjectFolder** (MainFolder)
- **ProjectFolder** → many → **Media**
- **ProjectFolder** → self → **ProjectFolder** (nested folders)

---

## 🔐 Autentykacja

### Mechanizm

```
┌────────────────────────────────────────────────────────────┐
│                    JWT + Refresh Token                     │
├────────────────────────────────────────────────────────────┤
│  1. Login → 6-digit code via email                        │
│  2. Verify code → JWT (15 min) + Refresh Token (7 days)  │
│  3. JWT w Authorization header                             │
│  4. Refresh Token w HttpOnly cookie                        │
│  5. JWT expired → Refresh → new JWT                        │
└────────────────────────────────────────────────────────────┘
```

### Role

| Rola | Opis |
|------|------|
| Guest | Nieaktywny użytkownik (po rejestracji) |
| Member | Aktywny użytkownik |
| Admin | Administrator |

---

## ☁️ MinIO (S3 Storage)

### Buckets

| Bucket | Opis |
|--------|------|
| `original` | Oryginalne zdjęcia |
| `preview` | Podgląd (1600px, WebP) |
| `thumbnail` | Miniatury |

### Operacje

- **Upload**: Presigned PUT URL (15 min ważności)
- **Download**: Presigned GET URL (1 godzina ważności)
- **Delete**: RemoveObject API

---

## 🖼️ ImageProcessor (Go)

### Funkcje

1. **Resize**: max 1600px (portrait=height, landscape=width)
2. **Convert**: → WebP, 82% quality
3. **Strip**: EXIF, ICC metadata

### Endpoint

```
POST /import
Body: <raw image binary>
Response: "Koniec przetwarzania! Plik zapisany jako: photo_<timestamp>.webp"
```

---

## 🌐 Frontend (Blazor)

### Struktura

```
PhotoApp.Front/
├── Components/
│   ├── Pages/          # Route pages
│   │   ├── Main.razor
│   │   ├── Home.razor
│   │   └── LoginPage.razor
│   └── Layout/        # Layouts
├── Connection/         # API clients
│   └── ApiConnection.cs
└── wwwroot/          # Static assets
```

### API Communication

```
Frontend → ApiConnection → HttpClient → PhotoApp.Api
```

---

## 🔧 Konfiguracja (.NET Aspire)

```
AppHost
├── PostgreSQL (container)
│   └── photoapp database
├── MinIO (container)
│   └── minioadmin/minioadmin
├── PhotoApp.Api
│   └── Environment: MinIO, JWT, Email config
└── PhotoApp.Front
    └── Environment: API URL
```

---

## 📝 Więcej informacji

- [Quick Start](QUICKSTART.md)
- [Coding Standards](CODING_STANDARDS.md)
- [Moduł API](modules/api.md)
- [Moduł Front](modules/front.md)
