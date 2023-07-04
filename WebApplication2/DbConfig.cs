using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2
{
    public class DbConfig : DbContext
    {
        public DbConfig(DbContextOptions<DbConfig> options): base(options) {
            
        }
        public DbSet<ToDoModel> ToDo { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ToDoModel>().ToTable(nameof(ToDo));
        }
    }
}
