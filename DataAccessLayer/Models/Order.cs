using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    // Model voor orders in het systeem
    // Bevat orderinformatie, klant, gebruiker en orderregels
    public class Order
    {
        // Unieke identificatie van de order (primary key)
        public int Id { get; set; }

        // Datum waarop de order is geplaatst
        public DateTime OrderDate { get; set; }

        // ID van de klant die de order heeft geplaatst (foreign key)
        [Required]
        public int CustomerId { get; set; }
        
        // Navigatie property naar de klant
        public Customer Customer { get; set; } = null!;

        // ID van de gebruiker die de order heeft aangemaakt (foreign key)
        [Required]
        public int UserId { get; set; }
        
        // Navigatie property naar de gebruiker
        public User User { get; set; } = null!;
        
        // Collectie van orderregels die bij deze order horen
        public ICollection<OrderRegel> OrderRegels { get; set; } = new List<OrderRegel>();
    }
}
