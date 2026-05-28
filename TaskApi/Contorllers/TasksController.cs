using Microsoft.AspNetCore.Mvc;
using TaskApi.Dtos;
using TaskApi.Services;

namespace TaskApi.Controllers;

[ApiController]
[Route("tasks")]
public class TasksController(ITaskService taskService) : ControllerBase
{
    private readonly ITaskService _taskService = taskService;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? completed,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10
    )
    {
        if (page < 1)
        {
            return BadRequest(new { message = "page debe ser mayor o igual a 1" });
        }

        if (pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new { message = "pageSize debe estar entre 1 y 100" });
        }

        var result = await _taskService.GetAllAsync(completed, search, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var task = await _taskService.GetByIdAsync(id);

        if (task is null)
        {
            return NotFound(new { message = $"Task con id {id} no encontrada" });
        }

        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
    {
        var newTask = await _taskService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = newTask.Id }, newTask);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskRequest request)
    {
        var updatedTask = await _taskService.UpdateAsync(id, request);

        if (updatedTask is null)
        {
            return NotFound(new { message = $"Task con id {id} no encontrada" });
        }

        return Ok(updatedTask);
    }

    [HttpPatch("{id:int}/complete")]
    public async Task<IActionResult> Complete(int id)
    {
        var completedTask = await _taskService.CompleteAsync(id);

        if (completedTask is null)
        {
            return NotFound(new { message = $"Task con id {id} no encontrada" });
        }

        return Ok(completedTask);
    }

    [HttpPatch("{id:int}/uncomplete")]
    public async Task<IActionResult> Uncomplete(int id)
    {
        var task = await _taskService.UncompleteAsync(id);

        if (task is null)
        {
            return NotFound(new { message = $"Task con id {id} no encontrada" });
        }

        return Ok(task);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _taskService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new { message = $"Task con id {id} no encontrada" });
        }

        return NoContent();
    }
}
