using Microsoft.EntityFrameworkCore;
using Payment.Entities;

namespace Payment.Persistences
{
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Entities.Payment> Payments { get; set; }
    }
}
