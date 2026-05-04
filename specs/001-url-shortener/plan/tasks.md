# Tasks: URL Shortener Service

**Feature**: 001-url-shortener  
**Created**: 2026-04-30  
**Status**: Ready for Implementation

## Phase 1: Setup

### Goal
Initialize Aspire solution with Blazor Server and all required projects.

- [x] T001 Create Aspire solution structure with AppHost (Aspire.Hosting.SqlServer), ServiceDefaults, Core, Constants, Web, Api, and DatabaseMigrator projects.
- [x] T002 Configure .NET 10 target framework in all projects.
- [x] T003 Add BlazorBlueprintUI NuGet package to Web project.
- [x] T004 Add Aspire.Microsoft.EntityFrameworkCore.SqlServer package.
- [x] T005 Add QRCoder package to Core project (for local QR fallback)
- [x] T006 Configure Aspire appHost with SQL Server resource.
- [x] T007 Create solution slnx file.

## Phase 2: Foundational

### Goal
Create database context, entity, and core infrastructure.

**Dependencies**: Phase 1 must complete first

- [x] T008 Create abstract Entity in src/LinkVault.Core/Domain/Entity.cs
- [x] T008a Create Link entity in src/LinkVault.Core/Data/Link.cs
- [x] T008b Configure short code length (default: 8 characters) in ShortCodeGenerator
- [x] T009 Create DbContext in src/LinkVault.Data/LinkVaultDbContext.cs
- [x] T010 Configure EF Core with SQL Server in Program.cs
- [x] T011 Create initial EF Core migration
- [x] T012 Add connection string configuration for SQL Server
- [x] T013 Configure health checks with Aspire
- [x] T014 Add rate limiting middleware for endpoint protection
- [ ] T015 Add input validation/sanitization for XSS and SQL injection protection
- [ ] T015a Configure Problem Details in LinkVault.Web.Api Program.cs

### Phase 2a: Vertical Slice Foundation

### Goal
Establish vertical slice architecture pattern in LinkVault.Web.Api project.

**Dependencies**: Phase 2 must complete first

- [ ] T015b [VS] Create Features folder in LinkVault.Web.Api project
- [ ] T015c [VS] Create UrlShortening feature slice in LinkVault.Web.Api/Features/UrlShortening/
- [ ] T015d [VS] Create LinkRedirection feature slice in LinkVault.Web.Api/Features/LinkRedirection/
- [ ] T015e [VS] Create QrCodeGeneration feature slice in LinkVault.Web.Api/Features/QrCodeGeneration/
- [ ] T015f [VS] Define CreateShortUrlRequest DTO in UrlShortening/Commands/
- [ ] T015g [VS] Define ShortUrlResponse DTO in UrlShortening/Responses/
- [ ] T015h [VS] Define RedirectQuery DTO in LinkRedirection/Queries/
- [ ] T015g [VS] Define CreateShortUrlRequest DTO in UrlShortening/Commands/
- [ ] T015h [VS] Define ShortUrlResponse DTO in UrlShortening/Responses/
- [ ] T015i [VS] Define RedirectQuery DTO in LinkRedirection/Queries/
- [ ] T015j [VS] Configure Problem Details in Program.cs

## Phase 3: User Story 1 - URL Shortening

### Goal
Anonymous users can enter a URL on the landing page and receive a shortened URL.

**Independent Test Criteria**: Can create a shortened URL by entering a valid URL on the landing page.

- [ ] T016 [P] [US1] Create LinkGeneratorService in src/LinkVault.Core/Services/LinkGeneratorService.cs
- [ ] T017 [P] [US1] Create UrlShorteningHandler in src/LinkVault.Core/Features/UrlShortening/Commands/CreateShortUrlCommand.cs
- [ ] T018 [P] [US1] Create shortened URL response model in src/LinkVault.Core/Features/UrlShortening/Responses/ShortUrlResponse.cs
- [ ] T018a [P] [US1] Implement duplicate URL detection - return existing short code if OriginalUrl matches
- [ ] T019 [US1] Implement CreateShortUrl endpoint/route in Web project
- [ ] T019a [US1] Create UrlShorteningEndpoint.cs static class with Handle method
- [ ] T019b [US1] Map endpoint in Program.cs using MapGroup
- [ ] T020 [US1] Create landing page Blazor component in src/LinkVault.Web/Pages/Index.razor
- [ ] T021 [US1] Add basic styling with BlazorBlueprintUI in Index.razor

## Phase 4: User Story 2 - Link Expiration

### Goal
Links automatically expire after 30 days.

**Independent Test Criteria**: accessing an expired short URL displays an error page.

