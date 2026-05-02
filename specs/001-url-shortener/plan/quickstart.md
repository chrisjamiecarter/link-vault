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
  LinkVault.AppHost/              # Aspire orchestrator
  LinkVault.ServiceDefaults/       # Shared config
  LinkVault.Web/               # Blazor Server app
  LinkVault.Core/              # Domain logic
  LinkVault.Data/              # EF Core + DbContext
  LinkVault.MigrationService/  # Migration Worker Service
```

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