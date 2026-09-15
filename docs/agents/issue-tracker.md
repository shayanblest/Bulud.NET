# Issue tracking

GitHub Issues are the source of truth for outcomes and acceptance criteria. Beads tasks are the source of truth for ordered, assignable implementation units when Beads is available.

## Parent issue

Use one GitHub parent issue for an architecture or product outcome. It must state scope, non-goals, acceptance criteria, dependencies, and measurable completion conditions.

## Child task

Create a child issue and corresponding Beads task for each independently reviewable implementation stage. Record affected packages, prerequisites, validation, and the parent issue link.

## Status flow

Move tasks through ready, in progress, review, validation, and complete states. Do not begin a task whose dependencies are incomplete. Close the child task only after the user has approved the merged PR.

## Current parent

Issue #12 is the parent for the modular architecture and .NET 10 migration. Create its child issues only after explicit authorization to change GitHub.
