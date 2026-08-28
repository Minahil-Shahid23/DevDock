using DevDock.Application.DTOs;
using DevDock.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevDock.API.Controllers;

[Route("api/projects")]
[Authorize]
public class ProjectsController : BaseApiController
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponseDto>> Create(CreateProjectDto dto)
    {
        var result = await _projectService.CreateProjectAsync(CurrentUserId, dto);
        return CreatedAtAction(nameof(GetById), new { projectId = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<List<ProjectResponseDto>>> GetMyProjects()
    {
        var result = await _projectService.GetUserProjectsAsync(CurrentUserId);
        return Ok(result);
    }

    [HttpGet("{projectId}")]
    public async Task<ActionResult<ProjectResponseDto>> GetById(Guid projectId)
    {
        var result = await _projectService.GetProjectByIdAsync(projectId, CurrentUserId);
        return Ok(result);
    }

    [HttpPost("{projectId}/members")]
    public async Task<IActionResult> AddMember(Guid projectId, AddMemberDto dto)
    {
        await _projectService.AddMemberAsync(projectId, CurrentUserId, dto);
        return NoContent();
    }

    [HttpDelete("{projectId}/members/{memberUserId}")]
    public async Task<IActionResult> RemoveMember(Guid projectId, Guid memberUserId)
    {
        await _projectService.RemoveMemberAsync(projectId, CurrentUserId, memberUserId);
        return NoContent();
    }

    [HttpGet("{projectId}/dashboard")]
public async Task<ActionResult<ProjectDashboardDto>> GetDashboard(Guid projectId)
{
    var result = await _projectService.GetProjectDashboardAsync(projectId, CurrentUserId);
    return Ok(result);
}
}