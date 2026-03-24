# VirtualPark

> Sistema full-stack de gestión de parques de diversiones, construido con .NET 8 y Angular 20.

<p align="center">
  <img src="https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8" />
  <img src="https://img.shields.io/badge/Angular_20-DD0031?style=for-the-badge&logo=angular&logoColor=white" alt="Angular 20" />
  <img src="https://img.shields.io/badge/TypeScript_5.9-3178C6?style=for-the-badge&logo=typescript&logoColor=white" alt="TypeScript" />
  <img src="https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" alt="SQL Server" />
  <img src="https://img.shields.io/badge/Entity_Framework_Core-512BD4?style=for-the-badge&logoColor=white" alt="Entity Framework Core" />
  <img src="https://img.shields.io/badge/Tailwind_CSS-06B6D4?style=for-the-badge&logo=tailwindcss&logoColor=white" alt="Tailwind CSS" />
  <img src="https://img.shields.io/badge/Angular_Material-757575?style=for-the-badge&logo=angular&logoColor=white" alt="Angular Material" />
  <img src="https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white" alt="Docker" />
  <img src="https://img.shields.io/badge/GitHub_Actions-2088FF?style=for-the-badge&logo=githubactions&logoColor=white" alt="GitHub Actions" />
</p>
---

## Acerca del proyecto

**VirtualPark** es una plataforma integral para gestionar todos los aspectos de un parque de diversiones: desde atracciones y control de acceso de visitantes, hasta mantenimiento, eventos especiales y un motor de puntuación dinámico con arquitectura de plugins.

Este proyecto fue desarrollado en el marco de la materia **Diseño de Aplicaciones 2 (DA2)** de Ingeniería de Software en la Universidad ORT Uruguay. El objetivo es aplicar buenas prácticas de diseño, arquitectura limpia, testing automatizado y CI/CD en un sistema real de complejidad media-alta.

El backend sigue los principios de **Clean Architecture** con capas bien delimitadas, el frontend es una SPA moderna en Angular con componentes standalone, y la infraestructura de CI/CD automatiza la validación del código en cada push y pull request.

## Stack tecnológico

| Capa | Tecnologías |
|------|-------------|
| **Backend** | .NET 8, ASP.NET Core Web API, Entity Framework Core, SQL Server (Azure SQL Edge) |
| **Frontend** | Angular 20, Angular Material, Tailwind CSS 3.4, RxJS 7.8, TypeScript 5.9 |
| **Testing** | xUnit + Moq (.NET), Karma + Jasmine (Angular) |
| **DevOps** | Docker Compose, GitHub Actions (self-hosted runners) |
| **Documentación** | Swagger / OpenAPI |

## Funcionalidades

- **Gestión de atracciones** — CRUD completo de atracciones del parque, con capacidad máxima, restricciones de edad y tipología configurable
- **Control de acceso de visitantes** — Registro de entradas y salidas mediante NFC y códigos QR, con timestamps y control de aforo en tiempo real
- **Motor de puntuación dinámico** — Sistema de estrategias intercambiables (plugin architecture) para otorgar puntos: por tipo de atracción, combos de visitas, multiplicadores de eventos y más
- **Programa de recompensas** — Los visitantes canjean puntos acumulados por recompensas, restringidas según nivel de membresía
- **Niveles de membresía** — Membresías escalonadas que desbloquean beneficios y recompensas exclusivas
- **Gestión de tickets** — Múltiples tipos de tickets con códigos QR y asociación a eventos especiales
- **Eventos especiales** — Eventos con duración definida, capacidad dedicada, precios diferenciados y atracciones vinculadas
- **Mantenimiento e incidencias** — Registro de incidentes en atracciones y programación de mantenimiento preventivo
- **Autenticación basada en roles** — Gestión de sesiones con roles diferenciados: Administrador, Operador y Visitante
- **Reportes y analíticas** — Reportes operacionales sobre atracciones, visitantes y eventos
- **Ranking diario** — Tabla de posiciones diaria de visitantes según puntos acumulados

## Arquitectura

### Backend — Clean Architecture

El backend sigue **Clean Architecture** con capas claramente separadas y dependencias que apuntan hacia el dominio:

