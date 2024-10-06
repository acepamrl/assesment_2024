using Event.Entities;
using Microsoft.EntityFrameworkCore;

namespace Event.Persistences
{
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Entities.Event> Events { get; set; }
    }
}
