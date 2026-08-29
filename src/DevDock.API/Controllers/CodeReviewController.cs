using DevDock.Application.DTOs;
using DevDock.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevDock.API.Controllers;

[Route("api/code-review")]
[Authorize]
public class CodeReviewController : BaseApiController
{
    private readonly ICodeReviewService _codeReviewService;

    public CodeReviewController(ICodeReviewService codeReviewService)
    {
        _codeReviewService = codeReviewService;
    }

    [HttpPost]
    public async Task<ActionResult<CodeReviewResponseDto>> ReviewCode(CodeReviewRequestDto dto)
    {
        var result = await _codeReviewService.ReviewCodeAsync(CurrentUserId, dto);
        return Ok(result);
    }
}