namespace DevDock.Application.DTOs;

public class CodeReviewRequestDto
{
    public string Code { get; set; } = string.Empty;
    public string Language { get; set; } = "csharp";
    public Guid? ProjectId { get; set; }
}

public class CodeReviewResponseDto
{
    public Guid Id { get; set; }
    public int BugCount { get; set; }
    public int SecurityIssueCount { get; set; }
    public int PerformanceIssueCount { get; set; }
    public List<string> Suggestions { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}