using DevDock.Application.DTOs;

namespace DevDock.Application.Interfaces;

public interface IProjectService
{
    Task<ProjectResponseDto> CreateProjectAsync(Guid ownerId, CreateProjectDto dto);
    Task<List<ProjectResponseDto>> GetUserProjectsAsync(Guid userId);
    Task<ProjectResponseDto> GetProjectByIdAsync(Guid projectId, Guid userId);
    Task AddMemberAsync(Guid projectId, Guid requestingUserId, AddMemberDto dto);
    Task RemoveMemberAsync(Guid projectId, Guid requestingUserId, Guid memberUserId);
    Task<bool> IsUserProjectMemberAsync(Guid projectId, Guid userId);
 Task<ProjectDashboardDto> GetProjectDashboardAsync(Guid projectId, Guid userId);
}