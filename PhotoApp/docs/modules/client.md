# AGENTS_CLIENT.md - PhotoApp.Client (DEPRECATED)

## Overview

**STATUS**: This project is **deprecated and scheduled for removal**.

PhotoApp.Client was the original **Blazor WebAssembly** frontend, replaced by **PhotoApp.Front**.

---

## Why Deprecated?

| Old (PhotoApp.Client) | New (PhotoApp.Front) |
|-----------------------|----------------------|
| Pure WebAssembly | Hybrid Server + WASM |
| Client-side only | SSR capabilities |
| Separate API layer | Integrated with AppHost |
| Legacy structure | Modern Aspire-ready |

---

## Current State

Still exists in solution but:
- **Not referenced** by AppHost
- **Not built** in current workflow
- **Kept for reference** until full migration

### File Structure (Preserved)
```
PhotoApp.Client/
├── Connection/           # API clients
├── Models/              # Duplicated models
├── Layout/              # Old layouts
├── Pages/               # Old pages
├── Program.cs           # WASM entry point
└── _Imports.razor
```

---

## Migration Status

All components migrated to `PhotoApp.Front`:
- ✅ Pages → `PhotoApp.Front/Components/Pages/`
- ✅ Connection → `PhotoApp.Front/Connection/`
- ✅ Models → Use `PhotoApp.Common` instead

---

## Action Required

**Delete this project** after confirming:
1. All pages functional in PhotoApp.Front
2. All API clients work correctly
3. No dependencies reference PhotoApp.Client

### Steps to Remove
```bash
# 1. Remove from solution
dotnet sln remove PhotoApp/PhotoApp.sln \
  PhotoApp/PhotoApp.Client/PhotoApp.Client.WebAssembly.csproj

# 2. Delete directory
rm -rf PhotoApp/PhotoApp.Client/

# 3. Commit changes
git add -A && git commit -m "Remove deprecated PhotoApp.Client"
```

---

## Important Notes

- **Do not make changes** to this project
- **Do not add new features** here
- Reference `PhotoApp.Front` for current implementation
- This file exists as a reminder for cleanup
