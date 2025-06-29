using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataAccessLayer
{
    // Factory voor het aanmaken van de database context tijdens design-time (bijv. voor migraties)
    public class MatrixIncDbContextFactory : IDesignTimeDbContextFactory<MatrixIncDbContext>
    {
        // Maakt een nieuwe instantie van de MatrixIncDbContext aan met SQLite configuratie
        public MatrixIncDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MatrixIncDbContext>();
            optionsBuilder.UseSqlite("Data Source=MatrixInc.db");
            return new MatrixIncDbContext(optionsBuilder.Options);
        }
    }
} 