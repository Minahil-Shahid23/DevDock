using DevDock.Application.Interfaces;
using DevDock.Domain.Entities;
using DevDock.Infrastructure.Persistence;

namespace DevDock.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IRepository<User> Users { get; }
    public IRepository<Project> Projects { get; }
    public IRepository<TaskItem> Tasks { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Users = new Repository<User>(context);
        Projects = new Repository<Project>(context);
        Tasks = new Repository<TaskItem>(context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}