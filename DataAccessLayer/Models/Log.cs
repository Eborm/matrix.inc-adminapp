using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    // Model voor logregels in het systeem
    // Houdt acties van gebruikers bij voor auditing en monitoring
    public class Log
    {
        // Unieke identificatie van de logregel (primary key)
        public int Id { get; set; }

        // Omschrijving van de uitgevoerde actie
        public string Action { get; set; }
        
        // Gebruikersnaam van de gebruiker die de actie heeft uitgevoerd
        public string User { get; set; }

        // Tijdstip waarop de actie is uitgevoerd
        public DateTime Time { get; set; }

        // Stad waar de actie is uitgevoerd (indien bekend)
        public string City { get; set; }
    }
}
