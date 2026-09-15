# PRD: modular architecture and .NET 10 migration

## Outcome

Transform Bulud.NET from packages centered on `Bulud.Base` into independently consumable capability, abstraction, provider, and framework-integration packages targeting .NET 10.

## Users

- Application developers who install only the Bulud.NET capability they need.
- Maintainers who add a provider without changing existing providers.
- Contributors who need clear ownership and dependency direction.

## In scope

- Move source into the approved `src/` capability layout.
- Define and create package boundaries and project references.
- Separate capability contracts from provider implementations.
- Move framework-specific APIs to their framework integration packages.
- Target .NET 10 directly.
- Align public namespaces with package ownership.
- Centralize shared build and NuGet metadata.
- Document architecture and task workflow.

## Out of scope

- New provider functionality or changed provider logic.
- Compatibility packages for `Bulud.Base`.
- General dependency upgrades not required by .NET 10.
- New CI/CD systems or broad behavior-test suites.
- Package publication or release automation.

## Acceptance criteria

- Every public type currently owned by `Bulud.Base` has one target module owner.
- Provider packages reference only their capability abstraction(s), core primitives when needed, and external SDKs.
- No provider references another provider.
- Generic abstractions remain free of ASP.NET Core, EF Core, and provider SDK dependencies.
- The solution restores and builds with .NET 10.
- Documentation describes the final module map and migration rules.

## Milestones

1. Establish governance, domain vocabulary, architecture records, and task decomposition.
2. Create the target project skeleton and shared build configuration.
3. Move core primitives and abstraction contracts.
4. Move provider implementations and registration extensions without behavior changes.
5. Isolate framework integrations and retire `Bulud.Base`.
6. Validate package dependency boundaries and the .NET 10 build.
