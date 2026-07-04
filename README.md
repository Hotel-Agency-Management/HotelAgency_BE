# HotelAgency_BE

Backend API for a hotel and travel agency booking platform, built with **ASP.NET Core 9.0**. Provides authentication, agency/hotel/room management, reservations, housekeeping tickets, payments, and reporting for a multi-role hospitality operation. Designed to pair with a separate SPA frontend (default `http://localhost:3000`).

## Tech Stack

- **Framework:** ASP.NET Core 9.0 Web API
- **Database:** MySQL 8.0 via EF Core 9.0 (Pomelo provider)
- **Auth:** ASP.NET Identity + JWT Bearer tokens (access + refresh tokens)
- **Background Jobs:** Hangfire (MySQL storage)
- **Email:** Azure Communication Services
- **File Storage:** Azure Blob Storage (photos, documents)
- **API Docs:** OpenAPI (`/openapi`)

## Architecture

Three-layer architecture:

```
Controllers → Services → Repositories → EF Core → MySQL
```

- **Controllers/** — HTTP endpoints. Split into `Admin/` (admin-only), `PublicApis/` (unauthenticated), and role-scoped controllers at the top level.
- **Services/** — Business logic, with `Interfaces/` and `Implementations/` split.
- **Repositories/** — EF Core data access, same interface/implementation split.
- **DTO/** — API contracts, organized per domain area.
- **Models/** — EF Core entity classes.
- **Strategies/** + **Factories/** — Strategy/Factory pattern for behavior that varies by user type (registration, profile shape, login response).
- **Filters/** — Action-level ownership/existence guard attributes (e.g. hotel/agency/ticket ownership checks).
- **Exceptions/** + **Middleware/** — Typed domain exceptions, all mapped to HTTP responses by a global exception-handling middleware.

See `CLAUDE.md` for full architectural notes and design pattern details.

## User Roles

Ten roles drive authorization throughout the API (`Constants/Roles.cs`):

`SUPER_ADMIN`, `AGENCY_OWNER`, `PROPERTY_MANAGER`, `FRONT_DESK_STAFF`, `HOUSEKEEPING_MANAGER`, `HOUSEKEEPING_EMPLOYEE`, `ACCOUNTANT`, `CUSTOMER_SUPPORT`, `AUDITOR`, `CUSTOMER`

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- MySQL 8.0
- (Optional) Azure Communication Services + Azure Blob Storage accounts for email/file features

### Setup

1. Clone the repository.
2. Configure `appsettings.Development.json` (or use `dotnet user-secrets`) with your own values for:
   - `ConnectionStrings:DefaultConnection` — MySQL connection string
   - `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` — JWT signing settings
   - `EmailOptions:ConnectionString`, `EmailOptions:Sender` — Azure Communication Services
   - `AzureBlobStorage:ConnectionString`, `AzureBlobStorage:ContainerName`
   - `AppLinks:BaseUrl` — frontend base URL used in email links
   - `AuthSettings:DefaultPassword`

   > **Never commit real credentials.** `appsettings.json` in this repo is a template — replace secrets locally or via environment variables/user-secrets before running.

3. Apply database migrations:

   ```bash
   dotnet ef database update
   ```

4. Run the API:

   ```bash
   dotnet run
   ```

### Common Commands

```bash
dotnet build                              # Build the project
dotnet run                                # Start development server
dotnet publish -c Release                 # Production build
dotnet ef migrations add <MigrationName>  # Create a new EF migration
dotnet ef database update                 # Apply pending migrations
```


## Development Notes

- **API docs:** available at `/openapi` in Development.
- **Hangfire dashboard:** available at `/hangfire` in Development only (background email jobs, etc.).
- **Emails are always async** — sent via Hangfire background jobs (`IEmailJobService`), never inline.
- Feature-specific design docs live under `plans/<feature-name>.md`.

## Project Structure

```
Booking/
├── Controllers/        # Admin/, PublicApis/, and role-scoped endpoints
├── Services/           # Interfaces/ + Implementations/
├── Repositories/       # Interfaces/ + Implementations/
├── Models/             # EF Core entities
├── DTO/                # API contracts per domain
├── Data/
│   ├── Context/        # ApplicationDbContext
│   └── Seeders/        # Plan, Role, RoomAmenity, RoomType seed data
├── Migrations/         # EF Core migrations
├── Strategies/         # Registration/Profile/LoginResponse strategies
├── Factories/          # Strategy factories
├── Clients/            # Email, Blob Storage, Email Job clients
├── Filters/            # Ownership/existence guard attributes
├── Exceptions/         # Typed domain exceptions
├── Middleware/         # Global exception handling
├── Configurations/     # Strongly-typed settings classes
├── Constants/          # Roles, claims, messages, app link paths
├── Enums/              # UserRole, ReservationStatus, TicketStatus, etc.
├── EmailTemplates/     # HTML email templates
├── Converter/          # Polymorphic JSON converters
├── Utils/              # Auth/TeamMember helpers
├── plans/              # Per-feature design docs
├── Program.cs          # DI wiring, middleware pipeline
 8080
```

## Contributing

See `CLAUDE.md` for architectural conventions (exception handling, DI patterns, role constants, etc.) before making changes.
