using Microsoft.EntityFrameworkCore;

namespace Todo.API.Data;

public class TodoDbContext : DbContext
{
    public DbSet<TodoItem> Todos => Set<TodoItem>();

    public TodoDbContext(DbContextOptions<TodoDbContext> options)
        : base(options)
    {
    }
}