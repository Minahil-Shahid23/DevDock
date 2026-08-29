using DevDock.Application.DTOs;

namespace DevDock.Application.Interfaces;

public interface ICodeReviewService
{
    Task<CodeReviewResponseDto> ReviewCodeAsync(Guid userId, CodeReviewRequestDto dto);
}