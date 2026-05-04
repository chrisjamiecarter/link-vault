# URL Shortener Service Specification

**Feature Name**: URL Shortener Service
**Short Name**: url-shortener
**Created**: 2026-04-26
**Status**: Draft (Simplified MVP - Clarified 2026-04-30)

## Overview

A simplified .NET 10 + Blazor URL shortening service for anonymous users. Users enter a long URL on a landing page and receive a shortened URL with QR code. Features include link expiration, QR code generation (external API with local fallback), and link redirection. No authentication required.

## Clarifications

### Session 2026-04-30

- Q: What landing page approach for anonymous users? → A: Simple landing page only (User enters URL → gets shortened link + QR code displayed, no retrieval)
- Q: What to do with User entity? → A: Remove User entity entirely, simplify Link to not require UserId
- Q: QR code generation approach? → A: External QR code service API with local fallback
- Q: Keep expiration or aliases? → A: Keep expiration only (remove aliases)
- Q: Deployment config? → A: Remove deployment config for now; focus on runnable local app

## Problem Statement

Users need to share long, unwieldy URLs in a more manageable format. This simplified service provides anonymous URL shortening with QR code generation and optional link expiration for time-sensitive content.

## User Scenarios

### Scenario 1: URL Shortening (Anonymous)
An anonymous user visits the landing page, enters a long URL, and receives a shortened URL and QR code instantly. No authentication required. The shortened URL redirects to the original.

### Scenario 2: Link Expiration
A user creates a link that expires after a specified date/time. Attempts to access the link after expiration return an error page.

### Scenario 3: QR Code Generation
A user requests a QR code for a shortened URL. The system generates a scannable QR code image via external API (with local fallback) that can be viewed or downloaded.

### Scenario 4: Link Redirection
An end-user accesses a shortened URL and is seamlessly redirected to the original target URL.

## Functional Requirements

### FR-00: Vertical Slice Architecture
The system MUST implement vertical slice architecture where each feature is self-contained with its own request/response DTOs, handlers, and endpoints.

**Architecture Requirements:**
- Each feature MUST have its own folder containing Commands, Queries, Responses, and Validators
- Features MUST NOT share request/response DTOs across slice boundaries
- Each endpoint MUST be implemented as a static class with a static Handle method
- Validation MUST be scoped to the feature using DataAnnotations or FluentValidation
- Error responses MUST use RFC 9457 Problem Details format

### FR-01: URL Shortening
The system MUST accept a valid long URL and generate a unique short code that redirects to the original URL.
- Short codes MUST be URL-safe alphanumeric strings
- Short codes MUST be case-insensitive (e.g., abc, Abc, aBc all resolve to same link)
- Short codes MUST be generated using a cryptographically-secure random algorithm (e.g., System.Security.Cryptography.RandomNumberGenerator or crypto Random.Shared) to ensure collision resistance
- System MUST handle duplicate URLs by returning existing short codes when identical

### FR-02: Link Expiration
The system MUST automatically set an expiration time for shortened links (default: 30 days).
- Expired links MUST return a user-friendly error message
- Expired links MUST NOT redirect to the target URL
- System MUST automatically expire links without manual intervention
- Future: Authenticated users may set custom expiration times

### FR-03: QR Code Generation
The system MUST generate QR codes for shortened URLs via external API.
- QR codes MUST contain the full shortened URL
- QR codes MUST be downloadable as PNG images
- QR codes MUST be scannable by standard QR readers
- System MUST fallback to local generation if external API fails

### FR-04: External QR Code API Integration
The system MUST integrate with an external QR code generation service.
- Primary: External API service
- Fallback: Local QR code generation using .NET library

## Non-Functional Requirements

### NFR-01: Performance
- Redirect latency MUST be under 100ms for cached URLs
- System MUST support 10,000+ concurrent redirect requests
- Shortened URL generation MUST complete in under 500ms

### NFR-02: Scalability
- Horizontal scaling to multiple instances
- Database partitioning for high traffic
- CDN integration for static assets

### NFR-03: Security
- HTTPS enforced for all endpoints
- Input validation for all URLs
- Rate limiting: maximum 100 requests per minute per IP address for URL creation; 1000 requests per minute for redirect operations
- Protection against SQL injection and XSS
- Error responses MUST use RFC 9457 Problem Details format
- Use endpoint filters for validation (not middleware)

