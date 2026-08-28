using DevDock.Application.DTOs;

namespace DevDock.Application.Interfaces;

public interface ITaskService
{
    Task<TaskResponseDto> CreateTaskAsync(Guid projectId, Guid requestingUserId, CreateTaskDto dto);
    Task<List<TaskResponseDto>> GetProjectTasksAsync(Guid projectId, Guid requestingUserId);
    Task<TaskResponseDto> GetTaskByIdAsync(Guid taskId, Guid requestingUserId);
    Task<TaskResponseDto> UpdateTaskAsync(Guid taskId, Guid requestingUserId, UpdateTaskDto dto);
    Task DeleteTaskAsync(Guid taskId, Guid requestingUserId);
}