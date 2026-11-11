\# ADR 0002: Identifiers and persistence conventions



\## Status

Accepted (2026-11-11). The money representation is interim and will be superseded

when the Money value object is introduced.



\## Context

Every module stores data in PostgreSQL through EF Core. We need consistent rules for

identifiers, enums, money and timestamps, and for how much of an entity is exposed.

Payments make these choices expensive to change later, and identifiers will be

shared across modules and, after extraction, across services.



\## Decision

\- \*\*Identifiers:\*\* entity IDs are GUID version 7 (`Guid.CreateVersion7()`), generated in

&#x20; application code. EF Core is configured with `ValueGeneratedNever`.

\- \*\*Enums:\*\* stored as strings (`HasConversion<string>()`) with a maximum length.

\- \*\*Money:\*\* `decimal` with precision (18,2), never `double` or `float`. This is interim: a

&#x20; `Money` value object (amount and currency) replaces the separate columns in week 2.

\- \*\*Timestamps:\*\* `DateTimeOffset` in UTC, stored as `timestamptz`.

\- \*\*Entities:\*\* private setters, a private parameterless constructor for EF Core, and a

&#x20; public factory method that validates input. Entities and DbContexts are `internal`.

\- \*\*Migrations:\*\* one migration history table per module, inside that module's schema.



\## Alternatives considered

\- \*\*Database-generated integer identities:\*\* compact and sequential, but require a

&#x20; database round-trip before the ID is known, leak record counts, and collide when data

&#x20; moves between services.

\- \*\*Random GUIDs (version 4):\*\* the ID can also be created in code, but random values

&#x20; fragment the primary key index as the table grows.

\- \*\*ULIDs:\*\* time-ordered as well, but not a native database type and not needed when

&#x20; GUID v7 is available in the platform.

\- \*\*Enums stored as integers:\*\* smaller, but unreadable in the database and fragile when

&#x20; members are reordered.



\## Consequences

\- The ID exists before the row is saved, which helps with the transactional outbox and

&#x20; with publishing events that refer to the new entity.

\- IDs are roughly time-ordered, so inserts stay index-friendly, and they stay unique across

&#x20; modules and after extraction.

\- A v7 GUID reveals approximate creation time, which is acceptable for these entities.

\- String enums use more storage, and renaming an enum member needs a data migration.

\- Callers cannot create invalid entities, but the factory methods and private constructors

&#x20; add some ceremony.

