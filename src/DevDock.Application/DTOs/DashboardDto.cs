namespace DevDock.Application.DTOs;

public class ProjectDashboardDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int TotalTasks { get; set; }
    public int TodoCount { get; set; }
    public int InProgressCount { get; set; }
    public int InReviewCount { get; set; }
    public int DoneCount { get; set; }
    public int OverdueCount { get; set; }
}