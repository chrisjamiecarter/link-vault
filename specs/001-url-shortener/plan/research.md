# Research: URL Shortener Service

## Tech Stack Decisions

### .NET 10 Runtime
- **Decision**: Use .NET 10 preview/beta
- **Rationale**: User requirement
- **Alternative**: .NET 9 LTS (rejected - not requested)

### Blazor Server
- **Decision**: Use Blazor Server hosting
- **Rationale**: User requirement - simpler for this MVP than Blazor WebAssembly
- **Alternative**: Blazor WebAssembly (rejected for simplicity)
- **Note**: Server-prerendered components via .NET 10's streaming rendering

### BlazorBlueprintUI
- **Decision**: Use BlazorBlueprintUI for styling
- **Rationale**: User requirement
- **Note**: Need to verify .NET 10 compatibility

### SQL Server
- **Decision**: Use SQL Server database
- **Rationale**: User requirement
- **Integration**: Via EF Core with Aspire orchestration

### Aspire
- **Decision**: Use Aspire for local dev orchestration
- **Rationale**: User requirement
- **Components**: AppHost, ServiceDefaults, dashboard

### Vertical Slice Architecture
- **Decision**: Use vertical slice architecture
- **Rationale**: User requirement - organize by feature rather than layer

#### Recommended Folder Structure
```
src/
  Aspire.AppHost/                 # Aspire orchestrator
  Aspire.ServiceDefaults/       # Shared config
  LinkVault.Web.Blazor/                  # Blazor Server app
    Pages/
    Components/
  LinkVault.Core/                 # Domain logic
    Entities/
    Interfaces/
    Services/
    Features/
      UrlShortening/
        Commands/
        Queries/
        Responses/
  LinkVault.Data/                 # EF Core + DbContext
    Repositories/
    Migrations/
  LinkVault.MigrationService/       # Migration Worker Service (Aspire 13.x)
```

## Best Practices

### 1. BlazorBlueprintUI Integration
- Install via NuGet package
- Configure in Program.cs
- Use component library for common UI patterns
- Follow their component composition patterns

### 2. Vertical Slice in ASP.NET Core
- Group by feature (UrlShortening)
- Each feature has: Commands, Queries, Models, Responses
- Use MediatR or similar for request handling
- Shared infrastructure in sibling folders

### 3. Case-Insensitive Short Codes
- Store ShortCode as uppercase in database
- Normalize input to uppercase on lookup
- Use unique constraint on uppercase version - Index: UPPER(ShortCode) for fast lookups

### 4. QR Code Integration
- Primary: External API (configured via env var)
- Fallback: Local QRCoder library
- Retry pattern: Primary → fallback on failure
- Health check both endpoints

### 5. SQL Server + Aspire
- Use Aspire.Hosting.SqlServer (AppHost) and Aspire.Microsoft.EntityFrameworkCore.SqlServer (Service)
- Use Migration Worker Service pattern (Aspire 13.x official approach)
- Creates dedicated worker that waits for SQL Server container before running migrations
- Handles ephemeral container timing issues (container may not be ready on first start)

## External Dependencies

| Dependency | Purpose | Integration |
|------------|---------|-------------|
| QR Code API | Generate QR codes | HTTP client with retry |
| SQL Server | Data persistence | EF Core with Aspire |
| BlazorBlueprintUI | UI components | NuGet package |

## Questions for Future Investigation

1. BlazorBlueprintUI .NET 10 compatibility
2. Vertical slice vs minimal APIs vs controller pattern
3. QR code API provider options