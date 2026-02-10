using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TodoApp.Shared;

namespace TodoApp.Api.Tests;

public class TodoApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TodoApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTodos_ReturnsSeededTodos()
    {
        var response = await _client.GetAsync("/api/todos");
        response.EnsureSuccessStatusCode();

        var todos = await response.Content.ReadFromJsonAsync<List<TodoItem>>();
        Assert.NotNull(todos);
        Assert.True(todos.Count >= 3);
    }

    [Fact]
    public async Task PostTodo_CreatesNewTodo_Returns201()
    {
        var newTodo = new TodoItem { Title = "Test todo" };
        var response = await _client.PostAsJsonAsync("/api/todos", newTodo);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<TodoItem>();
        Assert.NotNull(created);
        Assert.Equal("Test todo", created.Title);
        Assert.False(created.IsComplete);
    }

    [Fact]
    public async Task GetTodoById_ReturnsCorrectTodo()
    {
        var response = await _client.GetAsync("/api/todos/1");
        response.EnsureSuccessStatusCode();

        var todo = await response.Content.ReadFromJsonAsync<TodoItem>();
        Assert.NotNull(todo);
        Assert.Equal(1, todo.Id);
    }

    [Fact]
    public async Task PutTodo_UpdatesTodo_Returns200()
    {
        var updated = new TodoItem { Title = "Updated title", IsComplete = true };
        var response = await _client.PutAsJsonAsync("/api/todos/1", updated);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var todo = await response.Content.ReadFromJsonAsync<TodoItem>();
        Assert.NotNull(todo);
        Assert.Equal("Updated title", todo.Title);
        Assert.True(todo.IsComplete);
    }

    [Fact]
    public async Task DeleteTodo_RemovesTodo_Returns204()
    {
        // Create a todo to delete
        var newTodo = new TodoItem { Title = "To be deleted" };
        var createResponse = await _client.PostAsJsonAsync("/api/todos", newTodo);
        var created = await createResponse.Content.ReadFromJsonAsync<TodoItem>();

        var response = await _client.DeleteAsync($"/api/todos/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetTodoById_Returns404ForNonExistentId()
    {
        var response = await _client.GetAsync("/api/todos/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
