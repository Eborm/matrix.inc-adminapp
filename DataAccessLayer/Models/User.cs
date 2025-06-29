using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    // Model voor gebruikers in het systeem
    // Bevat gebruikersinformatie en permissies voor autorisatie
    public class User
    {
        // Unieke identificatie van de gebruiker (primary key)
        [Key]
        [Required]
        public int Id { get; set; }

        // Gebruikersnaam voor inloggen (verplicht veld)
        [Required]
        public string UserName { get; set; }

        // Wachtwoord voor authenticatie (verplicht veld, wordt versleuteld opgeslagen)
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Permissie niveau van de gebruiker (0 = admin, 1 = manager, 2 = gebruiker)
        public int Permissions { get; set; }

        // Collectie van orders die door deze gebruiker zijn aangemaakt
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
