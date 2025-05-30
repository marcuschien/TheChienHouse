using Microsoft.EntityFrameworkCore;

namespace TheChienHouse.Models
{
    public class RetailContext : DbContext
    {
        public RetailContext(DbContextOptions<RetailContext> options)
            : base(options)
        {
        }

        public DbSet<MenuItem> MenuItems { get; set; } = null!;
        public DbSet<Sale> Sales { get; set; } = null!;
    }
}
