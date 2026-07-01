using DevDock.Application.DTOs;
using MediatR;

namespace DevDock.Application.Features.Auth.Commands;

public record RegisterCommand(
    string FullName,
    string Email,
    string Password,
    string Role) : IRequest<AuthResponseDto>;