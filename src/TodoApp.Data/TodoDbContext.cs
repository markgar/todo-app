using Microsoft.EntityFrameworkCore;
using TodoApp.Shared;

namespace TodoApp.Data;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }

    public DbSet<TodoItem> Todos => Set<TodoItem>();
}
