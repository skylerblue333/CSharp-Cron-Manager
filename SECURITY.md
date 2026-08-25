# Security Policy

Sky Schedule is an engineering-beta parsing/evaluation component, not a production execution or authorization boundary.

## Current controls

Schedule expressions are parsed as data and never executed as commands. Field values are bounded, invalid syntax fails closed, next-run search is bounded, and the runtime container executes as a non-root user. CI performs package vulnerability reporting before merge.

## Boundaries

This repository does not execute shell commands, store credentials, persist jobs, authenticate callers, authorize schedules, coordinate distributed locks, manage secrets, provide tenant isolation, or terminate TLS. Integrators must keep command execution and privileged job payloads outside this parser and enforce their own identity, authorization, persistence, retry, and deployment controls.

Report vulnerabilities privately through GitHub security reporting when available. Do not publish credentials or working exploit details in public issues.
