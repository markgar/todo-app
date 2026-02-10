using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseInMemoryDatabase("TodoDb"));

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    if (!db.Todos.Any())
    {
        db.Todos.AddRange(
            new TodoItem { Title = "Buy groceries", IsComplete = false },
            new TodoItem { Title = "Walk the dog", IsComplete = true },
            new TodoItem { Title = "Do laundry", IsComplete = false }
        );
        db.SaveChanges();
    }
}

app.MapGet("/api/todos", async (TodoDbContext db) =>
    Results.Ok(await db.Todos.ToListAsync()));

app.MapGet("/api/todos/{id}", async (int id, TodoDbContext db) =>
    await db.Todos.FindAsync(id) is TodoItem todo
        ? Results.Ok(todo)
        : Results.NotFound());

app.MapPost("/api/todos", async (TodoItem input, TodoDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(input.Title))
        return Results.BadRequest("Title is required and cannot be empty.");

    var todo = new TodoItem { Title = input.Title, IsComplete = false };
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/api/todos/{todo.Id}", todo);
});

app.MapPut("/api/todos/{id}", async (int id, TodoItem input, TodoDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(input.Title))
        return Results.BadRequest("Title is required and cannot be empty.");

    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    todo.Title = input.Title;
    todo.IsComplete = input.IsComplete;
    await db.SaveChangesAsync();
    return Results.Ok(todo);
});

app.MapDelete("/api/todos/{id}", async (int id, TodoDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

public partial class Program { }
