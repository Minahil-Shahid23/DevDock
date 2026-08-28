using DevDock.Application.DTOs;
using DevDock.Application.Exceptions;
using DevDock.Application.Interfaces;
using DevDock.Domain.Entities;
using DevDock.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DevDock.Infrastructure.Services;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _context;

    public ProjectService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectResponseDto> CreateProjectAsync(Guid ownerId, CreateProjectDto dto)
    {
        var project = new Project
        {
            Name = dto.Name,
            Description = dto.Description,
            OwnerId = ownerId
        };

        _context.Projects.Add(project);

        // Owner ko khud project ka member bana do (Role = "Owner")
        var ownerMember = new ProjectMember
        {
            ProjectId = project.Id,
            UserId = ownerId,
            Role = "Owner"
        };
        _context.ProjectMembers.Add(ownerMember);

        await _context.SaveChangesAsync();

        return await GetProjectByIdAsync(project.Id, ownerId);
    }

    public async Task<List<ProjectResponseDto>> GetUserProjectsAsync(Guid userId)
    {
        var projects = await _context.Projects
            .Where(p => p.Members.Any(m => m.UserId == userId))
            .Include(p => p.Owner)
            .Include(p => p.Members)
            .Include(p => p.Tasks)
            .ToListAsync();

        return projects.Select(MapToDto).ToList();
    }

    public async Task<ProjectResponseDto> GetProjectByIdAsync(Guid projectId, Guid userId)
    {
        var project = await _context.Projects
            .Include(p => p.Owner)
            .Include(p => p.Members)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
            throw new AuthException("Project not found.");

        var isMember = project.Members.Any(m => m.UserId == userId);
        if (!isMember)
            throw new AuthException("You are not a member of this project.");

        return MapToDto(project);
    }

    public async Task AddMemberAsync(Guid projectId, Guid requestingUserId, AddMemberDto dto)
    {
        var project = await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
            throw new AuthException("Project not found.");

        var requester = project.Members.FirstOrDefault(m => m.UserId == requestingUserId);
        if (requester == null || requester.Role != "Owner")
            throw new AuthException("Only the project owner can add members.");

        var userToAdd = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (userToAdd == null)
            throw new AuthException("User with this email not found.");

        var alreadyMember = project.Members.Any(m => m.UserId == userToAdd.Id);
        if (alreadyMember)
            throw new AuthException("User is already a member of this project.");

        var newMember = new ProjectMember
        {
            ProjectId = projectId,
            UserId = userToAdd.Id,
            Role = "Member"
        };

        _context.ProjectMembers.Add(newMember);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveMemberAsync(Guid projectId, Guid requestingUserId, Guid memberUserId)
    {
        var project = await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
            throw new AuthException("Project not found.");

        var requester = project.Members.FirstOrDefault(m => m.UserId == requestingUserId);
        if (requester == null || requester.Role != "Owner")
            throw new AuthException("Only the project owner can remove members.");

        if (memberUserId == project.OwnerId)
            throw new AuthException("Cannot remove the project owner.");

        var memberToRemove = project.Members.FirstOrDefault(m => m.UserId == memberUserId);
        if (memberToRemove == null)
            throw new AuthException("This user is not a member of the project.");

        _context.ProjectMembers.Remove(memberToRemove);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsUserProjectMemberAsync(Guid projectId, Guid userId)
    {
        return await _context.ProjectMembers
            .AnyAsync(m => m.ProjectId == projectId && m.UserId == userId);
    }

    private static ProjectResponseDto MapToDto(Project project)
    {
        return new ProjectResponseDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            OwnerId = project.OwnerId,
            OwnerName = project.Owner.FullName,
            CreatedAt = project.CreatedAt,
            MemberCount = project.Members.Count,
            TaskCount = project.Tasks.Count
        };
    }

    public async Task<ProjectDashboardDto> GetProjectDashboardAsync(Guid projectId, Guid userId)
{
    var isMember = await IsUserProjectMemberAsync(projectId, userId);
    if (!isMember)
        throw new AuthException("You are not a member of this project.");

    var project = await _context.Projects
        .FirstOrDefaultAsync(p => p.Id == projectId);

    if (project == null)
        throw new AuthException("Project not found.");

    var tasks = await _context.Tasks
        .Where(t => t.ProjectId == projectId)
        .ToListAsync();

    return new ProjectDashboardDto
    {
        ProjectId = project.Id,
        ProjectName = project.Name,
        TotalTasks = tasks.Count,
        TodoCount = tasks.Count(t => t.Status == Domain.Entities.TaskStatus.Todo),
        InProgressCount = tasks.Count(t => t.Status == Domain.Entities.TaskStatus.InProgress),
        InReviewCount = tasks.Count(t => t.Status == Domain.Entities.TaskStatus.InReview),
        DoneCount = tasks.Count(t => t.Status == Domain.Entities.TaskStatus.Done),
        OverdueCount = tasks.Count(t => t.Deadline.HasValue
            && t.Deadline.Value < DateTime.UtcNow
            && t.Status != Domain.Entities.TaskStatus.Done)
    };
}
}