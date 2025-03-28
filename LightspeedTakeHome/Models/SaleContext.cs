using Microsoft.EntityFrameworkCore;

namespace LightspeedTakeHome.Models
{
    public class SaleContext : DbContext
    {
        public SaleContext(DbContextOptions<SaleContext> options)
        : base(options)
        {
        }

        public DbSet<Sale> Sales { get; set; } = null!;
    }
}
