using Microsoft.EntityFrameworkCore;
using Ticket.Entities;

namespace Ticket.Persistences
{
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Entities.Ticket> Tickets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Ticket.Entities.Event> Events { get; set; }
    }
}
