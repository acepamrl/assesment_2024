using Microsoft.EntityFrameworkCore;

namespace Notification.Persistences
{
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Entities.Notification> Notifications { get; set; }
    }
}
