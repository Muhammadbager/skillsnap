# Architecture notes

## Project boundaries

- `SkillSnap.Api`: ASP.NET Core controllers, Identity, JWT issuance and validation, EF Core persistence, caching, and migrations.
- `SkillSnap.Client`: standalone Blazor WebAssembly UI, form validation, typed domain services, authentication-state restoration, and session state.
- `SkillSnap.Api.Tests`: focused unit tests for validation, persistence metadata, and token claims.

## Data model

`ApplicationUser` is the security identity. It has one `PortfolioUser`, which owns many `Project` and `Skill` entities. Separating the Identity record from public profile data keeps account concerns out of portfolio responses.

## Request flow

1. Registration creates an Identity user, adds the `User` role, creates a portfolio, and returns a signed JWT.
2. Blazor persists the session and reconstructs its `ClaimsPrincipal` after a reload.
3. Public reads use cached, no-tracking DTO queries.
4. Protected writes send the JWT. The API validates it, compares the portfolio claim with the record owner, saves the change, and invalidates the relevant cache entry.

## Production considerations

Use a secret manager for the JWT key, HTTPS everywhere, a persistent relational database, distributed caching for a scaled API, structured telemetry, rate limiting on authentication endpoints, and an HTTP-only cookie/BFF approach for browser tokens.
