# ADR 0003: CI with locked restores and a protected main branch

## Context
Some packageversions float (for example `10.0.*`), so two restores can resolve differently. 
The repository is public and should show that the project builds and passes its tests
from a clean machine.

## Decision
- **CI:** one GitHub Actions workflow (`ci`) runs on every pull request and on every
  push to `main`. A single job, `build-test`, runs on `ubuntu-latest` and does a locked
  restore, a Release build (warnings are errors), and the full test suite. The
  integration tests start a throwaway PostgreSQL container with Testcontainers.
- **Least privilege and cost control:** the workflow has `contents: read` permission,
  cancels superseded runs, has a 15-minute timeout, and uses no secrets. It has no
  path filters, so the required check always reports.
- **Reproducible restores:** NuGet lock files (`RestorePackagesWithLockFile`) are
  committed for every project. CI restores with `--locked-mode`, so a mismatch between
  projects and lock files fails the build. The SDK is pinned in `global.json` and
  local tools in `dotnet-tools.json`.
- **Protected main:** a repository ruleset on the default branch requires a pull
  request (zero approvals), requires the `build-test` status check, allows squash
  merges only, blocks force pushes and branch deletion, and has no bypass actors.
- **Workflow:** GitHub flow. Short-lived branches, one pull request per change,
  squash merge, head branches deleted after the merge.

## Consequences
- `main` only changes through pull requests that built and passed all tests, so it is
  always in a releasable state.
- Package changes appear as lock file diffs in pull requests, so they are visible and
  reviewed. The cost is larger diffs and occasional merge conflicts in lock files.
- Pull requests are reviewed by their author, so review quality depends on reading the
  diff carefully, especially AI-generated code. The automated gate backs this up but
  does not replace it.
- Every run starts on a clean machine and pulls the Postgres image, so it is slower than
  a local run. The integration tests also need Docker locally.
- Action versions referenced by major tags can change underneath the workflow.
- Docs-only changes still wait for the full check, because the check is required and the
  workflow has no path filters.

## Verification
The controls were tested by trying to break them. A pull request with a failing test
had its merge button disabled, and a direct push to `main` was rejected. 
