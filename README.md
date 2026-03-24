# VirtualPark

> Full-stack amusement park management system built with .NET 8 and Angular 20.

**Main**&ensp;
![Build - Test](https://github.com/IngSoft-DA2/310896-313578-303636/actions/workflows/build-test.yml/badge.svg?branch=main&event=push)
![Code Analysis](https://github.com/IngSoft-DA2/310896-313578-303636/actions/workflows/code-analysis.yml/badge.svg?branch=main&event=push)

**Develop**&ensp;
![Build - Test](https://github.com/IngSoft-DA2/310896-313578-303636/actions/workflows/build-test.yml/badge.svg?branch=develop&event=push)
![Code Analysis](https://github.com/IngSoft-DA2/310896-313578-303636/actions/workflows/code-analysis.yml/badge.svg?branch=develop&event=push)

---

## About

VirtualPark is a comprehensive platform for managing every aspect of an amusement park — from attractions and visitor access to maintenance scheduling, special events, and a dynamic scoring engine with a plugin architecture. It features a Clean Architecture backend, a modern Angular SPA, and automated CI/CD pipelines.

## Tech Stack

| Layer | Technologies |
|-------|-------------|
| **Backend** | .NET 8, ASP.NET Core Web API, Entity Framework Core, SQL Server (Azure SQL Edge) |
| **Frontend** | Angular 20, Angular Material, Tailwind CSS, RxJS, TypeScript 5.9 |
| **Testing** | xUnit (.NET), Karma + Jasmine (Angular) |
| **DevOps** | Docker Compose, GitHub Actions (self-hosted runners) |

## Features

- **Attraction Management** — Create, update, and monitor park attractions with capacity and age restrictions
- **Visitor Access Control** — NFC-based entry tracking with entry/exit timestamps and multiple entry methods
- **Dynamic Scoring Engine** — Pluggable strategy system for awarding points (by attraction type, combos, event multipliers, and more)
- **Rewards Program** — Visitors redeem accumulated points for rewards, gated by membership level
- **Membership Levels** — Tiered visitor memberships that unlock rewards and benefits
- **Ticket Management** — Multiple ticket types with QR codes and special-event associations
- **Special Events** — Time-bound events with dedicated capacity, pricing, and linked attractions
- **Maintenance & Incidences** — Track incidents on attractions and schedule preventive maintenance
- **Role-Based Authentication** — Session-managed user roles for staff and visitors
- **Reports & Analytics** — Operational reporting across attractions, visitors, and events

## Architecture

The backend follows **Clean Architecture** with clearly separated layers:

```
Domain  ←  Application  ←  Infrastructure  ←  WebAPI
                                                 ↕
                                               DTOs
```

| Layer | Responsibility |
|-------|---------------|
| `VirtualPark.Domain` | Entities, enumerations, domain interfaces (`IScoringStrategy`) |
| `VirtualPark.Application` | Business logic, service interfaces and implementations |
| `VirtualPark.Infrastructure` | EF Core DbContext, repositories, plugin loader |
| `VirtualPark.WebApi` | Controllers, filters, CORS, Swagger configuration |
| `VirtualPark.DTOs` | Request/response data transfer objects |
| `Plugins/` | External scoring strategy DLLs loaded at runtime |

The frontend (`VirtualPark-App`) uses **Angular standalone components** with Angular Material and Tailwind CSS for styling.

## Project Structure

```
VirtualPark/
├── .github/workflows/          # CI/CD pipeline definitions
├── VirtualPark/                # Backend solution
│   ├── VirtualPark.Domain/            # Entities & domain contracts
│   ├── VirtualPark.Application/       # Business logic & services
│   ├── VirtualPark.Infrastructure/    # Data access & plugin loading
│   ├── VirtualPark.WebApi/            # REST API entry point
│   ├── VirtualPark.DTOs/              # Data transfer objects
│   ├── Plugins/                       # Scoring strategy plugins
│   │   └── PuntuacionPorHora/         # Example: hourly scoring plugin
│   ├── *.Tests/                       # Unit test projects per layer
│   ├── docker-compose.yaml            # SQL Server container
│   └── VirtualPark.sln               # Solution file
├── VirtualPark-App/            # Angular frontend
│   ├── src/
│   ├── angular.json
│   ├── tailwind.config.js
│   └── package.json
└── README.md
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (LTS recommended)
- [Docker](https://www.docker.com/) (for the database container)

## Getting Started

### 1. Start the database

```bash
cd VirtualPark
docker compose up -d
```

This launches an **Azure SQL Edge** container on port `1433`.

### 2. Run the backend

```bash
cd VirtualPark/VirtualPark.WebApi
dotnet run
```

The API starts with Swagger UI available in development mode.

### 3. Run the frontend

```bash
cd VirtualPark-App
npm install
npm start
```

The app is served at `http://localhost:4200` and proxies API calls to the backend.

## Testing

**Backend** — run all .NET test projects:

```bash
cd VirtualPark
dotnet test VirtualPark.sln
```

**Frontend** — run Karma/Jasmine tests:

```bash
cd VirtualPark-App
npm test
```

## CI/CD

Two GitHub Actions workflows run on pushes and pull requests to `main` and `develop`:

| Workflow | Purpose |
|----------|---------|
| **build-test** | Restores, builds, and runs all .NET tests against the solution |
| **code-analysis** | Static code analysis for quality and standards enforcement |

Both use reusable workflows from the `IngSoft-DA2/workflows` repository on self-hosted runners.

## Plugin System

VirtualPark's scoring engine supports **runtime-loaded plugins** to define custom point-awarding strategies.

### How it works

1. A plugin is a .NET 8 class library that references `VirtualPark.Domain` and implements `IScoringStrategy`:

```csharp
public interface IScoringStrategy
{
    string Name { get; }
    int CalculatePoints(
        Attraction attraction,
        Visitor visitor,
        IEnumerable<AttractionAccess> todayAccesses,
        SpecialEvent? activeEvent
    );
}
```

2. Compile the plugin and place the DLL in the `Plugins/` directory.
3. The application discovers it automatically on startup.
4. Use the API to list and activate strategies:
   - `GET /api/scoring/strategies` — lists all available strategies (built-in + plugins)
   - `PUT /api/scoring/strategies/active` — sets the active scoring strategy

Built-in strategies include scoring by attraction type, combo visits, and event multipliers. The `PuntuacionPorHora` plugin demonstrates an hourly scoring strategy.

## Authors

| Name | Student ID |
|------|-----------|
| — | 310896 |
| — | 313578 |
| — | 303636 |
