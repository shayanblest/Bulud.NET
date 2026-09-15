# Delivery rules

## Roles and handoffs

Work follows this order: Product Manager -> Orchestrator -> Developer -> Reviewer <-> Developer -> Tester <-> Developer -> Orchestrator -> user merge approval.

- The Product Manager records the outcome, non-goals, acceptance criteria, dependencies, and completion conditions in a GitHub parent issue, then decomposes it into ordered Beads tasks.
- The Orchestrator selects an unblocked task and its live parent PR before assigning work.
- The Developer reads the issue, assigned task, this file, and the parent PR. The Developer branches from the parent PR head and changes only the assigned scope.
- The Reviewer checks the diff against the issue and repository standards.
- The Tester runs focused validation and records evidence.

## Change boundaries

- Use GitHub Issues and Beads as the task system when available.
- Treat `develop` as an integration-only branch. Never commit or push a change directly to `develop`.
- Create every change branch from the current `develop` head; use the `codex/` prefix unless the task specifies another branch name.
- Push the change branch and open a pull request targeting `develop`. Only the repository owner merges that pull request; agents must never merge it.
- Each PR must link its issue, state validation performed, and avoid unrelated cleanup.
- Preserve existing provider behavior during issue #12. Package moves, namespace alignment, project references, and compatibility-required dependency upgrades are allowed; new provider behavior is not.
- Do not merge, publish packages, push branches, create remote issues, or alter external services without explicit user approval.

## Completion

A task is ready for user merge approval only when its acceptance criteria are met, the required build/tests pass, and reviewer findings are resolved. After a user-approved merge, close the completed task and clean up only its merged feature branch.
