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
    // Repository voor het beheren van orders in de database
    public class OrderRepository : IOrderRepository
    {
        // Database context voor toegang tot orders
        private readonly MatrixIncDbContext _context;

        // Constructor voor dependency injection van de context
        public OrderRepository(MatrixIncDbContext context)
        {
            _context = context;
        }

        // Voegt een nieuwe order toe aan de database
        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        // Verwijdert een order uit de database
        public void DeleteOrder(Order order)
        {
            _context.Orders.Remove(order);
            _context.SaveChanges();
        }

        // Haalt alle orders op inclusief klantinformatie
        public IEnumerable<Order> GetAllOrders()
        {
            return _context.Orders.Include(o => o.Customer);
        }

        // Haalt een order op basis van het unieke ID inclusief klantinformatie
        public Order? GetOrderById(int id)
        {
            return _context.Orders.Include(o => o.Customer).FirstOrDefault(o => o.Id == id);
        }

        // Werkt een bestaande order bij
        public void UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
        }
    }
}
