# Todo App — Project Specification

## High-Level Summary

A full-stack Todo application consisting of a Blazor web front-end, an ASP.NET Core Web API middle tier, and an in-memory Entity Framework Core database. The application allows users to create, view, complete, and delete todo items. A shared class library defines the common `TodoItem` model used across all layers. The solution includes xUnit integration tests that exercise the API endpoints. The in-memory database is seeded with sample todos on startup.

## Tech Stack & Language

| Component         | Technology                                      |
|-------------------|-------------------------------------------------|
| Language          | C# / .NET 10                                    |
| Front-end         | Blazor (Server or WebAssembly)                  |
| API               | ASP.NET Core Web API (minimal or controller)    |
| ORM / Data        | Entity Framework Core with InMemory provider    |
| Shared Library    | .NET class library                              |
| Testing           | xUnit, Microsoft.AspNetCore.Mvc.Testing         |
| Build System      | dotnet CLI, single `.sln` solution file         |

## Solution Structure

```
builder/
├── TodoApp.sln
├── src/
│   ├── TodoApp.Shared/        # Shared class library (TodoItem model)
│   ├── TodoApp.Data/           # EF Core DbContext, InMemory config, seed data
│   ├── TodoApp.Api/            # ASP.NET Core Web API
│   └── TodoApp.Web/            # Blazor front-end
└── tests/
    └── TodoApp.Api.Tests/      # xUnit integration tests
```

## Features & Requirements

### TodoItem Model (TodoApp.Shared)

- Properties:
  - `Id` (int, auto-generated)
  - `Title` (string, required)
  - `IsComplete` (bool, default `false`)
- The model lives in `TodoApp.Shared` so it can be referenced by all other projects.

### Data Layer (TodoApp.Data)

- `TodoDbContext` inheriting from `DbContext` with a `DbSet<TodoItem>`.
- Configured to use `UseInMemoryDatabase("TodoDb")`.
- Seed data: at least 3 sample `TodoItem` records inserted on startup.

### API Layer (TodoApp.Api)

- RESTful endpoints:
  - `GET    /api/todos`        — List all todos
  - `GET    /api/todos/{id}`   — Get a single todo by id
  - `POST   /api/todos`        — Create a new todo (accepts JSON body with `title`)
  - `PUT    /api/todos/{id}`   — Update a todo (title and/or isComplete)
  - `DELETE /api/todos/{id}`   — Delete a todo
- Returns appropriate HTTP status codes (200, 201, 204, 404).
- Uses dependency injection for `TodoDbContext`.
- Seeds sample data on application startup.

### Web Front-End (TodoApp.Web)

- Blazor application (Server or WebAssembly) that communicates with the API.
- Pages / components:
  - **Todo List** — Displays all todos, shows completion status.
  - **Add Todo** — Form/input to add a new todo by title.
  - **Complete Todo** — Toggle a todo's completion status.
  - **Delete Todo** — Remove a todo from the list.
- Uses `HttpClient` to call the API endpoints.

### Integration Tests (TodoApp.Api.Tests)

- Uses `WebApplicationFactory<Program>` to spin up the API in-process.
- Test cases:
  - `GET /api/todos` returns seeded todos.
  - `POST /api/todos` creates a new todo and returns 201.
  - `GET /api/todos/{id}` returns a specific todo.
  - `PUT /api/todos/{id}` updates a todo.
  - `DELETE /api/todos/{id}` removes a todo and returns 204.
  - `GET /api/todos/{id}` returns 404 for a non-existent todo.

## Constraints & Guidelines

1. **Single solution file** — All projects belong to one `TodoApp.sln`.
2. **In-memory database only** — No external database dependencies; use EF Core InMemory provider.
3. **No authentication** — The app is unauthenticated for simplicity.
4. **Seed data on startup** — The API project must seed at least 3 sample todos when the application starts.
5. **.NET 10** — Use the latest .NET SDK available on the machine (currently 10.0.102).
6. **Standard project layout** — Source code under `src/`, tests under `tests/`.
7. **Minimal external dependencies** — Only use NuGet packages required for the stated tech stack.

## Acceptance Criteria

The project is considered **done** when all of the following are true:

- [ ] `dotnet build` succeeds with zero errors and zero warnings (warnings-as-errors not required but no build-breaking warnings).
- [ ] `dotnet test` passes all integration tests.
- [ ] The API project starts and responds to HTTP requests at the documented endpoints.
- [ ] `GET /api/todos` returns at least 3 seeded todos on a fresh start.
- [ ] All CRUD operations (create, read, update, delete) work correctly through the API.
- [ ] The Blazor front-end renders a todo list and supports add, complete, and delete operations via the API.
- [ ] The shared `TodoItem` model is used consistently across all projects.
- [ ] The EF Core InMemory database is used (no SQLite, SQL Server, or other providers).
- [ ] Integration tests use `WebApplicationFactory` and cover all API endpoints.
- [ ] All projects are included in a single `TodoApp.sln` solution file.
- [ ] The solution follows the directory structure defined above (`src/` and `tests/`).
