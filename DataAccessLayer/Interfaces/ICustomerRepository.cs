using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    // Interface voor klantenrepository
    // Definieert methoden voor het beheren van klanten in de database
    public interface ICustomerRepository
    {
        // Haalt alle klanten op
        public IEnumerable<Customer> GetAllCustomers();

        // Haalt een klant op basis van het unieke ID
        public Customer? GetCustomerById(int id);

        // Voegt een nieuwe klant toe aan de database
        public void AddCustomer(Customer customer);

        // Werkt een bestaande klant bij
        public void UpdateCustomer(Customer customer);

        // Verwijdert een klant uit de database
        public void DeleteCustomer(Customer customer);
    }
}
