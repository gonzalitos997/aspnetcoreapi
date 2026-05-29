using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaskApi.Dtos;

namespace TaskApi.Tests;

public class TasksCrudEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TasksCrudEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_Should_Return_Ok()
    {
        var response = await _client.GetAsync("/tasks");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResponse<TaskResponse>>();

        result.Should().NotBeNull();
        result!.Items.Should().NotBeNull();
        result.Items.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetById_Should_Return_Ok_When_Task_Exists()
    {
        var createRequest = new CreateTaskRequest { Title = "Tarea para obtener por id" };

        var createResponse = await _client.PostAsJsonAsync("/tasks", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskResponse>();
        createdTask.Should().NotBeNull();

        var response = await _client.GetAsync($"/tasks/{createdTask!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var task = await response.Content.ReadFromJsonAsync<TaskResponse>();

        task.Should().NotBeNull();
        task!.Id.Should().Be(createdTask.Id);
        task.Title.Should().Be("Tarea para obtener por id");
    }

    [Fact]
    public async Task GetById_Should_Return_NotFound_When_Task_Does_Not_Exist()
    {
        var response = await _client.GetAsync("/tasks/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_Should_Create_Task()
    {
        var request = new CreateTaskRequest { Title = "Nueva tarea desde test" };

        var response = await _client.PostAsJsonAsync("/tasks", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTask = await response.Content.ReadFromJsonAsync<TaskResponse>();

        createdTask.Should().NotBeNull();
        createdTask!.Id.Should().BeGreaterThan(0);
        createdTask.Title.Should().Be("Nueva tarea desde test");
        createdTask.IsCompleted.Should().BeFalse();
        createdTask.CompletedAt.Should().BeNull();
    }

    [Fact]
    public async Task Update_Should_Update_Task_When_Task_Exists()
    {
        var createRequest = new CreateTaskRequest { Title = "Tarea original" };

        var createResponse = await _client.PostAsJsonAsync("/tasks", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskResponse>();
        createdTask.Should().NotBeNull();

        var updateRequest = new UpdateTaskRequest
        {
            Title = "Tarea actualizada desde test",
            IsCompleted = true,
        };

        var response = await _client.PutAsJsonAsync($"/tasks/{createdTask!.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedTask = await response.Content.ReadFromJsonAsync<TaskResponse>();

        updatedTask.Should().NotBeNull();
        updatedTask!.Id.Should().Be(createdTask.Id);
        updatedTask.Title.Should().Be("Tarea actualizada desde test");
        updatedTask.IsCompleted.Should().BeTrue();
        updatedTask.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Update_Should_Return_NotFound_When_Task_Does_Not_Exist()
    {
        var request = new UpdateTaskRequest { Title = "No existe", IsCompleted = false };

        var response = await _client.PutAsJsonAsync("/tasks/999999", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Should_Return_NoContent_When_Task_Exists()
    {
        var createRequest = new CreateTaskRequest { Title = "Tarea para eliminar" };

        var createResponse = await _client.PostAsJsonAsync("/tasks", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskResponse>();
        createdTask.Should().NotBeNull();

        var response = await _client.DeleteAsync($"/tasks/{createdTask!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/tasks/{createdTask.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Should_Return_NotFound_When_Task_Does_Not_Exist()
    {
        var response = await _client.DeleteAsync("/tasks/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