```
Domain  ←  Application  ←  Infrastructure  ←  WebAPI
                                                 ↕
                                               DTOs
```

| Capa | Responsabilidad |
|------|----------------|
| `VirtualPark.Domain` | Entidades, enumeraciones e interfaces de dominio (`IScoringStrategy`, `RoleEnum`, `MembershipLevel`) |
| `VirtualPark.Application` | Lógica de negocio, interfaces e implementaciones de servicios, carga de plugins |
| `VirtualPark.Infrastructure` | DbContext de EF Core, repositorios, acceso a datos |
| `VirtualPark.WebApi` | Controladores REST, filtros de autorización, configuración de CORS y Swagger |
| `VirtualPark.DTOs` | Objetos de transferencia de datos (request/response) |
| `Plugins/` | DLLs externas de estrategias de puntuación cargadas en tiempo de ejecución |

### Frontend — Angular SPA

El frontend (`VirtualPark-App`) utiliza **componentes standalone de Angular 20** con:

- **Angular Material** como librería de componentes UI
- **Tailwind CSS** para estilos utilitarios y diseño responsivo
- **RxJS** para programación reactiva y manejo de estado
- **Servicios HTTP** dedicados por dominio (atracciones, scoring, auth, tickets, etc.)
- **Interceptores HTTP** para manejo de autenticación
- **Tema personalizado** vía SCSS con la paleta de Angular Material

## Endpoints principales

La API REST expone los siguientes grupos de endpoints (13 controladores):

| Recurso | Endpoints | Descripción |
|---------|-----------|-------------|
| `/api/attractions` | GET, POST, PUT, DELETE | CRUD de atracciones |
| `/api/attraction-access` | POST | Registro de entrada/salida e incidencias |
| `/api/auth` | POST | Login y registro de usuarios |
| `/api/users` | GET, POST, PUT, DELETE | Gestión de usuarios y roles |
| `/api/visitors` | GET, POST, PUT, DELETE | Gestión de visitantes y membresías |
| `/api/tickets` | GET, POST, PUT, DELETE | Gestión de tickets y QR |
| `/api/special-events` | GET, POST, PUT, DELETE | Gestión de eventos especiales |
| `/api/scoring/strategies` | GET, PUT | Listar y activar estrategias de puntuación |
| `/api/scoring/strategies/plugins` | POST | Subir plugin DLL (solo Admin) |
| `/api/scoring/ranking/daily` | GET | Ranking diario de visitantes |
| `/api/rewards` | GET, POST, PUT, DELETE | Catálogo de recompensas |
| `/api/reward-redemptions` | POST | Canje de recompensas |
| `/api/maintenance` | GET, POST, PUT | Mantenimiento e incidencias |
| `/api/reports` | GET | Reportes operacionales |

> La documentación interactiva completa está disponible en **Swagger UI** al ejecutar el backend en modo desarrollo.

## Estructura del proyecto

```
VirtualPark/
├── .github/workflows/              # Definiciones de pipelines CI/CD
├── VirtualPark/                    # Solución backend (.NET)
│   ├── VirtualPark.Domain/                # Entidades y contratos de dominio
│   ├── VirtualPark.Application/           # Lógica de negocio y servicios
│   ├── VirtualPark.Infrastructure/        # Acceso a datos y repositorios
│   ├── VirtualPark.WebApi/                # Punto de entrada de la API REST
│   ├── VirtualPark.DTOs/                  # Objetos de transferencia de datos
│   ├── Plugins/                           # Plugins de estrategias de puntuación
│   │   └── PuntuacionPorHora/             # Ejemplo: plugin de puntuación por hora
│   ├── *.Tests/                           # Proyectos de tests unitarios por capa
│   ├── docker-compose.yaml                # Contenedor de SQL Server
│   └── VirtualPark.sln                    # Archivo de solución
├── VirtualPark-App/                # Frontend Angular
│   ├── src/app/                           # Código fuente de la aplicación
│   ├── angular.json                       # Configuración de Angular CLI
│   ├── tailwind.config.js                 # Configuración de Tailwind CSS
│   └── package.json                       # Dependencias NPM
└── README.md
```

## Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (se recomienda versión LTS)
- [Docker](https://www.docker.com/) (para el contenedor de base de datos)

## Inicio rápido

### 1. Levantar la base de datos

```bash
cd VirtualPark
docker compose up -d
```

Esto lanza un contenedor de **Azure SQL Edge** en el puerto `1433`.

### 2. Ejecutar el backend

```bash
cd VirtualPark/VirtualPark.WebApi
dotnet run
```

La API inicia con **Swagger UI** disponible en modo desarrollo en `https://localhost:{puerto}/swagger`.

### 3. Ejecutar el frontend

```bash
cd VirtualPark-App
npm install
npm start
```

La aplicación se sirve en `http://localhost:4200` y redirige las llamadas API al backend mediante proxy.

## Configuración

### Connection string

La cadena de conexión a SQL Server se encuentra en `VirtualPark/VirtualPark.WebApi/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "ParkDb": "Server=localhost;Database=ParkDb;User Id=sa;Password=Passw1rd;TrustServerCertificate=True;"
  }
}
```

### CORS

La configuración de CORS en el backend permite solicitudes desde `http://localhost:4200` (el frontend Angular en desarrollo). Para producción, ajustar los orígenes permitidos en `Program.cs`.

### Swagger

La documentación interactiva de la API está habilitada automáticamente en modo desarrollo. Al ejecutar el backend, acceder a la ruta `/swagger` para explorar y probar los endpoints.

## Tests

**Backend** — ejecutar todos los proyectos de test .NET:

```bash
cd VirtualPark
dotnet test VirtualPark.sln
```

**Frontend** — ejecutar tests de Karma/Jasmine:

```bash
cd VirtualPark-App
npm test
```

## CI/CD

Dos workflows de GitHub Actions se ejecutan en cada push y pull request a las ramas `main` y `develop`:

| Workflow | Propósito |
|----------|-----------|
| **build-test** | Restaura dependencias, compila y ejecuta todos los tests .NET de la solución |
| **code-analysis** | Análisis estático de código para asegurar calidad y cumplimiento de estándares |

Ambos utilizan workflows reutilizables del repositorio `IngSoft-DA2/workflows` y corren en **self-hosted runners**. Se disparan únicamente cuando cambian archivos `.cs` o `.csproj`.

## Sistema de plugins

El motor de puntuación de VirtualPark soporta **plugins cargados en tiempo de ejecución** para definir estrategias personalizadas de asignación de puntos.

### Cómo funciona

1. Un plugin es una class library de .NET 8 que referencia `VirtualPark.Domain` e implementa la interfaz `IScoringStrategy`:

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

2. Compilar el plugin y colocar la DLL en el directorio `Plugins/`.
3. La aplicación lo descubre automáticamente al iniciar.
4. Usar la API para listar y activar estrategias:
   - `GET /api/scoring/strategies` — lista todas las estrategias disponibles (built-in + plugins)
   - `PUT /api/scoring/strategies/active` — establece la estrategia de puntuación activa
   - `POST /api/scoring/strategies/plugins` — sube una nueva DLL de plugin (solo Admin)

### Estrategias incluidas

| Estrategia | Descripción |
|------------|-------------|
| **ScoreByAttractionType** | Puntos según el tipo de atracción visitada |
| **ScoreByCombo** | Bonificación por visitar combinaciones de atracciones |
| **ScoreByEventMultiplier** | Multiplicador de puntos durante eventos especiales |
| **PuntuacionPorHora** | (Plugin) Puntuación variable según la hora del día |

### Creación de un plugin

1. Crear un proyecto de tipo **Class Library** en .NET 8
2. Agregar referencia al proyecto `VirtualPark.Domain`
3. Implementar la interfaz `IScoringStrategy`
4. Compilar en modo Release
5. Copiar la DLL resultante al directorio `Plugins/`
6. Reiniciar la aplicación o subir la DLL vía el endpoint de la API

> Los plugins con nombres de estrategia duplicados generarán un error al iniciar. Las DLLs inválidas se omiten con un warning en consola.

## Autores

| Nombre | Número de estudiante |
|--------|---------------------|
| Nicolás Píriz | 310896 |
| Santiago Suarez | 313578 |
| Manuel Stapff | 303636 |
