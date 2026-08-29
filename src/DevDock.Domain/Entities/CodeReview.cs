namespace DevDock.Domain.Entities;

public class CodeReview
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }

    public string Code { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;

    public int BugCount { get; set; }
    public int SecurityIssueCount { get; set; }
    public int PerformanceIssueCount { get; set; }
    public string Suggestions { get; set; } = string.Empty; // JSON array as string

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}