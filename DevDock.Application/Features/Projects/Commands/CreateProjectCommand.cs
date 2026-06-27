using DevDock.Application.DTOs;
using MediatR;

namespace DevDock.Application.Features.Projects.Commands;

public record CreateProjectCommand(string Name, string Description, Guid UserId) 
    : IRequest<ProjectDto>;