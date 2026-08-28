using DevDock.Application.DTOs;
using DevDock.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevDock.API.Controllers;

[Route("api")]
[Authorize]
public class TasksController : BaseApiController
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpPost("projects/{projectId}/tasks")]
    public async Task<ActionResult<TaskResponseDto>> Create(Guid projectId, CreateTaskDto dto)
    {
        dto.ProjectId = projectId; // ensure consistency
        var result = await _taskService.CreateTaskAsync(projectId, CurrentUserId, dto);
        return CreatedAtAction(nameof(GetById), new { taskId = result.Id }, result);
    }

    [HttpGet("projects/{projectId}/tasks")]
    public async Task<ActionResult<List<TaskResponseDto>>> GetProjectTasks(Guid projectId)
    {
        var result = await _taskService.GetProjectTasksAsync(projectId, CurrentUserId);
        return Ok(result);
    }

    [HttpGet("tasks/{taskId}")]
    public async Task<ActionResult<TaskResponseDto>> GetById(Guid taskId)
    {
        var result = await _taskService.GetTaskByIdAsync(taskId, CurrentUserId);
        return Ok(result);
    }

    [HttpPut("tasks/{taskId}")]
    public async Task<ActionResult<TaskResponseDto>> Update(Guid taskId, UpdateTaskDto dto)
    {
        var result = await _taskService.UpdateTaskAsync(taskId, CurrentUserId, dto);
        return Ok(result);
    }

    [HttpDelete("tasks/{taskId}")]
    public async Task<IActionResult> Delete(Guid taskId)
    {
        await _taskService.DeleteTaskAsync(taskId, CurrentUserId);
        return NoContent();
    }
}