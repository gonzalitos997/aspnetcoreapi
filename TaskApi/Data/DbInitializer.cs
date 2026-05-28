using TaskApi.Models;

namespace TaskApi.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext context)
    {
        if (context.Tasks.Any())
        {
            return;
        }

        var now = DateTime.UtcNow;

        var tasks = new List<TaskItem>
        {
            new TaskItem
            {
                Title = "Aprender ASP.NET Core",
                IsCompleted = false,
                CreatedAt = now,
                CompletedAt = null,
            },
            new TaskItem
            {
                Title = "Aprender Entity Framework Core",
                IsCompleted = false,
                CreatedAt = now,
                CompletedAt = null,
            },
            new TaskItem
            {
                Title = "Probar la API con curl",
                IsCompleted = true,
                CreatedAt = now,
                CompletedAt = now,
            },
        };

        context.Tasks.AddRange(tasks);
        context.SaveChanges();
    }
}
