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
        public DbSet<EventForm> EventForms { get; set; } = null!;
        public DbSet<LineItem> LineItems { get; set; } = null!;
        public DbSet<ContactForm> ContactForms { get; set; } = null!;

        // Creating the DB models so that API can map to it correctly
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Customizing the MenuItem table and columns
            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.ToTable("MenuItems");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("dish_name");
                entity.Property(e => e.Price).HasColumnName("price");
                entity.Property(e => e.DishType).HasColumnName("dish_type");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            // Customizing the Sale table and columns
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.ToTable("Sales");
                entity.Property(e => e.Id).HasColumnName("sale_id");
                entity.Property(e => e.Discount).HasColumnName("discount");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                // Ignore computed property
                entity.Ignore(e => e.Total);

                // Configure the relationship with LineItems
                entity.HasMany(e => e.LineItems)
                    .WithOne()
                    .HasForeignKey(li => li.SaleId);
            });

            // Customizing the LineItem table and columns
            modelBuilder.Entity<LineItem>(entity =>
            {
                entity.ToTable("LineItems");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.SaleId).HasColumnName("sale_id");
                entity.Property(e => e.MenuItemForSale).HasColumnName("menu_item_id");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.PricePerItem).HasColumnName("price_per_item");
                entity.Property(e => e.Discount).HasColumnName("discount");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                // Ignore computed properties - these are calculated, not stored
                entity.Ignore(e => e.SubTotal);
                entity.Ignore(e => e.TotalCost);
            });

            // Customizing the EventForm table and columns
            modelBuilder.Entity<EventForm>(entity =>
            {
                entity.ToTable("EventForms");
                entity.Property(e => e.Id).HasColumnName("form_id");
                entity.Property(e => e.ClientId).HasColumnName("client_id");
                entity.Property(e => e.EventType).HasColumnName("event_type").HasConversion<string>();
                entity.Property(e => e.DietaryRestrictions).HasColumnName("dietary_restrictions");
                entity.Property(e => e.EventDate).HasColumnName("event_date");
                entity.Property(e => e.FirstName).HasColumnName("first_name");
                entity.Property(e => e.LastName).HasColumnName("last_name");
                entity.Property(e => e.ClientEmail).HasColumnName("client_email");
                entity.Property(e => e.ClientPhoneNumber).HasColumnName("client_phone_number");
                entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>();
                entity.Property(e => e.Location).HasColumnName("location");
                entity.Property(e => e.BudgetPerPerson).HasColumnName("budget_per_person");
                entity.Property(e => e.NumberOfGuests).HasColumnName("number_of_guests");
                entity.Property(e => e.ExtraNotes).HasColumnName("extra_notes");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            // Customizing the ContactForm table and columns
            modelBuilder.Entity<ContactForm>(entity =>
            {
                entity.ToTable("ContactForms");
                entity.Property(e => e.Id).HasColumnName("form_id");
                entity.Property(e => e.ClientId).HasColumnName("client_id");
                entity.Property(e => e.FirstName).HasColumnName("first_name");
                entity.Property(e => e.LastName).HasColumnName("last_name");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
                entity.Property(e => e.Subject).HasColumnName("subject");
                entity.Property(e => e.Message).HasColumnName("message");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });
        }
    }
}