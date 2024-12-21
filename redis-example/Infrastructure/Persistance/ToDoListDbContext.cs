using redis_example.Entities;
using Microsoft.EntityFrameworkCore;

namespace redis_example.Infrastructure.Persistance;

public class ToDoListDbContext : DbContext
{
    public ToDoListDbContext(DbContextOptions<ToDoListDbContext> options) : base(options) { }

    public DbSet<ToDo> ToDos { get; set; }

    protected override void OnModelCreating(ModelBuilder builder) {
        builder.Entity<ToDo>()
            .HasKey(t => t.Id);
    }
}