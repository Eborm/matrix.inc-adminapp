using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    // Repository voor het beheren van producten in de database
    public class ProductRepository : IProductRepository
    {
        // Database context voor toegang tot producten
        private readonly MatrixIncDbContext _context;

        // Constructor voor dependency injection van de context
        public ProductRepository(MatrixIncDbContext context)
        {
            _context = context;
        }

        // Voegt een nieuw product toe aan de database
        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        // Verwijdert een product uit de database
        public void DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
        }

        // Haalt alle producten op inclusief onderdelen
        public IEnumerable<Product> GetAllProducts()
        {
            return _context.Products.Include(p => p.Parts);
        }

        // Haalt een product op basis van het unieke ID inclusief onderdelen
        public Product? GetProductById(int id)
        {
            return _context.Products.Include(p => p.Parts).FirstOrDefault(p => p.Id == id);
        }

        // Werkt een bestaand product bij
        public void UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        // Stelt een korting in voor een product met optionele start en einddatum
        public void SetDiscount(int ProductId, decimal discount, DateTime? startDate, DateTime? endDate)
        {
            var product = _context.Products.Find(ProductId);
            if (product != null)
            {
                product.Discount = discount;
                product.DiscountStartDate = startDate;
                product.DiscountEndDate = endDate;
                _context.SaveChanges();
            }
        }
    }
}
