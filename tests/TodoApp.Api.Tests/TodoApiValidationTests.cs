using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TodoApp.Shared;

namespace TodoApp.Api.Tests;

public class TodoApiValidationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TodoApiValidationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostTodo_WithEmptyTitle_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/todos", new TodoItem { Title = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostTodo_IgnoresClientSuppliedIdAndIsComplete()
    {
        var response = await _client.PostAsJsonAsync("/api/todos",
            new TodoItem { Id = 9999, Title = "Mass assign test", IsComplete = true });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<TodoItem>();
        Assert.NotNull(created);
        Assert.NotEqual(9999, created.Id);
        Assert.False(created.IsComplete, "Server should ignore client-supplied IsComplete");
    }

    [Fact]
    public async Task PostTodo_IgnoresClientSuppliedId()
    {
        var response = await _client.PostAsJsonAsync("/api/todos",
            new TodoItem { Id = 9999, Title = "Id override test" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<TodoItem>();
        Assert.NotNull(created);
        Assert.NotEqual(9999, created.Id);
    }

    [Fact]
    public async Task PutTodo_WithEmptyTitle_Returns400()
    {
        var response = await _client.PutAsJsonAsync("/api/todos/1",
            new TodoItem { Title = "", IsComplete = false });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PutTodo_NonExistentId_Returns404()
    {
        var response = await _client.PutAsJsonAsync("/api/todos/99999",
            new TodoItem { Title = "Does not exist", IsComplete = false });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTodo_NonExistentId_Returns404()
    {
        var response = await _client.DeleteAsync("/api/todos/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
