using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Models
{
    // Model voor orderregels in het systeem
    // Vertegenwoordigt een product in een order met de hoeveelheid
    public class OrderRegel
    {
        // Unieke identificatie van de orderregel (primary key)
        [Key]
        public int Id { get; set; }

        // ID van de order waar deze regel bij hoort (foreign key)
        [Required]
        public int OrderId { get; set; }
        
        // Navigatie property naar de order
        public Order Order { get; set; } = null!;

        // ID van het product in deze regel (foreign key)
        [Required]
        public int ProductId { get; set; }
        
        // Navigatie property naar het product
        public Product Product { get; set; } = null!;

        // Aantal van het product dat besteld is (verplicht veld)
        [Required]
        public int Quantity { get; set; }
    }
} 