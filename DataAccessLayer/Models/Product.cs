using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    // Model voor producten in het systeem
    // Bevat productinformatie, prijzen, kortingen en onderdelen
    public class Product
    {        
        // Unieke identificatie van het product (primary key)
        public int Id { get; set; }

        // Naam van het product
        public string Name { get; set; }

        // Beschrijving van het product
        public string Description { get; set; }

        // Originele prijs van het product
        public decimal Price { get; set; }

        // Kortingspercentage (0-100)
        public decimal Discount { get; set; }

        // Startdatum van de korting (optioneel)
        public DateTime? DiscountStartDate { get; set; }
        
        // Einddatum van de korting (optioneel)
        public DateTime? DiscountEndDate { get; set; }

        // Bepaalt of de korting momenteel actief is
        // True als er een korting is, binnen de geldige periode
        public bool IsDiscountActive =>
            Discount > 0 &&
            DiscountStartDate.HasValue &&
            DiscountEndDate.HasValue &&
            DiscountStartDate.Value.Date <= DateTime.Now.Date &&
            DiscountEndDate.Value.Date >= DateTime.Now.Date;

        // Berekent de prijs met korting als deze actief is
        // Anders wordt de originele prijs teruggegeven
        public decimal DiscountedPrice =>
            IsDiscountActive ? Math.Round(Price * (1 - Discount / 100), 2) : Price;

        // Collectie van orders waarin dit product voorkomt
        public ICollection<Order> Orders { get; set; } = new List<Order>();

        // Collectie van onderdelen die bij dit product horen
        public ICollection<Part> Parts { get; } = new List<Part>();

        // Collectie van orderregels waarin dit product voorkomt
        public ICollection<OrderRegel> OrderRegels { get; set; } = new List<OrderRegel>();
    }
}
