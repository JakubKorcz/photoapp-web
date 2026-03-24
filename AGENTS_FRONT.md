# AGENTS_FRONT.md - PhotoApp.Front

## Overview

PhotoApp.Front is the **frontend application** where users interact with the platform. Responsible for:
- **User interface** - Login, registration, verification codes
- **Project management** - Create, view, configure photo projects
- **Design customization** - Font, layout, cover photo settings
- **Folder navigation** - Browse project folders and media

**Target**: .NET 10.0  
**UI Framework**: MudBlazor v9.2.0  
**Rendering**: Blazor Server + Blazor WebAssembly (hybrid)

---

## Project Structure

```
PhotoApp.Front/
├── PhotoApp.Front/                    # Main Blazor Server app
│   ├── Components/                    # Razor components
│   │   ├── Pages/                    # Route pages
│   │   │   ├── Main.razor           # Main app (root route)
│   │   │   ├── Home.razor           # Landing page
│   │   │   ├── LoginPage.razor      # Login/signup
│   │   │   ├── Validation.razor      # Email verification
│   │   │   └── ...
│   │   └── Layout/                   # Layout components
│   │       ├── Main/                 # App shell
│   │       ├── Login/               # Auth forms
│   │       └── Home/                 # Landing layouts
│   ├── Connection/                   # API clients
│   │   ├── ApiConnection.cs          # Base HTTP client
│   │   ├── ApiConnection_Users.cs    # Auth endpoints
│   │   ├── ApiConnection_Project.cs  # Project endpoints
│   │   ├── ApiConnection_Account.cs  # Account endpoints
│   │   └── ApiConnection_Media.cs    # Media endpoints
│   ├── Models/                       # Frontend models
│   │   ├── UserModel.cs
│   │   ├── ProjectFormModel.cs
│   │   └── MenuItem.cs
│   ├── Program.cs                    # Entry point
│   └── wwwroot/                      # Static assets
└── PhotoApp.Front.Client/            # Shared WASM components
    ├── Program.cs
    └── _Imports.razor
```

---

## Key Components

### Pages (Routes)

| Route | Component | Purpose |
|-------|-----------|---------|
| `/`, `/{ProjectId}` | `Main.razor` | Main app with navigation |
| `/home` | `Home.razor` | Landing page with video |
| `/login`, `/signup` | `LoginPage.razor` | Unified auth with sliding animation |
| `/gallery/{Id}` | `RenderPage.razor` | Gallery display |
| `/validation` | `Validation.razor` | Account activation |
| `/not-found` | `NotFound.razor` | 404 page |
| `/error` | `Error.razor` | Error page |

### API Clients

| Client | Methods |
|--------|---------|
| `ApiConnection` | Base class with error handling |
| `ApiConnection_Users` | Login, Register, Verify codes |
| `ApiConnection_Project` | Create, Get, Update projects |
| `ApiConnection_Account` | Memory info |
| `ApiConnection_Media` | Media operations (partial) |

### Authentication Flow (UI)

```
1. User enters email/password → SignUpComponent.razor
2. Submit → ApiConnection.Register() → POST /auth/register
3. User receives email with access token
4. Enter 6-digit code → ValidationCodeForm.razor
5. Verify → ApiConnection.RegisterVerify() → POST /auth/login/{code}
6. Redirect to /main
```

### Layout Components

| Component | Purpose |
|-----------|---------|
| `UpperMenu.razor` | Top bar, memory display, logout |
| `MainNavMenu.razor` | Left sidebar navigation |
| `MainNavSubMenuFolders.razor` | Project folder tree |
| `NewGalleryBaseInformationPopup.razor` | Create project modal |
| `ProjectDetailPage.razor` | Full project view |

---

## UI Framework - MudBlazor

```razor
<MudThemeProvider />
<MudDialogProvider />
<MudSnackbarProvider />
<MudPopoverProvider />
```

### Components Used
- `MudDatePicker` - Date selection
- `MudDialogProvider` - Modals
- `MudSnackbarProvider` - Toast notifications

### Styling
- Custom CSS: `AuthStyles.css`, `colors.css`, `icons.css`, `Animations.css`
- Google Fonts: Bebas Neue, Arsenal, Montserrat, Anton, Inter
- Material Symbols Icons

---

## Build & Run

### Prerequisites
- .NET 10.0 SDK
- PhotoApp.Api running (or AppHost)

### Build
```bash
cd PhotoApp
dotnet build PhotoApp/PhotoApp.Front/PhotoApp.Front/PhotoApp.Front.csproj
```

### Run (Development)
```bash
cd PhotoApp/PhotoApp.Front/PhotoApp.Front
dotnet run
# Frontend available at https://localhost:5002
```

### Run with Aspire
```bash
cd PhotoApp
dotnet run --project PhotoApp.AppHost
# Frontend orchestrated by AppHost at http://localhost:15080
```

### Required Configuration

`appsettings.Development.json`:
```json
{
  "VITE_API_URL": "http://localhost:5001"
}
```

---

## Service Registration (Program.cs)

```csharp
builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpClient("PhotoApp.Api", client =>
{
    client.BaseAddress = new Uri(baseUrl);
})
.AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddScoped<ApiConnection>();
builder.Services.AddSingleton(mapper);
builder.Services.AddMudServices();
```

---

## Important Notes

- Uses `PhotoApp.Common` for shared DTOs
- AutoMapper maps frontend models → DTOs
- Error messages in Polish
- Token storage via `AuthenticationHeaderHandler`
- Uses `AddServiceDefaults()` for Aspire integration
