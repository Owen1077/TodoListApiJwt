using Microsoft.EntityFrameworkCore;
using TodoListAPI.Models;

namespace TodoListAPI.Data
{
    public class TodoAPIDbContext : DbContext
    {
        public TodoAPIDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Todo> Todos { get; set; }
        public DbSet<User> RegistrationTodos { get; set; }
    }
}
