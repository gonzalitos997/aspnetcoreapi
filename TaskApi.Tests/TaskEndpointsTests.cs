using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace TaskApi.Tests;

public class TasksEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TasksEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Complete_Should_Mark_Task_As_Completed()
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, "/tasks/1/complete");
        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var task = await response.Content.ReadFromJsonAsync<TaskApi.Dtos.TaskResponse>();

        task.Should().NotBeNull();
        task!.Id.Should().Be(1);
        task.IsCompleted.Should().BeTrue();
        task.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Uncomplete_Should_Mark_Task_As_NotCompleted()
    {
        var completeRequest = new HttpRequestMessage(HttpMethod.Patch, "/tasks/1/complete");
        await _client.SendAsync(completeRequest);

        var uncompleteRequest = new HttpRequestMessage(HttpMethod.Patch, "/tasks/1/uncomplete");
        var response = await _client.SendAsync(uncompleteRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var task = await response.Content.ReadFromJsonAsync<TaskApi.Dtos.TaskResponse>();

        task.Should().NotBeNull();
        task!.Id.Should().Be(1);
        task.IsCompleted.Should().BeFalse();
        task.CompletedAt.Should().BeNull();
    }

    [Fact]
    public async Task Complete_Should_Return_NotFound_When_Task_Does_Not_Exist()
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, "/tasks/999999/complete");
        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Uncomplete_Should_Return_NotFound_When_Task_Does_Not_Exist()
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, "/tasks/999999/uncomplete");
        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
