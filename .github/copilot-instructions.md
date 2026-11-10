# LedgerFlow



Payment orchestration platform: orders, payments, double-entry ledger, fulfillment.

Portfolio project, built as a modular monolith that is later split into services.



## Stack

.NET 10, ASP.NET Core minimal APIs, EF Core, PostgreSQL, Docker Compose, xUnit, Testcontainers.



## Architecture rules

- Modules live in src/Modules/<Name>/ with a Contracts project (public) and a Module project (internal).
- A module may reference another module's Contracts project only, never its Module project.
- One Postgres schema and one DbContext per module.
- Money is a value object (amount + currency). Never use double or float for money.
- Module namespaces avoid reserved keywords: use `LedgerFlow.Orders`, not `LedgerFlow.Orders.Module` (analyzer rule CA1716). The assembly keeps the `.Module` suffix.
- Use EF Core and Npgsql packages whose major version matches the .NET target (10.x for net10.0). Do not install 11.x packages.


## Working rules

- Write the test first, then the code.

- Explain trade-offs before making large changes.

- Do not edit generated EF migrations by hand.

- Keep changes small and focused, one concern per commit.

- Warnings are errors. Fix them, do not suppress them.



## Learning mode

The developer is returning after a break and rebuilding confidence.

- Explain the reasoning behind suggestions, not only the code.

- Mention the relevant C# or .NET concept when it is not obvious.

- Prefer simple, readable solutions over clever ones.

