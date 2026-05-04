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
- **Implementation**: Each feature owns its Commands, Queries, Responses, and Endpoint class

#### Recommended Folder Structure
```
src/
  Aspire.AppHost/              # Aspire orchestrator
  Aspire.ServiceDefaults/     # Shared config
  LinkVault.Core/              # Domain entities, interfaces, services, DbContext
    Entities/
    Interfaces/
    Services/                  # Business logic services
    Data/
      LinkVaultDbContext.cs   # EF Core DbContext
      Migrations/             # EF Core migrations
  LinkVault.Web.Api/           # Minimal API - vertical slices live here!
    Features/                  # Vertical slices
      UrlShortening/
        Commands/
          CreateShortUrlCommand.cs
          CreateShortUrlCommandValidator.cs
        Queries/
        Responses/
          ShortUrlResponse.cs
        UrlShorteningEndpoint.cs
      LinkRedirection/
        Queries/
          RedirectQuery.cs
          RedirectQueryHandler.cs
        RedirectEndpoint.cs
      QrCodeGeneration/
        Commands/
        Responses/
        QrCodeEndpoint.cs
  LinkVault.Web.Blazor/       # Blazor Server UI (separate project)
    Pages/
    Components/
  LinkVault.Tools.DatabaseMigrator/  # Migration Worker Service
```

#### Direct DbContext Pattern (No Repository)
```csharp
// In LinkVault.Web.Api/Features/UrlShortening/Commands/CreateShortUrlHandler.cs
public sealed class CreateShortUrlHandler
{
    private readonly LinkVaultDbContext _db;

    public CreateShortUrlHandler(LinkVaultDbContext db)
    {
        _db = db;  // Direct DbContext injection - no repository!
    }

    public async Task<ShortUrlResponse> HandleAsync(
        CreateShortUrlCommand command,
        CancellationToken ct)
    {
        // Direct EF Core query - no repository abstraction
        var existing = await _db.Links
            .FirstOrDefaultAsync(l => l.OriginalUrl == command.OriginalUrl, ct);

        if (existing is not null)
        {
            return new ShortUrlResponse(existing.ShortCode, ...);
        }

        var link = new Link { OriginalUrl = command.OriginalUrl, ... };
        _db.Links.Add(link);
        await _db.SaveChangesAsync(ct);

        return new ShortUrlResponse(link.ShortCode, ...);
    }
}
```

**Key differences from traditional pattern:**
- No ILinkRepository interface
- No LinkRepository implementation
- Commands and Queries inject `LinkVaultDbContext` directly
- Access ` _db.Links` directly for queries

#### Endpoint Class Pattern
```csharp
// Features/UrlShortening/UrlShorteningEndpoint.cs
public static class UrlShorteningEndpoint
{
    public static RouteGroupBuilder MapUrlShorteningEndpoints(
        this RouteGroupBuilder group)
    {
        group.MapPost("/", CreateShortUrl)
             .WithName("CreateShortUrl")
             .Produces<ShortUrlResponse>(StatusCodes.Status201Created);

        return group;
    }

    public static async Task<IResult> CreateShortUrl(
        CreateShortUrlRequest request,
        ILinkService linkService,
        CancellationToken ct)
    {
        var result = await linkService.CreateShortUrlAsync(request, ct);
        return result.Match(
            success => Results.Created($"/{success.ShortCode}", success),
            failure => Results.Problem(failure.Message, statusCode: failure.StatusCode));
    }
}
```

#### Key Patterns
- **Static Handle methods** - each endpoint is a static class with static Handle
- **Explicit DTOs** - Request/Response DTOs separate from domain entities
- **Problem Details** - all errors use RFC 9457 format
- **Feature isolation** - features do not share DTOs
- **Validation inline** - validators are co-located with commands

## Best Practices

### 1. BlazorBlueprintUI Integration
- Install via NuGet package
- Configure in Program.cs
- Use component library for common UI patterns
- Follow their component composition patterns

### 2. Vertical Slice in ASP.NET Core (Minimal API)
- Group by feature (UrlShortening)
- Each feature has: Commands, Queries, Responses, Endpoint class
- Use direct service injection (not MediatR)
- Commands/Queries inject LinkVaultDbContext directly
- Map endpoints via MapGroup in Program.cs

### 3. Direct DbContext Consumption
- Commands inject LinkVaultDbContext directly (no repository)
- Queries inject LinkVaultDbContext directly (no repository)
- Use AsNoTracking() for read queries
- Use async LINQ methods with CancellationToken
- Keep DbContext scoped (default for web apps)

### 4. Case-Insensitive Short Codes
- Store ShortCode as uppercase in database
- Normalize input to uppercase on lookup
- Use unique constraint on uppercase version - Index: UPPER(ShortCode) for fast lookups

### 5. QR Code Integration
- Primary: External API (configured via env var)
- Fallback: Local QRCoder library
- Retry pattern: Primary → fallback on failure
- Health check both endpoints

### 6. SQL Server + Aspire + DatabaseMigrator
- Use Aspire.Hosting.SqlServer (AppHost) and Aspire.Microsoft.EntityFrameworkCore.SqlServer (Service)
- Use LinkVault.Tools.DatabaseMigrator (Migration Worker Service pattern)
- Migrator waits for SQL Server container before running migrations
- Handles ephemeral container timing issues on first start
- Configure DbContext in LinkVault.Web.Api Program.cs
- Health check endpoint verifies database connectivity

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