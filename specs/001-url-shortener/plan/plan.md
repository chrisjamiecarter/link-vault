# Implementation Plan: URL Shortener Service

**Feature**: 001-url-shortener  
**Created**: 2026-04-30  
**Status**: Phase 0

## Technical Context

| Component | Technology | Rationale |
|-----------|-----------|----------|
| Runtime | .NET 10 | Latest |
| Frontend | Blazor Server | User requirement |
| UI Framework | BlazorBlueprintUI | User requirement |
| Database | SQL Server | User requirement |
| Orchestration | Aspire | User requirement |
| Architecture | Vertical Slice | User requirement |
| Data Access | Direct DbContext | User requirement - no repository pattern |

## Vertical Slice Architecture Guidelines

### Actual Solution Structure
```
src/
  Aspire.AppHost/               # Aspire orchestrator
  Aspire.ServiceDefaults/      # Shared config
  LinkVault.Core/               # Domain entities, interfaces, services, DbContext
    Entities/
      Link.cs
    Interfaces/
      ILinkService.cs
      IQrCodeService.cs
    Services/
      LinkService.cs           # Implements ILinkService
      QrCodeService.cs         # Implements IQrCodeService
    Data/
      LinkVaultDbContext.cs    # EF Core DbContext
      Migrations/              # EF Core migrations
  LinkVault.Web.Api/           # Minimal API (where features live!)
    Features/                   # Vertical slices
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
        LinkRedirectEndpoint.cs
      QrCodeGeneration/
        Commands/
        Responses/
        QrCodeEndpoint.cs
  LinkVault.Web.Blazor/        # Blazor Server UI
    Pages/
    Components/
```

### Slice Anatomy Rules
1. **Features live in LinkVault.Web.Api/Features/** - not in Core, not in Blazor
2. **Each slice contains**: Commands/, Queries/, Responses/, Endpoint class
3. **Never share DTOs** across feature boundaries
4. **Use static endpoint classes** with static Handle methods
5. **Validation belongs in slice** via DataAnnotations or FluentValidation
6. **Use Problem Details** for all error responses (RFC 9457)
7. **Direct DbContext consumption** - Commands and Queries inject LinkVaultDbContext directly, no repository pattern

### Data Access Pattern
```csharp
// In Commands/Queries - direct DbContext injection, no repository
// LinkVaultDbContext is in LinkVault.Core/Data/
public sealed class CreateShortUrlHandler
{
    private readonly LinkVaultDbContext _db;

    public CreateShortUrlHandler(LinkVaultDbContext db)
    {
        _db = db;
    }

    public async Task<ShortUrlResponse> HandleAsync(
        CreateShortUrlCommand command,
        CancellationToken ct)
    {
        // Direct DbContext usage - no repository
        var link = new Link { ... };
        _db.Links.Add(link);
        await _db.SaveChangesAsync(ct);
        // ...
    }
}
```

### Blazor Server Integration
- LinkVault.Web.Blazor calls LinkVault.Web.Api endpoints via HTTP
- Or Blazor can inject LinkVault.Core services directly
- Do NOT put Blazor code inside feature slices
- Use direct service injection, not MediatR

## Phase 0: Research

### Research Tasks

- [ ] Research BlazorBlueprintUI integration with .NET 10
- [ ] Research vertical slice architecture patterns in ASP.NET Core
- [ ] Research SQL Server + EF Core integration with Aspire
- [ ] Research case-insensitive short code generation
- [ ] Research QR code external API integration patterns

### Best Practices to Document

- Blazor Blueprint UI component usage
- Vertical slice folder structure
- Aspire service configuration for SQL Server
- Load testing tooling (e.g., k6, NBomber, or Azure Load Testing)

## Phase 1: Design

### Data Model

**Entity: Link**
- Id: GUID (PK)
- OriginalUrl: string (required, validated)
- ShortCode: string (unique, indexed, case-insensitive)
- CreatedAt: DateTime
- ExpiresAt: DateTime (optional, default: CreatedAt + 30 days)
- IsActive: boolean
- QrCodeUrl: string (optional)

### Key Design Decisions Needed

1. Short code generation algorithm (case-insensitive)
2. QR code API integration (with fallback)
3. Vertical slice folder structure
4. Aspire appHost configuration

## Constitution Check

- [ ] All tech stack choices documented
- [ ] All design decisions rationale provided
- [ ] Vertical slice directory structure defined for each feature
- [ ] Request/Response DTOs are explicit and separate from entities
- [ ] Endpoint pattern documented (static class with Handle method)
- [ ] Validation strategy defined (DataAnnotations or FluentValidation)
- [ ] Error responses use Problem Details format