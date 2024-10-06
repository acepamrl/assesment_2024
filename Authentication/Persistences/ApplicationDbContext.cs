using Authentication.Entities;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Persistences
{
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
    }
}
