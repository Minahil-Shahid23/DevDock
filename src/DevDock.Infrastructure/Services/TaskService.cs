using DevDock.Application.DTOs;
using DevDock.Application.Exceptions;
using DevDock.Application.Interfaces;
using DevDock.Domain.Entities;
using DevDock.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using TaskStatus = DevDock.Domain.Entities.TaskStatus;

namespace DevDock.Infrastructure.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _context;

    public TaskService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskResponseDto> CreateTaskAsync(Guid projectId, Guid requestingUserId, CreateTaskDto dto)
    {
        await EnsureUserIsMemberAsync(projectId, requestingUserId);

        // Agar assign kar rahe hain kisi ko, check karo wo bhi project ka member hai
        if (dto.AssignedToId.HasValue)
        {
            var isAssigneeMember = await _context.ProjectMembers
                .AnyAsync(m => m.ProjectId == projectId && m.UserId == dto.AssignedToId.Value);

            if (!isAssigneeMember)
                throw new AuthException("Cannot assign task to a user who is not a project member.");
        }

        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            ProjectId = projectId,
            AssignedToId = dto.AssignedToId,
            CreatedById = requestingUserId,
            Priority = dto.Priority,
            Deadline = dto.Deadline,
            Status = TaskStatus.Todo
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return await GetTaskByIdAsync(task.Id, requestingUserId);
    }

    public async Task<List<TaskResponseDto>> GetProjectTasksAsync(Guid projectId, Guid requestingUserId)
    {
        await EnsureUserIsMemberAsync(projectId, requestingUserId);

        var tasks = await _context.Tasks
            .Where(t => t.ProjectId == projectId)
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return tasks.Select(MapToDto).ToList();
    }

    public async Task<TaskResponseDto> GetTaskByIdAsync(Guid taskId, Guid requestingUserId)
    {
        var task = await _context.Tasks
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
            throw new AuthException("Task not found.");

        await EnsureUserIsMemberAsync(task.ProjectId, requestingUserId);

        return MapToDto(task);
    }

    public async Task<TaskResponseDto> UpdateTaskAsync(Guid taskId, Guid requestingUserId, UpdateTaskDto dto)
    {
        var task = await _context.Tasks
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
            throw new AuthException("Task not found.");

        await EnsureUserIsMemberAsync(task.ProjectId, requestingUserId);

        if (dto.AssignedToId.HasValue)
        {
            var isAssigneeMember = await _context.ProjectMembers
                .AnyAsync(m => m.ProjectId == task.ProjectId && m.UserId == dto.AssignedToId.Value);

            if (!isAssigneeMember)
                throw new AuthException("Cannot assign task to a user who is not a project member.");

            task.AssignedToId = dto.AssignedToId.Value;
        }

        if (!string.IsNullOrWhiteSpace(dto.Title))
            task.Title = dto.Title;

        if (dto.Description != null)
            task.Description = dto.Description;

        if (dto.Status.HasValue)
            task.Status = dto.Status.Value;

        if (dto.Priority.HasValue)
            task.Priority = dto.Priority.Value;

        if (dto.Deadline.HasValue)
            task.Deadline = dto.Deadline.Value;

        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // reload karo taake AssignedTo naam update reflect ho agar assignee change hua ho
        task = await _context.Tasks
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy)
            .FirstAsync(t => t.Id == taskId);

        return MapToDto(task);
    }

    public async Task DeleteTaskAsync(Guid taskId, Guid requestingUserId)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
            throw new AuthException("Task not found.");

        await EnsureUserIsMemberAsync(task.ProjectId, requestingUserId);

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
    }

    private async Task EnsureUserIsMemberAsync(Guid projectId, Guid userId)
    {
        var isMember = await _context.ProjectMembers
            .AnyAsync(m => m.ProjectId == projectId && m.UserId == userId);

        if (!isMember)
            throw new AuthException("You are not a member of this project.");
    }

    private static TaskResponseDto MapToDto(TaskItem task)
    {
        return new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            ProjectId = task.ProjectId,
            AssignedToId = task.AssignedToId,
            AssignedToName = task.AssignedTo?.FullName,
            CreatedById = task.CreatedById,
            CreatedByName = task.CreatedBy.FullName,
            Status = task.Status,
            Priority = task.Priority,
            Deadline = task.Deadline,
            CreatedAt = task.CreatedAt
        };
    }
}