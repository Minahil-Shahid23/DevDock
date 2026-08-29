using DevDock.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevDock.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Project> Projects => Set<Project>();
public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
public DbSet<CodeReview> CodeReviews => Set<CodeReview>();
public DbSet<TaskItem> Tasks => Set<TaskItem>();
     protected override void OnModelCreating(ModelBuilder modelBuilder)

    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.Property(u => u.Role).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasOne(rt => rt.User)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(rt => rt.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(rt => rt.Token).IsRequired();
        }
        );
        modelBuilder.Entity<Project>(entity =>
{
    entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
    entity.HasOne(p => p.Owner)
          .WithMany()
          .HasForeignKey(p => p.OwnerId)
          .OnDelete(DeleteBehavior.Restrict); // owner delete ho to project delete na ho
});

modelBuilder.Entity<ProjectMember>(entity =>
{
    entity.HasIndex(pm => new { pm.ProjectId, pm.UserId }).IsUnique(); // same user dubara member na ban sake
    
    entity.HasOne(pm => pm.Project)
          .WithMany(p => p.Members)
          .HasForeignKey(pm => pm.ProjectId)
          .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(pm => pm.User)
          .WithMany()
          .HasForeignKey(pm => pm.UserId)
          .OnDelete(DeleteBehavior.Restrict);
});
modelBuilder.Entity<TaskItem>(entity =>
{
    entity.Property(t => t.Title).IsRequired().HasMaxLength(300);
    entity.Property(t => t.Status).HasConversion<string>();     // enum ko string ki tarah save karo (readable rehta hai DB mein)
    entity.Property(t => t.Priority).HasConversion<string>();

    entity.HasOne(t => t.Project)
          .WithMany(p => p.Tasks)
          .HasForeignKey(t => t.ProjectId)
          .OnDelete(DeleteBehavior.Cascade);       // project delete ho to uske tasks bhi delete ho jayein

    entity.HasOne(t => t.AssignedTo)
          .WithMany()
          .HasForeignKey(t => t.AssignedToId)
          .OnDelete(DeleteBehavior.SetNull);       // agar assigned user delete ho, task unassigned ho jaye (delete nahi)

    entity.HasOne(t => t.CreatedBy)
          .WithMany()
          .HasForeignKey(t => t.CreatedById)
          .OnDelete(DeleteBehavior.Restrict);
});
modelBuilder.Entity<CodeReview>(entity =>
{
    entity.Property(cr => cr.Code).IsRequired();
    entity.Property(cr => cr.Suggestions).IsRequired();

    entity.HasOne(cr => cr.User)
          .WithMany()
          .HasForeignKey(cr => cr.UserId)
          .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(cr => cr.Project)
          .WithMany()
          .HasForeignKey(cr => cr.ProjectId)
          .OnDelete(DeleteBehavior.SetNull);
});

        base.OnModelCreating(modelBuilder);
    }
}