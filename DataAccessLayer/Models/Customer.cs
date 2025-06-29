using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    // Model voor klanten in het systeem
    // Bevat klantinformatie en hun order geschiedenis
    public class Customer
    {
        // Unieke identificatie van de klant (primary key)
        [Key]
        [Required]
        public int Id { get; set; }

        // Naam van de klant (verplicht veld)
        [Required]
        public string Name { get; set; }

        // Adres van de klant (verplicht veld)
        [Required]
        public string Address { get; set; }

        // Geeft aan of de klant actief is in het systeem
        public bool Active { get; set; }

        // Collectie van orders die door deze klant zijn geplaatst
        public ICollection<Order> Orders { get; } = new List<Order>();
    }
}