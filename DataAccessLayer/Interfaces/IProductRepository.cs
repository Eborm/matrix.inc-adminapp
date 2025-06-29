using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    // Interface voor productrepository
    // Definieert methoden voor het beheren van producten in de database
    public interface IProductRepository
    {
        // Haalt alle producten op
        public IEnumerable<Product> GetAllProducts();

        // Haalt een product op basis van het unieke ID
        public Product? GetProductById(int id);

        // Voegt een nieuw product toe aan de database
        public void AddProduct(Product product);

        // Werkt een bestaand product bij
        public void UpdateProduct(Product product);

        // Verwijdert een product uit de database
        public void DeleteProduct(Product product);

        // Stelt een korting in voor een product met optionele start en einddatum
        public void SetDiscount(int ProductId, decimal discount, DateTime? startDate, DateTime? endDate);
    }
}
