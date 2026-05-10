# Quickstart: URL Shortener Service

## Prerequisites

- .NET 10 SDK (preview/beta)
- SQL Server (local or container)
- Visual Studio 2025 or VS Code

## Local Development Setup

### 1. Clone and Navigate
```bash
cd link-vault
```

### 2. Restore Packages
```bash
dotnet restore
```

### 3. Configure Environment
Create `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=LinkVault;User Id=sa;Password=your-password;TrustServerCertificate=true"
  },
  "ExternalQrCodeApi": {
    "BaseUrl": "https://api.qrcode.example.com",
    "ApiKey": "your-api-key"
  }
}
```

### 4. Run Database Migrations
```bash
dotnet ef database update
```

### 5. Run with Aspire
```bash
dotnet run --project src/LinkVault.AppHost
```

### 6. Open Browser
Navigate to: http://localhost:5000

## Project Structure

```
src/
  Aspire.AppHost/              # Aspire orchestrator
  Aspire.ServiceDefaults/     # Shared config
  LinkVault.Core/             # Domain entities, interfaces, services, DbContext
    Entities/
    Interfaces/
    Services/
    Data/
      LinkVaultDbContext.cs   # EF Core DbContext (in Core!)
      Migrations/             # EF Core migrations
  LinkVault.Web.Api/          # Minimal API - vertical slices live here!
    Features/                 # Vertical slices (feature-based)
      UrlShortening/
        Commands/
        Responses/
        UrlShorteningEndpoint.cs
      LinkRedirection/
        Queries/
        RedirectEndpoint.cs
      QrCodeGeneration/
        Commands/
        Responses/
        QrCodeEndpoint.cs
  LinkVault.Web.Blazor/       # Blazor Server UI
    Pages/
    Components/
  LinkVault.Tools.DatabaseMigrator/  # Migration Worker Service
```

**Key Points:**
- Vertical slice features live in `LinkVault.Web.Api/Features/`, not in Core
- Core contains domain entities, service interfaces, **AND** DbContext (LinkVault.Core/Data/)
- Commands and Queries consume **LinkVaultDbContext directly** (no repository pattern)
- Blazor project calls API endpoints or injects Core services directly
- DatabaseMigrator runs migrations on startup, waits for SQL Server container

## Common Commands

| Command | Purpose |
|---------|---------|
| `dotnet run --project src/LinkVault.AppHost` | Run with Aspire |
| `dotnet build` | Build solution |
| `dotnet test` | Run tests |

## First-Time Usage

1. Open http://localhost:5000
2. Enter a long URL (e.g., https://example.com/very-long-url)
3. Click "Shorten"
4. Copy the shortened URL
5. Download QR code if needed

## Troubleshooting

- **Database connection**: Check SQL Server is running
- **QR code fails**: Verify API key or check fallback is working
- **Port conflicts**: Check ports 5000, 5001 are available
- **Migration creation**: Package Manager Console `Add-Migration [Name] -Project LinkVault.Core -StartupProject LinkVault.Tools.DatabaseMigrator -OutputDir Database/Migrations`