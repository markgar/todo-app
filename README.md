# Todo App

A full-stack Todo application built with .NET 10.

## Architecture

The solution is organized into three layers plus a shared library:

1. **TodoApp.Web** — A Blazor web front-end for managing todos (add, complete, delete, list).
2. **TodoApp.Api** — An ASP.NET Core Web API middle tier exposing RESTful endpoints for todo operations.
3. **TodoApp.Data** — Entity Framework Core data layer using an in-memory database (`UseInMemoryDatabase`).
4. **TodoApp.Shared** — A shared class library containing the `TodoItem` model (id, title, isComplete).

## Features

- Create, list, complete, and delete todos
- RESTful API with standard HTTP methods
- In-memory database seeded with sample data on startup
- Blazor interactive UI that communicates with the API
- xUnit integration tests using `WebApplicationFactory`

## Tech Stack

- .NET 10
- Blazor (web front-end)
- ASP.NET Core Web API
- Entity Framework Core (InMemory provider)
- xUnit + `Microsoft.AspNetCore.Mvc.Testing`

## Getting Started

```bash
dotnet build
dotnet test
dotnet run --project src/TodoApp.Api
dotnet run --project src/TodoApp.Web
```
