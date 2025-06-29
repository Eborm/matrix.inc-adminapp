using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    // Model voor onderdelen in het systeem
    // Vertegenwoordigt onderdelen die bij producten horen
    public class Part
    {
        // Unieke identificatie van het onderdeel (primary key)
        public int Id { get; set; }

        // Naam van het onderdeel
        public string Name { get; set; }

        // Beschrijving van het onderdeel
        public string Description { get; set; }

        // Collectie van producten waar dit onderdeel bij hoort
        public ICollection<Product> Products { get; } = new List<Product>();
    }
}
