# Coding Standards - Zasady pisania kodu

Ten dokument zawiera zasady i konwencje obowiązujące w projekcie PhotoApp. Dotyczy zarówno developerów jak i agentów AI.

---

## ✅ DO's - Rób

### Ogólne

- **Używaj async/await** dla wszystkich operacji I/O (baza danych, sieć, pliki)
- **Sufiks Async** dla metod asynchronicznych: `GetUserAsync()`, `SaveDataAsync()`
- **Primary constructors** (C# 12): `public class UserService(UserRepository repo)`
- **Nullable reference types** - bądź explicite co może być null
- **Zwracaj null** dla "not found" zamiast rzucać wyjątki

### Nazewnictwo

| Element | Konwencja | Przykład |
|---------|-----------|----------|
| Klasy/Metody | PascalCase | `UserService`, `GetUserByIdAsync` |
| Pola prywatne | camelCase z `_` | `_userRepository`, `_config` |
| Parametry | camelCase | `userName`, `projectId` |
| Właściwości | PascalCase | `UserName`, `ProjectId` |
| Stałe | PascalCase | `MaxUploadSize` |
| Interfejsy | PascalCase z `I` | `IUserRepository` |
| DTO | PascalCase + `Dto` | `UserModelDto` |
| Enums | PascalCase | `SystemRole`, `PhotoType` |

### Struktura plików

```csharp
// Plik: PhotoApp.Api/Service/UserService.cs
using System;
using PhotoApp.Api.DbObjects;
using PhotoApp.Api.Repository;

namespace PhotoApp.Api.Service
{
    public class UserService
    {
        // Implementacja
    }
}
```

### Importy

Grupuj w kolejności:
1. **System** - `using System;`, `using System.Collections.Generic;`
2. **Third-party** - `using Microsoft.EntityFrameworkCore;`, `using AutoMapper;`
3. **Project** - `using PhotoApp.Api.DbObjects;`, `using PhotoApp.Api.Repository;`

### Obsługa błędów

```csharp
// ✅ DOBRZE - zwróć null dla not found
public async Task<User?> GetUserByIdAsync(Guid id)
{
    var user = await _repository.FindAsync(id);
    return user; // może być null
}

// ❌ ŹLE - nie rzucaj wyjątków dla błędów biznesowych
public User GetUserById(Guid id)
{
    var user = _repository.Find(id);
    if (user is null)
        throw new UserNotFoundException(id); // DON'T DO THIS
}
```

### API Controllers

```csharp
[ApiController]
[Route("project")]
public class ProjectController : ControllerBase
{
    [HttpGet("user/{username}/projects")]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjectsAsync([FromRoute] string username)
    {
        var projects = await _service.GetProjectsByUserAsync(username);
        return Ok(projects);
    }
}
```

### Praca z bazą danych

```csharp
// ✅ DOBRZE - zawsze async
public async Task<User?> GetUserAsync(string username)
{
    return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
}
```

---

## ❌ DON'Ts - Nie rób

### Ogólne

- **Nie rzucaj wyjątków** dla błędów biznesowych (not found, invalid input)
- **Nie używaj `var`** gdy typ nie jest oczywisty z kontekstu
- **Nie duplikuj kodu** - wyodrębnij do wspólnych metod
- **Nie zostawiaj `TODO`** bez opisu: `// TODO: fix this` → `// TODO(issue#123): fix null reference`
- **Nie commituj secrets** - keys, passwords, tokens do .gitignore

### Nazewnictwo

- **Nie używaj** polskich znaków w kodzie (poza komentarzami)
- **Nie używaj** skrótów: `usr` → `user`, `usrnm` → `username`
- **Unikaj** jednoliterowych zmiennych: `i`, `j` acceptable w pętlach

### Importy

```csharp
// ❌ ŹLE - brakuje organizacji
using System.Text.Json;
using PhotoApp.Api.Service;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

// ✅ DOBRZE - pogrupowane
using System;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using AutoMapper;

using PhotoApp.Api.Service;
```

### Null handling

```csharp
// ❌ ŹLE - głębokie zagnieżdżenie
if (user != null)
{
    if (user.Profile != null)
    {
        if (user.Profile.Settings != null)
        {
            // głęboko...
        }
    }
}

// ✅ DOBRZE - wczesny return / pattern matching
if (user?.Profile?.Settings is null)
{
    return null;
}
// teraz możesz bezpiecznie użyć user.Profile.Settings
```

### ApiConnection (Frontend)

```csharp
// ❌ ŹLE - brak obsługi błędów
public async Task<User> GetUser(Guid id)
{
    return await _httpClient.GetFromJsonAsync<User>($"/api/users/{id}");
}

// ✅ DOBRZE - używaj ApiResult
public async Task<ApiResult<UserDto>> GetUser(Guid id)
{
    return await SendGetRequestWithoutData<UserDto>($"/users/{id}");
}
```

---

## 🔧 Konwencje specyficzne dla technologii

### C# / .NET

- Wersja: **.NET 10.0** (Preview)
- Używaj **primary constructors** (C# 12)
- Włącz **nullable reference types**: `<Nullable>enable</Nullable>`
- Używaj **implicit usings** dla redukcji boilerplate

### Go (ImageProcessor)

- Wersja: **Go 1.21+**
- Nazwy plików: **lowercase z underscores**: `image_processor.go`
- Exported: **PascalCase**: `func ProcessImage()`
- Private: **camelCase**: `func processImage()`

```go
// ✅ DOBRZE
func ProcessImage(buffer []byte) ([]byte, error) {
    if buffer == nil {
        return nil, errors.New("buffer is nil")
    }
    // processing...
}

// ❌ ŹLE
func process_image(buf []byte) []byte { // brak obsługi błędów
```

### Blazor / Razor

- Komponenty: **PascalCase**: `UserProfile.razor`
- Kod w komponencie: `@code { }` zamiast osobnego .cs
- Używaj **MudBlazor** dla UI

---

## 📝 Przykłady

### Poprawny Service

```csharp
public class ProjectService(
    IMapper mapper,
    UserRepository userRepository,
    ProjectRepository projectRepository,
    ProjectFolderRepository folderRepository)
{
    public async Task<Project?> GetProjectByIdAsync(Guid id)
    {
        var project = await projectRepository.GetByIdAsync(id);
        if (project is null)
        {
            return null;
        }
        
        project.MainFolder = await folderRepository.GetByIdAsync(project.MainFolderId);
        return project;
    }
}
```

### Poprawny API Client

```csharp
public partial class ApiConnection
{
    public async Task<ApiResult<ServerAuthResponse>> LoginAsync(UserModel user)
    {
        var url = "auth/login";
        var userDto = _mapper.Map<UserModelDto>(user);
        return await SendPostRequest<ServerAuthResponse, UserModelDto>(url, userDto);
    }
}
```

---

## ⚠️ Ważne zasady

1. **Testuj async/await** - nigdy nie blokuj wątku
2. **Loguj błędy** z odpowiednim poziomem (Warning/Error)
3. **Walidacja** - waliduj dane wejściowe na granicach systemu
4. **Konsystencja** - trzymaj się ustalonych konwencji
5. **Czytelność** - kod jest czytany częściej niż pisany

---

## 📖 Więcej informacji

- [Architektura](ARCHITECTURE.md)
- [Moduł API](modules/api.md)
- [Moduł Frontend](modules/front.md)
