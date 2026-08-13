# PropMan API

PropMan is a multi-company property management REST API built with ASP.NET Core. It centralizes properties, units, tenants, assignments, repairs, rent invoices, payments, notifications, and operational dashboard data for property managers.

## Features

- JWT-based authentication with company and role claims
- Company-scoped property, unit, and tenant management
- Property image uploads served from `wwwroot`
- Tenant-to-unit assignment for rental and work-and-pay arrangements
- Invoice generation, payment allocation, balances, and tenant credit tracking
- Repair logging, filtering, and completion workflows
- HTML email notifications through SMTP
- Hangfire-backed invoice generation and payment-reminder workflows
- Paginated and date-filtered tenant, payment, invoice, and repair queries
- Swagger/OpenAPI documentation in development
- Centralized API responses, audit logging, and exception logging

## Technology stack

- .NET 9 and ASP.NET Core Web API
- Entity Framework Core 9 with SQL Server
- JWT bearer authentication
- Hangfire with SQL Server storage
- MailKit/MimeKit for SMTP email
- Swashbuckle for Swagger/OpenAPI

## Project structure

```text
Controllers/   HTTP endpoints for authentication, companies, properties, and tenants
Models/        EF Core entities, DTOs, and the database context
Services/      Business logic, authentication, email, payments, and background jobs
Repository/    Data-access implementations and interfaces
logic/         Property-assignment rules
Mappings/      Entity-to-DTO mapping helpers
Middleware/    Global exception logging
Migrations/    Incremental EF Core database migrations
constants/     Status values, messages, and email templates
payload/       Shared API response and pagination types
wwwroot/       Static files and uploaded property images
```

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server reachable by the application
- An SMTP account that supports SSL on connect (the current email client uses this mode)
- The `dotnet-ef` CLI tool if you need to apply migrations

## Configuration

The application reads its settings from ASP.NET Core configuration. For local development, environment variables keep credentials out of source control:

```bash
export ConnectionStrings__DefaultConnection='Server=localhost,1433;Database=PropertyManagementDB;User Id=<user>;Password=<password>;TrustServerCertificate=True'
export AppSettings__Token='<a-long-random-signing-key>'
export EmailSettings__host='<smtp-host>'
export EmailSettings__Port='465'
export EmailSettings__UserName='<smtp-username>'
export EmailSettings__Password='<smtp-password>'
export EmailSettings__SenderEmail='<sender-address>'
export EmailSettings__SenderName='PropMan'
```

The scaffolded `DataContext` currently contains a local SQL Server connection in `OnConfiguring`. Ensure that connection matches your environment, or remove that fallback before relying exclusively on `ConnectionStrings:DefaultConnection`. Never commit production database, JWT, or SMTP credentials.

## Run locally

From the directory containing `PropMan.csproj`:

```bash
dotnet restore
dotnet build
dotnet run --launch-profile http
```

The HTTP launch profile listens on `http://localhost:5152`. In the Development environment, Swagger UI is available at the application root:

```text
http://localhost:5152/
```

The Hangfire dashboard is available at:

```text
http://localhost:5152/jobs
```

### Database setup

This repository contains incremental migrations, but no full initial-schema migration or database creation script. A compatible `PropertyManagementDB` schema and an initial company/admin user are therefore required before the complete API can be used. After restoring that schema, apply the included migrations:

```bash
dotnet tool install --global dotnet-ef # only if dotnet-ef is not installed
dotnet ef database update
```

User registration and company endpoints require authentication, so the initial account must be seeded or created directly in the database. All later protected requests use the company identity embedded in that user's JWT.

## Authentication

Sign in through `POST /api/Auth/login`. A successful response includes a JWT and the user's role. Use the token on protected endpoints:

```http
Authorization: Bearer <access-token>
```

In Swagger, select **Authorize** and enter `Bearer <access-token>`. Tokens currently expire after 50 minutes.

## API overview

| Area | Base route | Main operations |
| --- | --- | --- |
| Authentication | `/api/Auth` | Log in and register authenticated users |
| Companies | `/api/Company` | Create a company |
| Properties | `/api/Property` | Manage properties and units, upload images, assign tenants, track repairs, and retrieve dashboard summaries |
| Tenants | `/api/Tenant` | Manage tenants, record payments, query invoices/payment plans, and trigger invoice workflows |

Except for login, controller routes are protected by JWT authentication. Use the Swagger document as the source of truth for request fields, query parameters, and response schemas.

## Background jobs

Hangfire uses the same SQL Server connection as the application. Invoice generation and payment reminders are implemented as background-job operations. Email delivery depends on valid SMTP configuration, and failed application exceptions are recorded through the exception-logging middleware.

## Development notes

- CORS currently permits the local frontend origins `http://localhost:5173` and `https://localhost:5173`.
- Uploaded property images are written beneath `wwwroot/uploads/properties` and exposed as static files.
- API services and repositories are registered automatically by naming convention (`I{Name}` implemented by `{Name}`).
- The two recurring jobs currently share the `send-payment-reminders` identifier, so the later registration replaces the earlier one; give each job a unique identifier to schedule both.
- The repository does not currently include an automated test project; use `dotnet build` and Swagger-based endpoint checks when validating changes.

## Production checklist

Before deploying, move all secrets to a managed secret store, remove the scaffolded connection string from `DataContext`, restrict CORS, protect the Hangfire dashboard, require HTTPS metadata for JWT authentication, replace the test invoice-email recipient, and add a complete database provisioning strategy.
