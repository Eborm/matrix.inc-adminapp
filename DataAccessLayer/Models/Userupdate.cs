using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    // Model voor het wijzigen van het wachtwoord van een gebruiker
    public class Userupdate
    {
        // Huidig wachtwoord van de gebruiker (verplicht veld)
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Nieuw wachtwoord dat ingesteld moet worden
        public string Password_new { get; set; }
    }
}
