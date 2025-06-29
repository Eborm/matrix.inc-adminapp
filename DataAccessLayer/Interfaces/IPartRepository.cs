using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    // Interface voor onderdelenrepository
    // Definieert methoden voor het beheren van onderdelen in de database
    public interface IPartRepository
    {
        // Haalt alle onderdelen op
        public IEnumerable<Part> GetAllParts();

        // Haalt een onderdeel op basis van het unieke ID
        public Part? GetPartById(int id);

        // Voegt een nieuw onderdeel toe aan de database
        public void AddPart(Part part);

        // Werkt een bestaand onderdeel bij
        public void UpdatePart(Part part);

        // Verwijdert een onderdeel uit de database
        public void DeletePart(Part part);
    }
}
