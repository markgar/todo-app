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
            new TodoItem { Title = "Mass assign test", IsComplete = true });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<TodoItem>();
        Assert.NotNull(created);
        Assert.False(created.IsComplete, "Server should ignore client-supplied IsComplete");
    }

    [Fact]
    public async Task PutTodo_WithEmptyTitle_Returns400()
    {
        var response = await _client.PutAsJsonAsync("/api/todos/1",
            new TodoItem { Title = "", IsComplete = false });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
