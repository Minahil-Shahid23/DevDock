using DevDock.Application.DTOs;
using MediatR;

namespace DevDock.Application.Features.Auth.Commands;

public record LoginCommand(
    string Email,
    string Password) : IRequest<AuthResponseDto>;