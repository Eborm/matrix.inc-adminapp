using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    // Repository voor het beheren van onderdelen in de database
    public class PartRepository : IPartRepository
    {
        // Database context voor toegang tot onderdelen
        private readonly MatrixIncDbContext _context;

        // Constructor voor dependency injection van de context
        public PartRepository(MatrixIncDbContext context) 
        {
            _context = context; 
        }   

        // Voegt een nieuw onderdeel toe aan de database
        public void AddPart(Part part)
        {
            _context.Parts.Add(part);
            _context.SaveChanges();
        }

        // Verwijdert een onderdeel uit de database
        public void DeletePart(Part part)
        {
            _context.Parts.Remove(part);
            _context.SaveChanges();
        }

        // Haalt alle onderdelen op
        public IEnumerable<Part> GetAllParts()
        {
            return _context.Parts;            
        }

        // Haalt een onderdeel op basis van het unieke ID
        public Part? GetPartById(int id)
        {
            return _context.Parts.FirstOrDefault(p => p.Id == id);
        }

        // Werkt een bestaand onderdeel bij
        public void UpdatePart(Part part)
        {
            _context.Parts.Update(part);
            _context.SaveChanges();
        }
    }
}
