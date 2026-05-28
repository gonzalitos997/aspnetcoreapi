using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Dtos;
using TaskApi.Models;

namespace TaskApi.Services;

public class TaskService(AppDbContext dbContext) : ITaskService
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<PagedResponse<TaskResponse>> GetAllAsync(
        bool? completed = null,
        string? search = null,
        int page = 1,
        int pageSize = 10
    )
    {
        var query = _dbContext.Tasks.AsQueryable();

        if (completed.HasValue)
        {
            query = query.Where(task => task.IsCompleted == completed.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.Trim();
            query = query.Where(task => task.Title.Contains(searchTerm));
        }

        var totalItems = await query.CountAsync();

        var items = await query
            .OrderByDescending(task => task.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(task => MapToResponse(task))
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        return new PagedResponse<TaskResponse>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
        };
    }

    public async Task<TaskResponse?> GetByIdAsync(int id)
    {
        var task = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);

        if (task is null)
        {
            return null;
        }

        return MapToResponse(task);
    }

    public async Task<TaskResponse> CreateAsync(CreateTaskRequest request)
    {
        var newTask = new TaskItem
        {
            Title = request.Title,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            CompletedAt = null,
        };

        _dbContext.Tasks.Add(newTask);
        await _dbContext.SaveChangesAsync();

        return MapToResponse(newTask);
    }

    public async Task<TaskResponse?> UpdateAsync(int id, UpdateTaskRequest request)
    {
        var task = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);

        if (task is null)
        {
            return null;
        }

        var wasCompleted = task.IsCompleted;

        task.Title = request.Title;
        task.IsCompleted = request.IsCompleted;

        if (!wasCompleted && request.IsCompleted)
        {
            task.CompletedAt = DateTime.UtcNow;
        }
        else if (wasCompleted && !request.IsCompleted)
        {
            task.CompletedAt = null;
        }

        await _dbContext.SaveChangesAsync();

        return MapToResponse(task);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);

        if (task is null)
        {
            return false;
        }

        _dbContext.Tasks.Remove(task);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    private static TaskResponse MapToResponse(TaskItem task)
    {
        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            IsCompleted = task.IsCompleted,
            CreatedAt = task.CreatedAt,
            CompletedAt = task.CompletedAt,
        };
    }
}
