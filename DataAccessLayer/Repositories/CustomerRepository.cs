using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    // Repository voor het beheren van klanten in de database
    public class CustomerRepository : ICustomerRepository
    {
        // Database context voor toegang tot klanten
        private readonly MatrixIncDbContext _context;

        // Constructor voor dependency injection van de context
        public CustomerRepository(MatrixIncDbContext context)
        {
            _context = context;
        }

        // Voegt een nieuwe klant toe aan de database
        public void AddCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
        }

        // Verwijdert een klant uit de database
        public void DeleteCustomer(Customer customer)
        {
            _context.Customers.Remove(customer);
            _context.SaveChanges();
        }

        // Haalt alle klanten op inclusief hun orders
        public IEnumerable<Customer> GetAllCustomers()
        {
            return _context.Customers.Include(c => c.Orders);
        }

        // Haalt een klant op basis van het unieke ID inclusief orders
        public Customer? GetCustomerById(int id)
        {
            return _context.Customers.Include(c => c.Orders).FirstOrDefault(c => c.Id == id);
        }

        // Werkt een bestaande klant bij
        public void UpdateCustomer(Customer customer)
        {
            _context.Customers.Update(customer);
            _context.SaveChanges();
        }
    }
}
