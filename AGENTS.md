# AGENTS.md - PhotoApp Development Guide

This file provides guidelines for agentic coding agents working on the PhotoApp codebase.

---

## Quick Reference

| Project | Run Command | Port |
|---------|-------------|------|
| **AppHost** (Recommended) | `dotnet run --project PhotoApp.AppHost` | 15081 |
| **PhotoApp.Api** | `dotnet run` (in Api folder) | 5001 |
| **PhotoApp.Front** | `dotnet run` (in Front folder) | 5002 |
| **ImageProcessor** | `go run main.go` | 8080 |

---

## Project Overview

PhotoApp is a photo management platform with:
- **Backend**: ASP.NET Core 10 Web API (C#)
- **Frontend**: Blazor (Server + WebAssembly hybrid)
- **Image Processing**: Go microservice
- **Database**: PostgreSQL with Entity Framework Core
- **Storage**: MinIO (S3-compatible object storage)
- **UI Framework**: MudBlazor

---

## Module Documentation

| Project | File | Description |
|---------|------|-------------|
| **PhotoApp.Api** | `AGENTS_API.md` | Backend: auth, projects, storage, email |
| **PhotoApp.Front** | `AGENTS_FRONT.md` | Frontend: pages, MudBlazor, API clients |
| **PhotoApp.Common** | `AGENTS_COMMON.md` | Shared: DTOs, Enums |
| **PhotoApp.ImageProcessor** | `AGENTS_IMAGEPROCESSOR.md` | Go: image resizing |
| **PhotoApp.AppHost** | `AGENTS_APPHOST.md` | .NET Aspire orchestration |
| **PhotoApp.Client** | `AGENTS_CLIENT.md` | ⚠️ DEPRECATED - scheduled for removal |

---

## Project Structure

```
PhotoApp/
├── PhotoApp.sln                          # Main solution
├── PhotoApp.Api/                         # Web API (backend)
│   ├── Controllers/                      # Endpoints
│   ├── DbObjects/                        # Entities
│   ├── Repository/                       # Data access
│   ├── Service/                          # Business logic
│   └── Tools/                             # Mailer, Tokens
├── PhotoApp.Front/                       # Blazor frontend
│   ├── PhotoApp.Front/                   # Main app
│   └── PhotoApp.Front.Client/            # Shared components
├── PhotoApp.Common/                      # Shared models
│   ├── ModelsShared/                     # DTOs
│   └── EnumShared/                       # Enums
├── PhotoApp.ImageProcessor/              # Go microservice
├── PhotoApp.AppHost/                     # Aspire orchestration
├── PhotoApp.ServiceDefaults/             # Shared configs
└── PhotoApp.Client/                      # DEPRECATED
```

---

## Build Commands

### Recommended: Build Entire Solution
```bash
cd PhotoApp
dotnet build
```

### Individual Projects

```bash
# API
dotnet build PhotoApp/PhotoApp.Api/PhotoApp.Api.csproj

# Frontend
dotnet build PhotoApp/PhotoApp.Front/PhotoApp.Front/PhotoApp.Front.csproj

# Common
dotnet build PhotoApp/PhotoApp.Common/PhotoApp.Common.csproj

# AppHost
dotnet build PhotoApp/PhotoApp.AppHost/PhotoApp.AppHost.csproj

# ImageProcessor (Go)
cd PhotoApp/PhotoApp.ImageProcessor
go build -o imageprocessor
```

### Clean & Rebuild
```bash
dotnet clean && dotnet build
```

---

## Run Commands

### Recommended: Use AppHost (orchestrates everything)
```bash
cd PhotoApp
dotnet run --project PhotoApp.AppHost
```

### Run Individual Projects (requires external services)

```bash
# API (requires PostgreSQL, MinIO)
cd PhotoApp/PhotoApp.Api
dotnet run
# → https://localhost:5001

# Frontend (requires API running)
cd PhotoApp/PhotoApp.Front/PhotoApp.Front
dotnet run
# → https://localhost:5002

# ImageProcessor
cd PhotoApp/PhotoApp.ImageProcessor
go run main.go
# → http://localhost:8080
```

---

## Test Commands

Currently **no tests exist**. When added:

```bash
# Run all tests
dotnet test

# Run for specific project
dotnet test PhotoApp/PhotoApp.Api/PhotoApp.Api.csproj

# Single test
dotnet test --filter "FullyQualifiedName~TestClassName.MethodName"
```

---

## Database Migrations

```bash
cd PhotoApp/PhotoApp.Api

# Create migration
dotnet ef migrations add <MigrationName>

# Apply migrations
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

---

## Docker

```bash
cd PhotoApp

# Build and run everything
docker-compose up --build

# Run specific service
docker-compose up photo-app-api

# Rebuild
docker-compose up --build photo-app-api
```

---

## Code Style Guidelines

### C# Conventions

| Element | Convention | Example |
|---------|------------|---------|
| Classes/Methods | PascalCase | `UserService`, `GetUserByIdAsync` |
| Private fields | camelCase + underscore | `_userRepository` |
| Parameters | camelCase | `userName` |
| Properties | PascalCase | `UserName` |
| DTOs | PascalCase + "Dto" suffix | `UserModelDto` |
| Enums | PascalCase | `SystemRole` |

### Key Principles
- Use **async/await** for all I/O
- Suffix async methods with **Async**
- Use **primary constructors** (C# 12)
- Enable **nullable reference types**
- Return **null** for "not found" (don't throw)

### Example
```csharp
public async Task<User?> GetUserByIdAsync(Guid id)
{
    var user = await _repository.FindAsync(id);
    if (user is null)
    {
        return null;
    }
    return user;
}
```

### API Controllers
```csharp
[ApiController]
[Route("project")]
public class ProjectController : ControllerBase
{
    [HttpGet("user/{username}/projects")]
    public ActionResult<IEnumerable<ProjectDto>> GetProjects([FromRoute] string username)
    {
        var projects = _service.GetProjectsByUser(username);
        return Ok(projects);
    }
}
```

---

## Go Code Style (ImageProcessor)

### Naming
- Exported: `PascalCase`
- Private: `camelCase`
- Files: lowercase with underscores

### Error Handling
```go
if err != nil {
    http.Error(w, "Error message", http.StatusBadRequest)
    return
}
```

---

## Important Notes

- **Always use async/await** for database and I/O operations
- **Primary constructors** used throughout (C# 12)
- **Nullable reference types** enabled
- **Polish comments** exist in some files - maintain consistency
- **No tests exist** yet
