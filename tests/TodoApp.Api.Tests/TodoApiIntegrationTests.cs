using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TodoApp.Shared;

namespace TodoApp.Api.Tests;

public class TodoApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TodoApiIntegrationTests(WebApplicationFactory<Program> factory)
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
        Assert.True(todos.Count >= 3, "Expected at least 3 seeded todos");
    }

    [Fact]
    public async Task PostTodo_ReturnsCreatedWithNewTodo()
    {
        var newTodo = new TodoItem { Title = "Integration test todo" };

        var response = await _client.PostAsJsonAsync("/api/todos", newTodo);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<TodoItem>();
        Assert.NotNull(created);
        Assert.Equal("Integration test todo", created.Title);
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
    public async Task DeleteTodo_ReturnsNoContent()
    {
        // Create a todo to delete
        var createResponse = await _client.PostAsJsonAsync("/api/todos", new TodoItem { Title = "To delete" });
        var created = await createResponse.Content.ReadFromJsonAsync<TodoItem>();

        var response = await _client.DeleteAsync($"/api/todos/{created!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify it's gone
        var getResponse = await _client.GetAsync($"/api/todos/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetTodoById_Returns404ForNonExistent()
    {
        var response = await _client.GetAsync("/api/todos/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