### NFR-04: Availability
- 99.9% uptime target
- Graceful degradation during failures (QR fallback)
- Health check endpoints for monitoring

## Key Entities

### Entity: Link
- Id: GUID (primary key)
- OriginalUrl: string (required, validated)
- ShortCode: string (unique, indexed)
- CreatedAt: DateTime
- ExpiresAt: DateTime (optional)
- IsActive: boolean
- QrCodeUrl: string (optional, URL to QR code image)

## Key Architectural Components

### Request/Response DTOs
All HTTP operations MUST use explicit DTOs separate from domain entities:

| Feature | Request DTO | Response DTO |
|---------|--------------|--------------|
| URL Shortening | CreateShortUrlRequest | ShortUrlResponse |
| Link Redirection | RedirectRequest (route param) | RedirectResult (HTTP redirect) |
| QR Code | QrCodeRequest (short code) | QrCodeResponse (image URL) |

### Feature Slice Structure
Each feature MUST follow this structure under `LinkVault.Web.Api/Features/`:

```
LinkVault.Web.Api/
  Features/
    [FeatureName]/
      Commands/
        [Operation]Command.cs
        [Operation]CommandValidator.cs
      Queries/
        [Operation]Query.cs
        [Operation]QueryHandler.cs
      Responses/
        [Operation]Response.cs
      [FeatureName]Endpoint.cs  (maps routes to handlers)
```

### Data Access Pattern
- Commands and Queries MUST consume LinkVaultDbContext directly
- NO repository pattern - no ILinkRepository or LinkRepository
- Inject LinkVaultDbContext in handler constructors
- Use _db.Links directly for all data operations

### Endpoint Pattern
```csharp
// LinkVault.Web.Api/Features/UrlShortening/UrlShorteningEndpoint.cs
public static class UrlShorteningEndpoint
{
    public static RouteGroupBuilder MapUrlShorteningEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", CreateShortUrl)
             .WithName("CreateShortUrl")
             .Produces<ShortUrlResponse>(StatusCodes.Status201Created);
        return group;
    }

    public static async Task<IResult> CreateShortUrl(
        CreateShortUrlRequest request,
        LinkVaultDbContext db,
        CancellationToken ct)
    {
        // Direct DbContext usage - no repository
        var handler = new CreateShortUrlHandler(db);
        var result = await handler.HandleAsync(request, ct);
        return Results.Created($"/{result.ShortCode}", result);
    }
}
```

### Endpoint Pattern
All HTTP endpoints MUST follow this pattern:
```csharp
public static class CreateShortUrlEndpoint
{
    public static async Task<IResult> Handle(
        CreateShortUrlRequest request,
        ILinkService linkService,
        CancellationToken ct)
    {
        // Implementation
    }
}
```

## Success Criteria

| ID | Criterion | Metric |
|----|-----------|--------|
| SC-01 | Anonymous users can create shortened URLs | 100% success rate for valid inputs |
| SC-02 | Redirects complete successfully | 99.9% redirect success rate |
| SC-03 | Redirect latency is acceptable | Median redirect time under 100ms |
| SC-04 | Expired links are properly handled | 100% of expired links show error page |
| SC-05 | QR codes are generated via external API | QR code generated for each shortened URL |
| SC-06 | QR codes are scannable | 100% QR codes scan correctly |
| SC-07 | External QR API falls back to local | Service continues when external API fails |
| SC-08 | Application runs locally | Starts without errors using Aspire |

## Assumptions

1. Users have modern browsers supporting JavaScript and cookies
2. International users are supported with Unicode in target URLs
3. QR codes are generated at 300x300 pixels
4. Short codes are 8 characters by default
5. .NET 10 is the current preview/beta version
6. External QR code API service is configured via environment variable
7. Local QR code fallback uses QRCoder or similar .NET library

## Out of Scope

- User authentication and accounts
- Custom aliases/branded short links
- Analytics dashboard and click tracking
- Link management or retrieval UI
- Azure Functions
- Cloud deployment (deferred to future)
- Browser extensions or bookmarklets
- Mobile applications (iOS/Android)
- Desktop CLI application
- Enterprise SSO integration
- Custom domain names
- Bulk URL import/export
- Email-based link sharing
- SMS-based link sharing
- Webhook notifications
- API rate limiting tiers
- Team/organization accounts
- White-label options