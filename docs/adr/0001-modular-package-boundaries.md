# ADR 0001: adopt capability-oriented package boundaries

## Status

Accepted

## Context

`Bulud.Base` currently combines unrelated capabilities and framework integrations. This forces consumers and providers to inherit dependencies they do not need.

## Decision

Bulud.NET will use a small dependency-free core, capability-specific abstraction packages, independently published provider packages, and isolated framework integration packages. Providers will never reference other providers. Sources will live under `src/` by capability.

## Consequences

The migration is a breaking release with new package references and aligned namespaces. Package ownership and dependency direction become explicit, while new providers can be added without modifying existing provider packages.
