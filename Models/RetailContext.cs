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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Customizing the MenuItem table and columns
            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.ToTable("MenuItems"); // Table name

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("dish_name");
                entity.Property(e => e.Price).HasColumnName("price");
                entity.Property(e => e.DishType).HasColumnName("dish_type");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            //Customizing the Sale table and columns
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.ToTable("Sales");
                entity.Property(e => e.Id).HasColumnName("sale_id");
                // Add more property configurations as needed
            });

            // Add additional configurations here
        }
    }
}
