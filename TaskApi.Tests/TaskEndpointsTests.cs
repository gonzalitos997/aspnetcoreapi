using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace TaskApi.Tests;

public class TaskEndpointsTests : IClassFixture<TaskApiFactory>
{
    private readonly HttpClient _client;

    public TaskEndpointsTests(TaskApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTasks_ShouldReturnOk()
    {
        var response = await _client.GetAsync("/tasks");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostTasks_WithValidBody_ShouldCreateTask()
    {
        var request = new { title = "Aprender integration testing" };

        var response = await _client.PostAsJsonAsync("/tasks", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Aprender integration testing");
    }

    [Fact]
    public async Task PostTasks_WithInvalidBody_ShouldReturnBadRequest()
    {
        var request = new { title = "" };

        var response = await _client.PostAsJsonAsync("/tasks", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PutTasks_WhenMarkingAsCompleted_ShouldSetCompletedAt()
    {
        var createRequest = new { title = "Tarea para completar" };

        var createResponse = await _client.PostAsJsonAsync("/tasks", createRequest);
        createResponse.EnsureSuccessStatusCode();

        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskResponseDto>();

        var updateRequest = new { title = "Tarea para completar", isCompleted = true };

        var updateResponse = await _client.PutAsJsonAsync(
            $"/tasks/{createdTask!.Id}",
            updateRequest
        );

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedTask = await updateResponse.Content.ReadFromJsonAsync<TaskResponseDto>();

        updatedTask.Should().NotBeNull();
        updatedTask!.IsCompleted.Should().BeTrue();
        updatedTask.CompletedAt.Should().NotBeNull();
    }
}
