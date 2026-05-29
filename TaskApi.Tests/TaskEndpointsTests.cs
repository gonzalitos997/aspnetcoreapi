using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TaskApi.Dtos;

namespace TaskApi.Tests;

public class TasksEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TasksEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Complete_Should_Mark_Task_As_Completed()
    {
        var response = await _client.PatchAsync("/tasks/1/complete", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var task = await response.Content.ReadFromJsonAsync<TaskResponse>();

        task.Should().NotBeNull();
        task!.Id.Should().Be(1);
        task.IsCompleted.Should().BeTrue();
        task.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Uncomplete_Should_Mark_Task_As_NotCompleted()
    {
        await _client.PatchAsync("/tasks/1/complete", content: null);

        var response = await _client.PatchAsync("/tasks/1/uncomplete", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var task = await response.Content.ReadFromJsonAsync<TaskResponse>();

        task.Should().NotBeNull();
        task!.Id.Should().Be(1);
        task.IsCompleted.Should().BeFalse();
        task.CompletedAt.Should().BeNull();
    }

    [Fact]
    public async Task Complete_Should_Return_NotFound_When_Task_Does_Not_Exist()
    {
        var response = await _client.PatchAsync("/tasks/999999/complete", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Uncomplete_Should_Return_NotFound_When_Task_Does_Not_Exist()
    {
        var response = await _client.PatchAsync("/tasks/999999/uncomplete", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
