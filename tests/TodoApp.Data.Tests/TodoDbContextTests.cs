using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Shared;

namespace TodoApp.Data.Tests;

public class TodoDbContextTests
{
    private TodoDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new TodoDbContext(options);
    }

    [Fact]
    public async Task CanAddAndRetrieveTodoItem()
    {
        using var context = CreateContext();
        var item = new TodoItem { Title = "Test Todo" };

        context.Todos.Add(item);
        await context.SaveChangesAsync();

        var retrieved = await context.Todos.FirstOrDefaultAsync(t => t.Title == "Test Todo");
        Assert.NotNull(retrieved);
        Assert.Equal("Test Todo", retrieved.Title);
        Assert.True(retrieved.Id > 0);
    }

    [Fact]
    public async Task TodoItem_IsComplete_DefaultsToFalse()
    {
        using var context = CreateContext();
        var item = new TodoItem { Title = "Incomplete Todo" };

        context.Todos.Add(item);
        await context.SaveChangesAsync();

        var retrieved = await context.Todos.FirstAsync();
        Assert.False(retrieved.IsComplete);
    }

    [Fact]
    public async Task CanDeleteTodoItem()
    {
        using var context = CreateContext();
        var item = new TodoItem { Title = "Delete Me" };
        context.Todos.Add(item);
        await context.SaveChangesAsync();

        context.Todos.Remove(item);
        await context.SaveChangesAsync();

        var count = await context.Todos.CountAsync();
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task CanUpdateTodoItem()
    {
        using var context = CreateContext();
        var item = new TodoItem { Title = "Original" };
        context.Todos.Add(item);
        await context.SaveChangesAsync();

        item.Title = "Updated";
        item.IsComplete = true;
        await context.SaveChangesAsync();

        var retrieved = await context.Todos.FirstAsync();
        Assert.Equal("Updated", retrieved.Title);
        Assert.True(retrieved.IsComplete);
    }

    [Fact]
    public async Task CanStoreMultipleTodoItems()
    {
        using var context = CreateContext();
        context.Todos.AddRange(
            new TodoItem { Title = "First" },
            new TodoItem { Title = "Second" },
            new TodoItem { Title = "Third" }
        );
        await context.SaveChangesAsync();

        var todos = await context.Todos.ToListAsync();
        Assert.Equal(3, todos.Count);
    }
}
