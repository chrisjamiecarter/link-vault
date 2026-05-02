# Implementation Plan: URL Shortener Service

**Feature**: 001-url-shortener  
**Created**: 2026-04-30  
**Status**: Phase 0

## Technical Context

| Component | Technology | Rationale |
|-----------|-----------|----------|
| Runtime | .NET 10 | Latest preview/beta |
| Frontend | Blazor Server | User requirement |
| UI Framework | BlazorBlueprintUI | User requirement |
| Database | SQL Server | User requirement |
| Orchestration | Aspire | User requirement |
| Architecture | Vertical Slice | User requirement |

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