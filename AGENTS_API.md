# AGENTS_API.md - PhotoApp.Api

## Overview

PhotoApp.Api is the **backend core** of the application. Responsible for:
- **Authentication** - Login, register, email verification, JWT tokens
- **Project management** - CRUD for photo projects with folder organization
- **Media management** - Photo metadata and organization
- **Object storage** - MinIO (S3-compatible) integration for file storage
- **Email services** - SMTP notifications for login codes

**Target**: .NET 10.0  
**Architecture**: Controller → Service → Repository → DbContext

---

## Project Structure

```
PhotoApp.Api/
├── Controllers/           # API endpoints
│   ├── UsersAuthController.cs    # /auth - register, login, logout, refresh
│   ├── AccountController.cs      # /account - memory info
│   ├── ProjectController.cs        # /project - project CRUD
│   ├── MediaController.cs          # /media - media metadata
│   └── ObjectStorageAccessController.cs  # /storage - MinIO URLs
├── DbObjects/             # Entity classes (User, Project, Media, etc.)
├── Repository/           # Data access layer
├── Service/              # Business logic
├── Tools/                # Utilities
│   ├── Mailer/           # Email sending (Gmail SMTP)
│   │   ├── Mailer.cs     # SMTP client, HTML templates
│   │   └── CodeGenerator.cs  # 6-digit code generation
│   └── Tokens/           # JWT management
│       └── TokenManager.cs   # Access token (15min) + refresh token
├── Migrations/           # EF Core migrations
├── AppDbContext.cs       # EF Core DbContext
├── MappingProfile.cs      # AutoMapper config
└── Program.cs            # Entry point
```

---

## Key Components

### Controllers

| Controller | Route | Endpoints |
|------------|-------|-----------|
| `UsersAuthController` | `/auth` | POST register, login, logout; GET refresh; PATCH activate |
| `AccountController` | `/account` | GET memory |
| `ProjectController` | `/project` | GET all projects, GET project by id |
| `MediaController` | `/media` | POST create media |
| `ObjectStorageAccessController` | `/storage` | GET download URL, GET upload URL, DELETE |

### Services

| Service | Responsibility |
|---------|----------------|
| `UserService` | Auth, registration, email codes, token generation |
| `ProjectService` | Project CRUD, folder management |
| `MediaService` | Media metadata, folder assignment |
| `TokenService` | (minimal, TokenManager does the work) |

### Repositories

| Repository | Manages |
|------------|---------|
| `UserRepository` | Users, passwords, activation |
| `ProjectRepository` | Photo projects |
| `ProjectFolderRepository` | Folder hierarchy |
| `MediaRepository` | Media metadata |
| `RefreshTokenRepository` | JWT refresh tokens (7-day expiry) |

### Tools

| Tool | Description |
|------|-------------|
| `TokenManager` | Generates JWT (15min) + refresh tokens |
| `Mailer` | SMTP client, sends HTML emails via Gmail |
| `CodeGenerator` | Generates 6-digit numeric codes |

---

## Authentication Flow

```
REGISTRATION:
1. POST /auth/register → Creates user, sends email with JWT (Guest role)
2. GET /auth/register/activity → Validates token, issues refresh cookie
3. PATCH /auth/register/activate → Sets IsActive = true

LOGIN:
1. POST /auth/login → Validates credentials, sends 6-digit code to email
2. POST /auth/login/{code} → Verifies code, returns JWT + refresh cookie

REFRESH:
GET /auth/refresh/{username} → Uses refresh cookie to get new JWT
```

---

## Database Schema

```
User
├── Id (Guid, PK)
├── Username (unique, required)
├── Email (required)
├── PasswordHash
├── EmailLoginCode (6-digit)
├── EmailLoginCodeExpiration (DateTime)
└── IsActive (bool)

Project
├── Id (Guid, PK)
├── ProjectName
├── Username (owner)
├── MainFolderId → ProjectFolder
├── CreatedAt
└── WebDesignAssignments (junction table)

ProjectFolder
├── Id (Guid, PK)
├── Name
├── IsHeadFolder (bool)
├── ParentFolderId (self-reference)
└── ProjectId

Media
├── Id (Guid, PK)
├── Name, Extension, Type
├── ObjectKey (MinIO path)
├── SizeBytes, Width, Height
├── IsLiked
└── ParentFolderId → ProjectFolder

RefreshToken
├── Token (string)
├── Username
├── Expires (DateTime)
└── IsRevoked (bool)
```

---

## Build & Run

### Prerequisites
- .NET 10.0 SDK
- PostgreSQL running
- MinIO running

### Build
```bash
cd PhotoApp
dotnet build PhotoApp/PhotoApp.Api/PhotoApp.Api.csproj
```

### Run (Development)
```bash
cd PhotoApp/PhotoApp.Api
dotnet run
# API available at https://localhost:5001
# Swagger at https://localhost:5001/swagger
```

### Run with Aspire
```bash
cd PhotoApp
dotnet run --project PhotoApp.AppHost
# All services orchestrated by AppHost
```

### Required Configuration

`appsettings.Development.json`:
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
    "Token": "YOUR_SECRET_KEY_AT_LEAST_32_CHARS",
    "Issuer": "PhotoApp.Api",
    "Audience": "PhotoApp.Client",
    "Email": "your-email@gmail.com",
    "EmailPassword": "gmail-app-password"
  }
}
```

---

## External Integrations

| Service | Details |
|---------|---------|
| **PostgreSQL** | EF Core with Npgsql, auto-migrate on startup |
| **MinIO** | S3 storage, presigned URLs for upload/download |
| **Gmail SMTP** | Port 587, SSL enabled |

---

## Important Notes

- Uses **primary constructors** (C# 12)
- All DB operations are **async**
- Nullable reference types enabled
- Auto-migration on startup
- Refresh tokens stored in HttpOnly cookies
- TokenManager generates JWT with HmacSha512