**Dependencies**: Phase 3 (US1) must complete first

- [ ] T022 [US2] Add expiration check logic in redirect flow
- [ ] T023 [US2] Create ExpiredLinkHandler in src/LinkVault.Core/Features/LinkExpiration/ExpiredLinkHandler.cs
- [ ] T023a [US2] Create LinkExpiration feature slice with Queries/ folder
- [ ] T023b [US2] Create LinkExpirationEndpoint.cs static class in LinkRedirection slice
- [ ] T024 [US2] Create error page Blazor component in src/LinkVault.Web/Pages/Error/Expired.razor

## Phase 5: User Story 3 - QR Code Generation

### Goal
Users receive a QR code image for their shortened URL.

**Independent Test Criteria**: QR code image is generated and displayed for each shortened URL.

**Dependencies**: Phase 3 (US1) must complete first

- [ ] T025 [P] [US3] Create IQrCodeService interface in src/LinkVault.Core/Interfaces/IQrCodeService.cs
- [ ] T026 [P] [US3] Create ExternalQrCodeService in src/LinkVault.Core/Services/ExternalQrCodeService.cs
- [ ] T027 [P] [US3] Create LocalQrCodeService (fallback) in src/LinkVault.Core/Services/LocalQrCodeService.cs
- [ ] T028 [US3] Implement QR code fallback pattern (external primary, local on failure)
- [ ] T028a [US3] Create QrCodeGeneration feature slice with Commands/, Responses/ folders
- [ ] T028b [US3] Create GenerateQrCodeCommand.cs in QrCodeGeneration/Commands/
- [ ] T028c [US3] Create QrCodeResponse.cs in QrCodeGeneration/Responses/
- [ ] T028d [US3] Create QrCodeEndpoint.cs static class with Handle method
- [ ] T029 [US3] Add QR code display to landing page in Index.razor
- [ ] T030 [US3] Add QR code download functionality in Index.razor

## Phase 6: User Story 4 - Link Redirection

### Goal
End-users can access shortened URL and be redirected to original URL.

**Independent Test Criteria**: Accessing /{shortCode} redirects to the original URL.

**Dependencies**: Phase 3 (US1) must complete first

- [ ] T031 [US4] Create redirect route in src/LinkVault.Web/Pages/{shortCode}.razor
- [ ] T031a [US4] Create RedirectQuery.cs in LinkRedirection/Queries/
- [ ] T031b [US4] Create RedirectQueryHandler.cs in LinkRedirection/Queries/
- [ ] T032 [US4] Implement redirect logic with expiration check
- [ ] T033 [US4] Add HTTP redirect response (302/301)

## Phase 7: Polish & Cross-Cutting

### Goal
Final integration and verification.

- [ ] T034 Run database migrations using Migration Worker Service pattern
- [ ] T035 Verify all user stories work independently
- [ ] T036 Verify application starts with Aspire dashboard
- [ ] T037 Apply BlazorBlueprintUI theming throughout
- [ ] T038 Add health check endpoint verification
- [ ] T039 Verify redirect latency meets <100ms requirement with load test
- [ ] T040 [VS] Verify all features follow vertical slice structure (Features/Commands/Responses/Endpoint.cs)
- [ ] T041 [VS] Verify all endpoints use static class with Handle method pattern
- [ ] T042 [VS] Verify all errors use Problem Details format (RFC 9457)

### Out of Scope (per spec.md)
- NFR-02 Scalability (horizontal scaling, DB partitioning, CDN) - deferred to future

## MVP Scope

**Recommended MVP Start**: Phase 1 + Phase 2 + Phase 3 (User Story 1)
- Core URL shortening functionality
- Can validate basic flow works before adding complexity

## Parallel Opportunities

The following tasks can run parallel (no dependencies between them):
- T016, T017, T018 (Core services and models)
- T025, T026, T027 (QR code services)

## Dependencies Summary

```
Phase 1 (Setup)
    ↓
Phase 2 (Foundational)
    ↓
Phase 3 (US1 - URL Shortening) ← Phase 4, 5, 6 depend on this
    ↓
Phase 4 (US2 - Expiration)
Phase 5 (US3 - QR Codes)
Phase 6 (US4 - Redirection)
    ↓
Phase 7 (Polish)
```

## Task Count

| Phase | Task Count |
|-------|-----------|
| Phase 1 | 7 |
| Phase 2 | 10 |
| Phase 2a (VS Foundation) | 8 |
| Phase 3 (US1) | 9 |
| Phase 4 (US2) | 5 |
| Phase 5 (US3) | 10 |
| Phase 6 (US4) | 5 |
| Phase 7 | 9 |
| **Total** | **63** |