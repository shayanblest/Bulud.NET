# Delivery rules

## Roles and handoffs

Work follows this order: Product Manager -> Developer -> Reviewer <-> Developer -> Tester <-> Developer -> user merge approval.

- The Product Manager records the outcome, non-goals, acceptance criteria, dependencies, and completion conditions in a GitHub parent issue, then decomposes it into ordered Beads tasks.
- The Developer selects an unblocked task, reads its parent issue, assigned Beads task, and this file, then branches from the current approved base branch or any prerequisite task branch named by the task. The Developer changes only the assigned scope.
- The Reviewer checks the diff against the issue and repository standards.
- The Tester runs focused validation and records evidence.

## Automatic workflow gates

- When the user assigns a product problem to the Product Manager, the Product Manager immediately records the approved outcome as a GitHub parent issue and decomposes it into small, ordered Beads tasks with clear dependencies, scope, acceptance criteria, and validation. Creating the remote issue requires the user's authorization for that product assignment.
- When the user asks the Developer to implement a ready task, the Developer selects only an unblocked task and completes the assigned scope on its task branch. The Developer must stop before any push or pull-request creation.
- Completion by the Developer automatically dispatches the Reviewer. The user does not need to request review separately.
- The Reviewer compares the task-branch diff with the assigned issue, Beads task, repository rules, and documented standards. Any finding returns the work to the Developer; the Reviewer runs again after each correction.
- The user's instruction to implement a ready task authorizes the Developer to push a Reviewer-approved diff and create its pull request. Only a Reviewer-approved diff may be pushed or used to create a pull request; the Developer must not merge it.
- After Reviewer approval, the Tester automatically runs the task's focused validation. A validation failure returns the work to the Developer, followed by another Reviewer and Tester pass. A task is handed to the user for merge approval only after the Tester reports the required evidence and no reviewer findings remain.

## Change boundaries

- Use GitHub Issues and Beads as the task system when available.
- Create a branch from the current approved base branch or the completed prerequisite branch named by the task; use the `codex/` prefix unless the task specifies another branch name.
- Each PR must link its issue, state validation performed, and avoid unrelated cleanup.
- Preserve existing provider behavior during issue #12. Package moves, namespace alignment, project references, and compatibility-required dependency upgrades are allowed; new provider behavior is not.
- Do not merge, publish packages, create remote issues outside a Product Manager assignment, or alter external services without explicit user approval. A ready-task implementation assignment authorizes only the Reviewer-approved branch push and pull-request creation described above.

## Completion

A task is ready for user merge approval only when its acceptance criteria are met, the required build/tests pass, and reviewer findings are resolved. After a user-approved merge, close the completed task and clean up only its merged feature branch.
