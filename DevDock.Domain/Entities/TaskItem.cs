using DevDock.Domain.Common;
using DevDock.Domain.Enums;

namespace DevDock.Domain.Entities;

public class TaskItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DevDock.Domain.Enums.TaskStatus Status { get; set; } = DevDock.Domain.Enums.TaskStatus.Todo;
    public string AssignedTo { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}