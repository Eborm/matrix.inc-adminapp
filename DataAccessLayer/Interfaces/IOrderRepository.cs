using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    // Interface voor orderrepository
    // Definieert methoden voor het beheren van orders in de database
    public interface IOrderRepository
    {
        // Haalt alle orders op
        public IEnumerable<Order> GetAllOrders();

        // Haalt een order op basis van het unieke ID
        public Order? GetOrderById(int id);

        // Voegt een nieuwe order toe aan de database
        public void AddOrder(Order order);

        // Werkt een bestaande order bij
        public void UpdateOrder(Order order);

        // Verwijdert een order uit de database
        public void DeleteOrder(Order order);
    }
}
