using DevDock.Application.DTOs;
using DevDock.Application.Interfaces;
using DevDock.Domain.Entities;
using MediatR;

namespace DevDock.Application.Features.Projects.Commands;

public class CreateProjectCommandHandler 
    : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProjectCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ProjectDto> Handle(
        CreateProjectCommand request, 
        CancellationToken cancellationToken)
    {
        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            UserId = request.UserId
        };

        await _unitOfWork.Projects.AddAsync(project);
        await _unitOfWork.SaveChangesAsync();

        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            UserId = project.UserId
        };
    }
}