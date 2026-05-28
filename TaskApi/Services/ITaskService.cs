using TaskApi.Dtos;

namespace TaskApi.Services;

public interface ITaskService
{
    Task<PagedResponse<TaskResponse>> GetAllAsync(
        bool? completed = null,
        string? search = null,
        int page = 1,
        int pageSize = 10
    );

    Task<TaskResponse?> GetByIdAsync(int id);
    Task<TaskResponse> CreateAsync(CreateTaskRequest request);
    Task<TaskResponse?> UpdateAsync(int id, UpdateTaskRequest request);
    Task<TaskResponse?> CompleteAsync(int id);
    Task<TaskResponse?> UncompleteAsync(int id);
    Task<bool> DeleteAsync(int id);
}
