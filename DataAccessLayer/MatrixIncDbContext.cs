using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    // Database context voor de MatrixInc applicatie
    // Bevat DbSets voor alle entiteiten en configureert de relaties
    public class MatrixIncDbContext : DbContext
    {
        // Constructor voor dependency injection van de contextopties
        public MatrixIncDbContext(DbContextOptions<MatrixIncDbContext> options) : base(options)
        {
        }

        // DbSets voor alle entiteiten in de database
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<OrderRegel> OrderRegels { get; set; }
        public DbSet<User> Users { get; set; }

        // Configureert de relaties tussen de entiteiten
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId).IsRequired();

            modelBuilder.Entity<Part>()
                .HasMany(p => p.Products)
                .WithMany(p => p.Parts);

            modelBuilder.Entity<User>();

            modelBuilder.Entity<OrderRegel>()
                .HasOne(or => or.Order)
                .WithMany(o => o.OrderRegels)
                .HasForeignKey(or => or.OrderId);

            modelBuilder.Entity<OrderRegel>()
                .HasOne(or => or.Product)
                .WithMany(p => p.OrderRegels)
                .HasForeignKey(or => or.ProductId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
