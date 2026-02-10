# Code Review Findings

- [ ] **src/TodoApp.Api/Program.cs (POST /api/todos)**: Mass assignment vulnerability — the endpoint binds the full `TodoItem` model from the request body, allowing clients to set `Id` and `IsComplete` directly. The spec says POST accepts only `title`. Fix: accept a DTO or anonymous type with only `Title`, and let the server assign `Id` and default `IsComplete` to `false`.
- [ ] **src/TodoApp.Api/Program.cs (PUT /api/todos/{id})**: No validation on `input.Title` — a client can send a null or empty `Title`, violating the `required` constraint on the model and leaving the todo in an inconsistent state. Fix: return 400 (Bad Request) if `Title` is null or empty before applying the update.
