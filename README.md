# Sky Schedule

**Status: engineering beta.** A focused .NET 8 schedule-expression parser and UTC next-run calculator for predictable application scheduling boundaries.

## Implemented behavior

- parses five-field cron-style expressions: minute, hour, day-of-month, month, day-of-week
- supports `*`, exact numeric values, comma-separated values, ranges, and `*/N` step values
- validates field bounds before evaluation
- computes the first matching UTC minute strictly after a supplied timestamp
- uses a bounded search horizon instead of looping forever on impossible schedules
- emits machine-readable JSON from the CLI
- includes xUnit coverage for parsing, steps, ranges, invalid expressions, and deterministic next-run calculation
- CI verifies .NET 8 restore/build/tests, package vulnerability reporting, CLI behavior, Docker build, and non-root runtime packaging

## Run

```bash
dotnet restore tests/SkySchedule.Tests.csproj
dotnet test tests/SkySchedule.Tests.csproj -c Release
dotnet run --project CSharp-Cron-Manager.csproj -- '*/15 * * * *' '2026-08-24T10:07:59Z'
```

Example output:

```json
{"expression":"*/15 * * * *","after":"2026-08-24T10:07:59+00:00","nextRun":"2026-08-24T10:15:00+00:00","timezone":"UTC"}
```

## Container

```bash
docker build -t sky-schedule .
docker run --rm sky-schedule '0 9 * * 1-5' '2026-08-24T10:07:59Z'
```

The runtime image executes as UID `10001` through the unprivileged `app` user.

## SKYCOIN4444 integration

Sky Schedule can be used as a deterministic scheduling primitive for workers, notification planning, maintenance windows, report generation, or workflow orchestration. A production scheduler should consume this library/CLI through an explicit adapter rather than assuming this repository itself owns durable jobs or execution.

## Explicit limitations

This repository is **not** a distributed scheduler, job queue, durable cron daemon, workflow engine, Kubernetes CronJob controller, or managed scheduling service. It does not persist jobs, execute arbitrary commands, coordinate multiple nodes, provide retries, locks, leases, HA, tenant isolation, authorization, timezone databases, or production deployment.

Evaluation is intentionally UTC-only. If local-time scheduling is required, the integrating service must convert an approved timezone-aware instant to UTC before evaluation and handle daylight-saving semantics explicitly.

See `SECURITY.md` and `CHANGELOG.md` for boundaries and productization history.
