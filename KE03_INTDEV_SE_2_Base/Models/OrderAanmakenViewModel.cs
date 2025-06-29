using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KE03_INTDEV_SE_2_Base.Models
{
    // Model voor het koppelen van een product aan een aantal in een order
    public class ProductAantal
    {
        // ID van het product
        public int ProductId { get; set; }
        // Naam van het product
        public string ProductNaam { get; set; } = string.Empty;
        // Aantal van dit product dat besteld wordt
        public int Aantal { get; set; }
    }

    // ViewModel voor het aanmaken van een nieuwe order
    public class OrderAanmakenViewModel
    {
        // ID van de klant die de order plaatst (verplicht)
        [Required]
        public int CustomerId { get; set; }
        // ID van de gebruiker die de order aanmaakt (verplicht)
        [Required]
        public int UserId { get; set; }
        // Datum waarop de order wordt geplaatst (verplicht)
        [Required]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }
        // Lijst van producten met hun aantallen die besteld worden
        public List<ProductAantal> Producten { get; set; } = new List<ProductAantal>();
    }
} 