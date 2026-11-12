\# ADR 0001: Modular monolith first, extract services later



\## Status

Accepted



\## Context

LedgerFlow is a payment orchestration platform built by one developer on a small

budget. It must show sound design, be cheap to run, and keep money movements consistent.



\## Decision

Build a modular monolith. Each module (Orders, Payments, Ledger, Fulfillment) has a public

Contracts project and an internal Module project, its own database schema and its own

DbContext. Boundaries are enforced by architecture tests. Payments (and possibly Ledger)

will be extracted into separate services once the core flow works.



\## Alternatives considered

\- Classic monolith: faster to start but boundaries erode and later extraction is costly.

\- Microservices from the start: independent scaling, but high operational cost and

&#x20; distributed-transaction complexity for a single developer.



\## Consequences

\- Simple local development, testing and deployment; consistent transactions within a module chain.

\- Boundaries depend on discipline and architecture tests.

\- Modules scale and fail together until extracted.

\- Extraction later is mostly mechanical, since modules already communicate only through contracts.

\- Boundaries are enforced by LedgerFlow.ArchitectureTests, which checks assembly references

