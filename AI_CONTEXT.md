# Project Context

KeyPilot is a web application that helps home buyers manage the process of purchasing property in New Zealand.

The product tracks:
- conditional deadlines
- tasks
- documents
- contacts
- settlement timeline

## Stack

Frontend
- Next.js (TypeScript)

Backend
- .NET 8 API

Database
- Postgres

Storage
- S3 compatible object storage

Architecture style:
- Modular monolith
- Domain modules

Modules:
- Property Workspace
- Workflow / Tasks
- Documents
- Contacts
- Notifications

## Key Principles

- Keep the backend as a modular monolith
- Use REST APIs
- Use presigned URLs for document uploads
- Postgres is the source of truth
