# SkillSnap — Capstone Submission

## 1. App and key features

SkillSnap is a full-stack portfolio site where developers create a profile, publish projects, and describe their skills. A Blazor WebAssembly client communicates with an ASP.NET Core Web API backed by EF Core and SQLite.

The three core features are:

1. **Projects and skills CRUD:** users can create, edit, delete, and browse portfolio content through validated forms. The API returns lean DTOs and clear HTTP errors.
2. **Authentication and authorization:** ASP.NET Core Identity manages accounts and password hashing. JWTs authenticate API requests, while role and ownership checks protect modifications.
3. **Performance and state:** common GET requests use in-memory caching, read queries use `AsNoTracking()` and server-side projections, and a scoped Blazor session service avoids repeatedly loading identity information.

## 2. Development challenges

The main challenges were coordinating authentication across two applications, modeling ownership, and keeping cached lists fresh.

- Identity, JWT validation, CORS, token persistence, and bearer headers all have to agree across the API and Blazor client. I solved this with one token service, a custom Blazor `AuthenticationStateProvider`, local-storage persistence, and a common authenticated-request helper.
- Projects and skills belong to a portfolio, while the login identity uses a string key. I modeled `ApplicationUser` and `PortfolioUser` separately with a unique link, then used a `portfolio_id` claim so protected writes can enforce ownership efficiently.
- Cached lists initially risk becoming stale after a write. Every successful POST, PUT, or DELETE now removes its list cache key, and the client reloads the relevant collection.

Copilot-style code assistance was used to scaffold repetitive service and component patterns, review middleware ordering, and refine validation and query projections. Each suggestion was checked against the build and automated tests.

## 3. Business logic, persistence, and state

API controllers coordinate application rules and delegate token creation to an injected service. Request models carry validation attributes; controllers normalize text, apply ownership rules, map entities to DTOs, invalidate caches, and return appropriate HTTP results. This keeps database entities out of the public contract.

`SkillSnapContext` inherits from `IdentityDbContext<ApplicationUser>`. A `PortfolioUser` has many `Project` and `Skill` records; both relationships use required foreign keys and cascade deletion. The database is versioned by an EF Core migration. Public reads use `AsNoTracking()`, ordering, and `Select()` projections so EF only retrieves required fields. Writes use tracked entities loaded by key.

On the client, `UserSessionService` is scoped for the lifetime of the WebAssembly app. `ApiAuthenticationStateProvider` restores a non-expired session from local storage and notifies Blazor when login or logout changes authentication. `ProjectService`, `SkillService`, and `ProfileService` isolate HTTP access; components load in `OnInitializedAsync` and refresh only the collection changed by a write.

## 4. Security

`ApplicationUser` derives from `IdentityUser`, and `SkillSnapContext` stores the Identity schema. Registration uses `UserManager`, so Identity validates and hashes passwords rather than the application handling credentials directly. Login uses `SignInManager.CheckPasswordSignInAsync` with lockout support.

The API issues a two-hour HMAC-SHA256 JWT containing subject, email, display name, role, and portfolio ID claims. The Blazor client stores the returned session in browser local storage, restores it through an authentication-state provider, and sends `Authorization: Bearer <token>` on protected requests. Expired sessions are discarded.

Controllers apply `[Authorize]` to all write routes. A user can modify only records matching the `portfolio_id` claim, while the `Admin` role can moderate any record. The administrative summary route uses `[Authorize(Roles = "Admin")]` explicitly. Model validation rejects malformed input, CORS allows only the configured local client origins, and the seed endpoint is disabled outside Development. The development signing key is replaceable through configuration or user-secrets and must not be used in production.

## 5. Performance improvements

- `IMemoryCache` stores public project and skill lists with a two-minute sliding expiration and ten-minute absolute cap. POST, PUT, and DELETE operations invalidate the corresponding key.
- Read queries use `AsNoTracking()`, database-side sorting, and DTO projections. This avoids change-tracking overhead, over-fetching, and reference cycles.
- `HttpClient` and all client services are dependency-injected and reused. The scoped session service keeps user and portfolio identity in memory, while page components reload only the data affected by a change.
- Cache hits and misses are logged for project reads, making the optimization observable during manual testing.

## Known limitations and next steps

- The current cache is process-local; a distributed cache would be needed for multiple API instances.
- Browser local storage is appropriate for this course demonstration, but a production deployment should consider a backend-for-frontend with secure, HTTP-only cookies.
- Image URLs are externally hosted. A future version could add authenticated object-storage uploads and image processing.
