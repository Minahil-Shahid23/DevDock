using DevDock.Domain.Entities;

namespace DevDock.Application.Interfaces;

public interface IUnitOfWork
{
    IRepository<User> Users { get; }
    IRepository<Project> Projects { get; }
    IRepository<TaskItem> Tasks { get; }
    Task<int> SaveChangesAsync();
}