# SkillSnap

SkillSnap is a full-stack portfolio and project tracker built with Blazor WebAssembly, ASP.NET Core, EF Core, SQLite, Identity, and JWT authentication. It gives developers a polished public showcase and a protected studio for maintaining projects, skills, and profile details.

## Highlights

- Complete project and skill CRUD with validation, ownership checks, friendly UI states, and cache invalidation.
- ASP.NET Core Identity registration and login, signed JWTs, persisted Blazor sessions, and user/admin roles.
- EF Core SQLite persistence with explicit one-to-many relationships, migrations, lean DTO projections, and `AsNoTracking()` reads.
- `IMemoryCache` for common list endpoints with sliding and absolute expiration.
- Responsive, accessible Blazor interface with public discovery, login/register, and a protected portfolio studio.
- Automated tests for model validation, EF relationships, and JWT claims.

## Architecture

```text
Blazor WebAssembly (SkillSnap.Client)
        │ HTTPS + bearer JWT
        ▼
ASP.NET Core Web API (SkillSnap.Api)
        ├── Identity + role authorization
        ├── Controllers + DTO projections
        ├── IMemoryCache
        └── EF Core ──► SQLite
```

## Run locally

Prerequisite: [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0).

```powershell
dotnet tool restore
dotnet restore
dotnet ef database update --project SkillSnap.Api --startup-project SkillSnap.Api
dotnet run --project SkillSnap.Api
```

In a second terminal:

```powershell
dotnet run --project SkillSnap.Client
```

Open `http://localhost:5234`. The client expects the API at `http://localhost:5299`; these are the default HTTP profiles in the checked-in development launch settings.

To load the sample portfolio, start the API and run:

```powershell
curl.exe -X POST http://localhost:5299/api/seed
```

Demo login: `jordan@skillsnap.local` / `DemoPass1!`. The seed route is available only in Development.

## Configuration and security

No JWT signing key is committed to the repository. In Development, the API generates a temporary signing key each time it starts. To use a stable local key—or before any real deployment—store one outside source control:

```powershell
dotnet user-secrets init --project SkillSnap.Api
dotnet user-secrets set "Jwt:Key" "a-long-random-production-secret" --project SkillSnap.Api
```

Write endpoints require a valid JWT and enforce portfolio ownership; users in the `Admin` role can moderate any project or skill. CORS is restricted to the two local client origins.

## Verification

```powershell
dotnet build SkillSnap.sln
dotnet test SkillSnap.sln --no-build
```

See [SUBMISSION.md](SUBMISSION.md) for rubric-aligned peer-review answers and [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) for implementation notes.
