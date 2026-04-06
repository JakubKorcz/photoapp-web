# AGENTS_APPHOST.md - PhotoApp.AppHost & PhotoApp.ServiceDefaults

## Overview

These are **.NET Aspire** projects for orchestrating the application.

### PhotoApp.AppHost
The **application orchestrator** that launches all services.

### PhotoApp.ServiceDefaults
**Shared configuration** for all services (OpenTelemetry, health checks, resilience).

---

## AppHost Structure

```
PhotoApp.AppHost/
├── AppHost.cs                    # Orchestration logic
├── PhotoApp.AppHost.csproj       # Aspire SDK + hosting packages
├── Properties/
│   └── launchSettings.json
└── appsettings*.json
```

### AppHost.cs (Current Configuration)

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("photoapp");

var minio = builder.AddMinIO("minio")
    .WithLogin("minioadmin", "minioadmin")
    .WithDataVolume("minio-data")
    .WithConsoleUI();

var minioEndpoint = minio.GetEndpoint("default");

builder.AddProject<Projects.PhotoApp_Api>("photoapp-api")
    .WithReference(postgres).WaitFor(postgres)
    .WithReference(minio).WaitFor(minio)
    .WithEnvironment("MinioSettings__Endpoint", minioEndpoint)
    .WithEnvironment("MinioSettings__AccessKey", minio.GetUsername())
    .WithEnvironment("MinioSettings__SecretKey", minio.GetPassword())
    .WithEnvironment("MinioSettings__UseSSL", "false")
    .WithEnvironment("AppSettings__Token", "...")
    .WithEnvironment("AppSettings__Issuer", "...")
    .WithEnvironment("AppSettings__Audience", "...");

var apiEndpoint = builder.GetHttpEndpoint("photoapp-api");

builder.AddProject<Projects.PhotoApp_Front>("photoapp-front")
    .WithReference(minio)
    .WithReference(postgres)
    .WithExternalHttpEndpoints()
    .WithEnvironment("VITE_API_URL", apiEndpoint);

builder.Build().Run();
```

---

## ServiceDefaults Structure

```
PhotoApp.ServiceDefaults/
├── Extensions.cs                 # AddServiceDefaults(), ConfigureOpenTelemetry()
├── PhotoApp.ServiceDefaults.csproj
└── (no Program.cs - it's a library)
```

### What AddServiceDefaults() Adds

| Feature | Implementation |
|---------|----------------|
| **OpenTelemetry Tracing** | HTTP instrumentation, excludes health endpoints |
| **OpenTelemetry Metrics** | AspNetCore, HTTP, Runtime |
| **Logging** | OpenTelemetry-based with scopes |
| **Health Checks** | `/alive` (liveness), `/health` (readiness) |
| **Service Discovery** | Built-in via `AddServiceDiscovery()` |
| **Resilience** | Standard HTTP resilience handler |
| **HTTP Client Defaults** | Service discovery + resilience |

---

## Build & Run

### Prerequisites
- .NET 10.0 SDK
- Docker (for PostgreSQL, MinIO containers)

### Build
```bash
cd PhotoApp
dotnet build PhotoApp/PhotoApp.AppHost/PhotoApp.AppHost.csproj
```

### Run
```bash
cd PhotoApp
dotnet run --project PhotoApp.AppHost
```

### Launch Profiles

| Profile | AppHost Port | Dashboard |
|---------|--------------|-----------|
| https | 15081 | Enabled (OTLP: 21274) |
| http | 15081 | Disabled |

### Expected Output
```
info: Aspire.Hosting.DistributedApplication[0]
      Aspire App Host is starting up...
info: Aspire.Hosting.Program[0]
      PostgreSQL container started
info: Aspire.Hosting.Program[0]
      MinIO container started
info: Aspire.Hosting.Program[0]
      photoapp-api running on https://localhost:15000
info: Aspire.Hosting.Program[0]
      photoapp-front running on https://localhost:15081
```

---

## Services Orchestrated

| Service | Type | Description |
|---------|------|-------------|
| `postgres` | Container | PostgreSQL 16 with pgAdmin |
| `minio` | Container | MinIO S3 storage |
| `photoapp-api` | Project | ASP.NET Core Web API |
| `photoapp-front` | Project | Blazor Server + WASM |

---

## Architecture

```
┌─────────────────────────────────────────────────────────┐
│                   AppHost (Aspire)                       │
│  - Orchestrates all services                           │
│  - Launches containers (PostgreSQL, MinIO)             │
│  - Provides Dashboard at http://localhost:15081          │
└─────────────────────────────────────────────────────────┘
                            │
        ┌───────────────────┼───────────────────┐
        ▼                   ▼                   ▼
┌───────────────┐   ┌───────────────┐   ┌───────────────┐
│   PostgreSQL  │   │    MinIO     │   │ photoapp-api │
│   (Container) │   │  (Container) │   │   (Project)  │
└───────────────┘   └───────────────┘   └───────┬───────┘
                                                 │
                    ┌────────────────────────────┘
                    ▼
            ┌───────────────┐
            │ photoapp-front│
            │   (Project)   │
            └───────────────┘
                    │
            ┌───────┴───────┐
            ▼               ▼
      ServiceDefaults    ServiceDefaults
      (photoapp-api)    (photoapp-front)
```

---

## Important Notes

- **Requires Aspire SDK**: `Aspire.AppHost.Sdk/13.1.0`
- AppHost automatically handles:
  - Container lifecycle
  - Environment variables
  - Health checks
  - Service discovery
- ServiceDefaults referenced by all service projects
- Provides production-ready observability out of the box
