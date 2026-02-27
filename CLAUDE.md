# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Episodes** is an ASP.NET Core Web API (C#, .NET 8) that tracks TV shows. It integrates with The Movie Database (TMDB) API. PostgreSQL is the database, managed with Liquibase migrations.

## Commands

### Build & Run
```bash
dotnet build Episodes.sln
dotnet run --project src/Episodes
```

### Tests
```bash
# Run all tests
dotnet test test/Episodes.Tests

# Run a specific test class
dotnet test test/Episodes.Tests --filter "ClassName=Episodes.Tests.Services.Tv.TvShowServiceTests"

# Run a specific test method
dotnet test test/Episodes.Tests --filter "Name~SearchTvShows_WhenSuccessful_ReturnsOk"

# Run with verbose output
dotnet test test/Episodes.Tests --verbosity detailed
```

### Local Database (PostgreSQL via Docker)
```bash
cd liquibase && docker-compose up
```
Connection string: `Host=localhost;Port=5432;Database=episodes;Username=episodes_user;Password=episodes_password`

## Architecture

### Layer Flow
`Controllers → ITvShowService → ITmdbClient → TMDB HTTP API`

Database access (EF Core) is via `ApplicationDbContext` but is not yet wired into service logic—entity models exist for future use.

### Key Files
- `src/Episodes/Controllers/TvController.cs` — REST endpoints (`GET /api/shows/search`)
- `src/Episodes/Services/Tv/TvShowService.cs` — Orchestrates TMDB calls, maps to internal DTOs
- `src/Episodes/Services/Tmdb/TmdbClient.cs` — HTTP client for TMDB API (Bearer token auth, snake_case JSON)
- `src/Episodes/Program.cs` — entry point, DI registration, HTTP resilience config (retry + circuit breaker), JSON snake_case naming
- `src/Episodes/Data/ApplicationDbContext.cs` — EF Core DbContext for `episodes` PostgreSQL schema
- `src/Episodes/Config/TmdbOptions.cs` — Bound from `"Tmdb"` config section

### Models Structure
- `Models/Tv/` — Internal DTOs returned by the API (`TvShowSearchResponse`, `TvSearchResult`)
- `Models/Tmdb/` — Raw TMDB API response shapes
- `Models/Entities/` — EF Core entity classes (Show, Season, Episode, User, UserShow, WatchedEpisode, etc.)

### Entry Points
- `Program.cs` — application entry point (standard ASP.NET Core hosting)

### Testing Conventions
- Framework: NUnit + xUnit, assertions via FluentAssertions, mocking via NSubstitute
- Tests mirror the `src/Episodes` folder structure under `test/Episodes.Tests/`
- Test naming pattern: `MethodName_WhenCondition_ExpectedResult`

### Configuration
- TMDB credentials and DB connection go in `appsettings.Development.json` (git-ignored)
- JSON serialization uses `JsonNamingPolicy.SnakeCaseLower` throughout
- `appsettings.*.json` files are excluded from git; use the development file locally
