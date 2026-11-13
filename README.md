# LedgerFlow
![ci](https://github.com/alpbsra/ledgerflow/actions/workflows/ci.yml/badge.svg)

A payment orchestration platform: customers place orders, payments are processed through a mock provider, and every money movement is recorded in an immutable double-entry ledger. A fulfillment step adds a logistics flavor.

## Architecture

LedgerFlow is a **modular monolith**: one application and one deployment, divided into modules with strict boundaries.

- Each module has a public `Contracts` project and an internal `Module` project.

- A module may reference another module's `Contracts` project only, never its `Module` project.

- Each module owns one PostgreSQL schema and one `DbContext`, with its own migration history.

- There are no cross-schema joins or foreign keys. Modules exchange data through contracts.

- Boundaries will be enforced by architecture tests in CI.

Payments (and possibly the ledger) will be extracted into separate services once the core flow works. The reasoning is recorded in the ADRs below.

## Tech stack

**Today:** .NET 10, ASP.NET Core minimal APIs, EF Core 10, PostgreSQL 17 with pgvector, Docker Compose.

**Planned:** xUnit and Testcontainers, GitHub Actions, AWS (EC2, SQS, ECR, SSM Parameter Store), Terraform, OpenTelemetry, `Microsoft.Extensions.AI`.

## Project structure

```
ledgerflow/
├── .github/                      # Copilot instructions (CI workflows to come)
├── docs/adr/                     # Architecture decision records
├── src/
│   ├── LedgerFlow.Host/          # ASP.NET Core entry point, wires modules together
│   ├── LedgerFlow.SharedKernel/  # Small shared building blocks
│   └── Modules/
│       └── Orders/
│           ├── Orders.Contracts/ # Public surface of the module
│           └── Orders.Module/    # Domain, infrastructure, API (internal)
├── docker-compose.yml            # Local PostgreSQL
├── Directory.Build.props         # Shared build settings (warnings are errors)
└── global.json                   # Pinned SDK version
```

## Run locally

### Prerequisites

- .NET SDK 10 (the exact version is pinned in `global.json`)

- Docker Desktop (or Docker Engine with Compose)

- Git

### Steps

1. **Clone and restore tools**

   ```powershell
   git clone https://github.com/YOUR-USERNAME/ledgerflow.git
   cd ledgerflow
   dotnet tool restore
   ```

   `dotnet tool restore` installs the EF Core command-line tool at the version pinned in `.config/dotnet-tools.json`.

2. **Start PostgreSQL** (pgvector image)

   ```powershell
   docker compose up -d
   docker compose ps
   ```

   Wait until the `ledgerflow-postgres` container shows `healthy`.

3. **Apply the database migrations**

   ```powershell
   $env:ASPNETCORE_ENVIRONMENT = "Development"
   dotnet ef database update --project src/Modules/Orders/Orders.Module --startup-project src/LedgerFlow.Host
dotnet ef database update --project src/Modules/Orders/Orders.Module --startup-project src/LedgerFlow.Host --context OrdersDbContext
dotnet ef database update --project src/Modules/Payments/Payments.Module --startup-project src/LedgerFlow.Host --context PaymentsDbContext
   ```

   Each module owns its own schema and migration history, so this creates the `orders` schema. Further modules will add their own command here.


4. **Run the API**

   ```powershell
   dotnet run --project src/LedgerFlow.Host
   ```

   The console prints the listening URL (for example `http://localhost:5165`).

5. **Try it**

   Open `src/LedgerFlow.Host/orders.http` in Visual Studio or VS Code (REST Client extension) and send the requests, or check the module at `/orders/ping`.

### Stop and clean up

```powershell
docker compose stop          # stop the database, keep the data
docker compose down -v       # remove the container and delete the data
```

### Run the tests

```powershell
dotnet test
```

Docker must be running: the integration tests start a throwaway PostgreSQL container with Testcontainers. The `global.json` file selects the Microsoft.Testing.Platform runner, which xUnit v3 uses.

### Notes

- The database credentials in `docker-compose.yml` and `appsettings.Development.json` are throwaway values for local development only. Real secrets are never committed to this repository.

- If port 5432 is already in use on your machine, change the port mapping in `docker-compose.yml` and the connection string in `appsettings.Development.json` to match.

## API

| Method | Path | Description | Responses |
|---|---|---|---|
| `POST` | `/orders` | Create an order | `201` with a `Location` header, `400` for invalid input |
| `GET` | `/orders/{id}` | Get an order by id | `200`, `404` |
| `GET` | `/orders/ping` | Module health check | `200` |

Example request:

```json
POST /orders
{
  "totalAmount": 125.50,
  "currency": "EUR"
}
```

## Conventions

- Warnings are treated as errors (`Directory.Build.props`).

- Tests are written before code where practical.

- Decisions that are hard to reverse get an ADR in `docs/adr`.

- EF Core and Npgsql package versions match the .NET major version.

- Changes go through short-lived branches and pull requests. `main` is always working; CI must pass before merging, and merges are squashed.

## Known limitations

- Payments does not verify that the referenced order exists. The two modules share no foreign key or code reference by design (see ADR 0001); the check will be added through a contract or an event.

## How AI is used in this project

GitHub Copilot is used as an assistant, guided by `.github/copilot-instructions.md`. Every suggestion is reviewed, built and tested before it is committed. Notes on where the assistant was wrong, and how it was corrected, will be added here as the project grows.