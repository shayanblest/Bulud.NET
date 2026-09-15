# Bulud.NET glossary

## Capability

A cohesive application concern offered through one or more Bulud.NET packages, such as file storage, communication, exporting, security, authentication, persistence, or web integration.

## Abstraction package

A package that defines the public contracts and shared models for one capability. It has no provider SDK, provider implementation, or dependency-injection container requirement.

## Provider package

A package that implements exactly one capability abstraction using a concrete technology or service. A provider depends on its abstraction package and required external SDKs, never on another provider.

## Framework integration package

A package whose public purpose is integration with a specific framework, such as ASP.NET Core or Entity Framework Core. It is not a generic capability abstraction.

## Core primitive

A dependency-free type that is broadly reusable across capabilities. A core primitive must not encode provider, framework, transport, or persistence behavior.

## Consumer

An application or library that references one or more Bulud.NET packages.
