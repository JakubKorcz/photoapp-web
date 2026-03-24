# AGENTS_COMMON.md - PhotoApp.Common

## Overview

PhotoApp.Common is a **shared class library** containing models and enums used by both:
- **PhotoApp.Api** (backend)
- **PhotoApp.Front** (frontend)

**Purpose**: Enables type sharing without duplication.

**Target**: .NET 10.0  
**Dependencies**: None (pure models)

---

## Project Structure

```
PhotoApp.Common/
├── ModelsShared/              # DTOs
│   ├── UserModelDto.cs
│   ├── RegisterModelDto.cs
│   ├── ServerAuthResponse.cs
│   ├── ProjectDto.cs
│   ├── ProjectBaseInformationDto.cs
│   ├── FolderDto.cs
│   ├── MediaDto.cs
│   ├── DesignSettingsDto.cs
│   ├── ProjectSettingsDto.cs
│   └── MemoryInfoResponse.cs
├── EnumShared/               # Enums
│   ├── SystemRole.cs
│   ├── PhotoType.cs
│   ├── Language.cs
│   ├── Layout.cs
│   ├── FontWeight.cs
│   ├── Device.cs
│   └── ExpiryDateSelectOption.cs
├── ErrorDictionary.cs        # Shared error messages
└── PhotoApp.Common.csproj
```

---

## DTOs

### Location: `ModelsShared/`

| DTO | Properties | Purpose |
|-----|-----------|---------|
| `UserModelDto` | Email, Username, Password | Basic credentials |
| `RegisterModelDto` | (inherits UserModelDto) | Registration |
| `ServerAuthResponse` | AccessToken, Username | Auth response |
| `ProjectBaseInformationDto` | Id, ProjectName, CreatedAt, PhotoShootDate, ExpiryDate, Language, Password, IsPernament | Core project info |
| `ProjectDto` | BaseInfo + MainFolder + DesignSettings (mobile/desktop) | Complete project |
| `FolderDto` | Id, Name, Folders[], Medias[] | Hierarchical folders |
| `MediaDto` | Id, Name, Extension, Type, ObjectKey, SizeBytes, Width, Height, IsLiked, Description | Media file |
| `DesignSettingsDto` | CoverPhoto, FontFamily, FontSize, FontWeight, FontLayout | Design config |
| `ProjectSettingsDto` | (empty) | Future settings |
| `MemoryInfoResponse` | TotalMemoryInBytes, UsedMemoryInBytes, FreeMemoryInBytes | Memory info |

---

## Enums

### Location: `EnumShared/`

| Enum | Values | Purpose |
|------|--------|---------|
| `SystemRole` | Admin, Member, Guest | User authorization |
| `PhotoType` | Original, Preview, Thumbnail | Media format |
| `Language` | UA, PL, EN | UI language |
| `Layout` | Left, Center, Right | Text alignment |
| `FontWeight` | Light(300), Regular(400), Medium(500), Bold(700), Black(900) | Font weight |
| `Device` | Mobile(0), Desktop(1) | Client device |
| `ExpiryDateSelectOption` | Custom(0), TwoWeeks(14), OneMonth(30), TwoMonths(60) | Project expiry |

---

## Error Dictionary

```csharp
public static class ErrorDictionary
{
    private static readonly Dictionary<string, string> _messages = new()
    {
        { "ERR001", "BŁĄD : Niepoprawne dane logowania." },
        { "ERR002", "BŁĄD : Twoje konto jest zablokowane." },
        { "ERR003", "BŁĄD : Sesja wygasła, zaloguj się ponownie." }
    };
    
    public static string GetMessage(string code) => ...
}
```

---

## Build & Run

### Build (as dependency)
```bash
cd PhotoApp
dotnet build PhotoApp/PhotoApp.Common/PhotoApp.Common.csproj
```

This is a **class library** - it doesn't run standalone. Other projects reference it.

---

## Usage

### In Frontend
```csharp
var response = await ApiConnection.Login(new UserModelDto 
{ 
    Username = "user", 
    Password = "pass" 
});
```

### In Backend
```csharp
public ActionResult<ServerAuthResponse> Login([FromBody] UserModelDto request)
{
    return Ok(new ServerAuthResponse { AccessToken = "...", Username = "..." });
}
```

---

## Important Notes

- **No business logic** - pure data structures only
- Nullable reference types enabled
- No dependencies (transitive via ImplicitUsings)
- AutoMapper maps: Entities ↔ DTOs
- All projects that share types reference this library
